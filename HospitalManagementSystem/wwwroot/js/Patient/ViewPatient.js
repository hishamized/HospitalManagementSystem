$(document).ready(function () {
    // Show modal
    $('#addHistoryBtn').on('click', function () {
        $('#medicalHistoryModal').removeClass('hidden').hide().fadeIn(200);
    });

    // Hide modal
    $('#closeModal').on('click', function () {
        $('#medicalHistoryModal').fadeOut(200, function () {
            $(this).addClass('hidden');
        });
    });

    // Submit form
    $('#medicalHistoryForm').on('submit', function (e) {
        e.preventDefault();

        var formData = {
            PatientId: $('#patientId').val(),
            Condition: $('#condition').val(),
            Diagnosis: $('#diagnosis').val(),
            Treatment: $('#treatment').val(),
            Medications: $('#medications').val(),
            Notes: $('#notes').val(),
            RecordDate: $('#recordDate').val(),
            AttendingPhysician: $('#attendingPhysician').val()
        };

        $.ajax({
            url: '/MedicalHistory/AddMedicalHistory',
            type: 'POST',
            data: formData,
            success: function (response) {
                $('#formMessage')
                    .text('✅ Medical history added successfully!')
                    .removeClass('text-red-500')
                    .addClass('text-green-500');
                console.log('Success:', response);

                // Hide modal after 2 seconds
                setTimeout(() => $('#medicalHistoryModal').fadeOut(200).addClass('hidden'), 1500);
            },
            error: function (xhr) {
                $('#formMessage')
                    .text('❌ Failed to add medical history. Check console for details.')
                    .removeClass('text-green-500')
                    .addClass('text-red-500');
                console.error('Error:', xhr.responseText);
            }
        });
    });
    // Toggle Medical History Table
    $('#viewHistoryBtn').on('click', function () {
        const section = $('#medicalHistorySection');
        section.toggleClass('hidden');

        if (!section.hasClass('hidden')) {
            const patientId = $('#patientId').val();

            $.ajax({
                url: `/MedicalHistory/GetMedicalHistoryByPatient?id=${patientId}`,
                type: 'GET',
                success: function (data) {
                    const tbody = $('#medicalHistoryTableBody');
                    tbody.empty();

                    if (data && data.length > 0) {
                        data.forEach(history => {
                            tbody.append(`
                                    <tr class="border-b border-gray-700 hover:bg-gray-900 transition">
                                        <td class="px-4 py-2">${history.condition || '-'}</td>
                                        <td class="px-4 py-2">${history.diagnosis || '-'}</td>
                                        <td class="px-4 py-2">${history.treatment || '-'}</td>
                                        <td class="px-4 py-2">${history.medications || '-'}</td>
                                        <td class="px-4 py-2">${new Date(history.recordDate).toLocaleDateString()}</td>
                                        <td class="px-4 py-2">${history.attendingPhysician || '-'}</td>
                                            <td class="px-4 py-2">
                                        <button
                                            class="bg-red-500 text-white px-2 py-1 rounded editHistoryBtn"
                                            data-id="${history.id}"
                                            data-condition="${history.condition}"
                                            data-diagnosis="${history.diagnosis}"
                                            data-treatment="${history.treatment}"
                                            data-medications="${history.medications}"
                                            data-notes="${history.notes}"
                                            data-recorddate="${history.recordDate}"
                                            data-physician="${history.attendingPhysician}">
                                            Edit
                                        </button>
                                       <a href="javascript:void(0);"
                                               class="deleteHistoryBtn text-red-500 hover:text-red-700 bg-red-500 text-white px-2 py-1 my-2"
                                           data-id="${history.id}">
                                           Delete
                                        </a>
                                    </td>
                                    </tr>
                                `);
                        });
                    } else {
                        tbody.append(`<tr><td colspan="6" class="text-center py-3 text-gray-400">No medical history found.</td></tr>`);
                    }
                },
                error: function (xhr) {
                    console.error('Error fetching medical history:', xhr.responseText);
                }
            });
        }
    });

    // Open modal and fill data
    $(document).on('click', '.editHistoryBtn', function () {
        const btn = $(this);

        $('#editHistoryId').val(btn.data('id'));
        $('#editCondition').val(btn.data('condition'));
        $('#editDiagnosis').val(btn.data('diagnosis'));
        $('#editTreatment').val(btn.data('treatment'));
        $('#editMedications').val(btn.data('medications'));
        $('#editNotes').val(btn.data('notes'));
        $('#editRecordDate').val(new Date(btn.data('recorddate')).toISOString().split('T')[0]);
        $('#editPhysician').val(btn.data('physician'));

        $('#editHistoryModal').removeClass('hidden').addClass('flex');
    });

    // Close modal
    $('#closeEditModal').on('click', function () {
        $('#editHistoryModal').addClass('hidden').removeClass('flex');
    });

    // Submit edit form
    $('#editHistoryForm').on('submit', function (e) {
        e.preventDefault();

        const dto = {
            id: $('#editHistoryId').val(),
            condition: $('#editCondition').val(),
            diagnosis: $('#editDiagnosis').val(),
            treatment: $('#editTreatment').val(),
            medications: $('#editMedications').val(),
            notes: $('#editNotes').val(),
            recordDate: new Date($('#editRecordDate').val()).toISOString(),
            attendingPhysician: $('#editPhysician').val()
        };

        $.ajax({
            url: '/MedicalHistory/UpdateMedicalHistory',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (res) {
                alert(res.message);
                $('#editHistoryModal').addClass('hidden').removeClass('flex');
                $('#viewHistoryBtn').click(); // Refresh table
            },
            error: function (xhr) {
                console.error('Error updating medical history:', xhr.responseText);
            }
        });
    });
    $(document).on('click', '.deleteHistoryBtn', function () {
        const btn = $(this);
        const historyId = btn.data('id');

        if (!confirm('Are you sure you want to delete this medical history?')) {
            return; // User cancelled
        }

        $.ajax({
            url: '/MedicalHistory/DeleteMedicalHistory', // Controller action
            type: 'POST',
            contentType: 'application/json',
            data: historyId.toString(),

            success: function (res) {
                alert(res.message);
                // Optionally remove the row from table
                btn.closest('tr').remove();
            },
            error: function (xhr) {
                console.error('Error deleting medical history:', xhr.responseText);
                alert('Failed to delete medical history.');
            }
        });
    });

    // Toggle modal
    $('#addAllergyBtn').on('click', function () {
        $('#addAllergyModal').removeClass('hidden').addClass('flex');
    });

    $('#closeAddAllergyModal').on('click', function () {
        $('#addAllergyModal').addClass('hidden').removeClass('flex');
    });

    // Submit form
    $('#addAllergyForm').on('submit', function (e) {
        e.preventDefault();

        const dto = {
            patientId: parseInt($('#allergyPatientId').val()),
            allergen: $('#allergen').val(),
            reaction: $('#reaction').val(),
            notes: $('#allergyNotes').val()
        };

        $.ajax({
            url: '/Allergy/AddAllergy',  // Controller action
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (res) {
                alert(res.message);
                $('#addAllergyModal').addClass('hidden').removeClass('flex');
                // Optionally refresh allergy table
            },
            error: function (xhr) {
                console.error('Error adding allergy:', xhr.responseText);
                alert('Failed to add allergy.');
            }
        });
    });

    $(document).on('click', '#viewAllergiesBtn', function () {
        const patientId = $('#patientId').val(); // lowercase 'p', matches the hidden input
        const container = $('#allergiesTableContainer');
        const button = $(this);

        // If table is already visible, just hide it
        if (!container.hasClass('hidden')) {
            container.addClass('hidden');
            button.text('👁 View Allergies'); // reset button text
            return;
        }

        // Otherwise, fetch allergies via AJAX
        $.ajax({
            url: '/Allergy/GetPatientAllergies',
            type: 'GET',
            data: { patientId: patientId },
            success: function (res) {
                const tbody = $('#allergiesTableBody');
                tbody.empty();

                if (res && res.length > 0) {
                    res.forEach(allergy => {
                        tbody.append(`
                        <tr class="hover:bg-gray-700 transition-colors duration-200">
                            <td class="px-6 py-4">${allergy.allergen}</td>
                            <td class="px-6 py-4">${allergy.reaction}</td>
                            <td class="px-6 py-4">${allergy.notes ?? ''}</td>
                            <td class="px-6 py-4">${new Date(allergy.createdAt).toLocaleDateString()}</td>
                                     <td class="px-6 py-4 text-center space-x-2">
                            <button
                                class="editAllergyBtn bg-yellow-500 hover:bg-yellow-600 text-white px-3 py-1 rounded transition duration-200"
                                data-id="${allergy.id}"
                                data-allergen="${allergy.allergen}"
                                data-reaction="${allergy.reaction}"
                                   data-notes='${allergy.notes}'>
                                ✏ Edit
                            </button>
                            <button 
                                class="deleteAllergyBtn bg-red-600 hover:bg-red-700 text-white px-3 py-1 rounded transition duration-200"
                                data-id="${allergy.id}">
                                🗑 Delete
                            </button>
                        </td>
                        </tr>
                    `);
                    });
                } else {
                    tbody.append(`<tr><td colspan="4" class="text-center py-4">No allergies found.</td></tr>`);
                }

                container.removeClass('hidden');
                button.text('Hide Allergies'); // change button text
            },
            error: function (xhr) {
                console.error('Error fetching allergies:', xhr.responseText);
                alert('Failed to load allergies.');
            }
        });
    });

    // Show modal and populate fields
    $(document).on('click', '.editAllergyBtn', function () {
        $('#editAllergyModal').removeClass('hidden flex').addClass('flex');
        $('#editAllergyId').val($(this).data('id'));
        $('#editAllergen').val($(this).data('allergen'));
        $('#editReaction').val($(this).data('reaction'));
        $('#editAllergyNotes').val($(this).data('notes'));
    });

    // Hide modal
    $('#cancelEditBtn').on('click', function () {
        $('#editAllergyModal').addClass('hidden').removeClass('flex');
    });

    // Submit edit form
    $('#editAllergyForm').on('submit', function (e) {
        e.preventDefault();

        const updatedData = {
            id: $('#editAllergyId').val(),
            patientId: $('#editPatientId').val(), // make sure you include PatientId
            allergen: $('#editAllergen').val(),
            reaction: $('#editReaction').val(),
            notes: $('#editAllergyNotes').val(), // your original code had typo: #editNotes
        };

        $.ajax({
            url: '/Allergy/EditAllergy',
            type: 'POST',
            contentType: 'application/json', // tell server this is JSON
            data: JSON.stringify(updatedData), // convert JS object to JSON
            success: function (res) {
                alert('Allergy updated successfully!');
                $('#editAllergyModal').addClass('hidden').removeClass('flex');
                $('#viewAllergiesBtn').trigger('click'); // refresh table
            },
            error: function (xhr) {
                console.error('Error updating allergy:', xhr.responseText);
                alert('Failed to update allergy.');
            }
        });
    });

    $(document).on('click', '.deleteAllergyBtn', function () {
        const btn = $(this);
        const allergyId = btn.data('id');
        if(!confirm('Are you sure you want to delete this allergy?')) {
            return; // User cancelled
        }

        $.ajax({
            url: '/Allergy/DeleteAllergy',
            type: 'POST',
            contentType: 'application/json',
            data: allergyId.toString(),
            success: function (res) {
                alert('Allergy deleted successfully!');
                $('#viewAllergiesBtn').trigger('click'); 
            },
            error: function (xhr) {
                console.error('Error deleting allergy:', xhr.responseText);
                alert('Failed to delete allergy.');
            }
        });
    });

    // Add Inaurance form handling
    $(document).ready(function () {
        // Open modal
        $('#addInsuranceBtn').on('click', function () {
            $('#insuranceModal').removeClass('hidden');
        });

        // Close modal
        $('#closeInsuranceModal').on('click', function () {
            $('#insuranceModal').addClass('hidden');
        });

        // Submit form via AJAX
        $('#addInsuranceForm').on('submit', function (e) {
            e.preventDefault();

            var formData = {
                PatientId: $('#PatientId').val(),
                ProviderName: $('#ProviderName').val(),
                PolicyNumber: $('#PolicyNumber').val(),
                StartDate: $('#StartDate').val(),
                EndDate: $('#EndDate').val()
            };

            $.ajax({
                url: '/Insurance/Add', // Controller action to be created
                type: 'POST',
                data: formData,
                success: function (response) {
                    if (response.success) {
                        alert('Insurance added successfully! ID: ' + response.insuranceId);
                        $('#insuranceModal').addClass('hidden');
                        $('#addInsuranceForm')[0].reset();
                    } else {
                        alert('Error: ' + response.message);
                    }
                },
                error: function (xhr) {
                    var errMsg = 'Unknown error';
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errMsg = xhr.responseJSON.message;
                    }
                    alert('Error adding insurance: ' + errMsg);
                    console.error(xhr);
                }
            });
        });
    });

    // Toggle table visibility
    $('#toggleInsuranceBtn').on('click', function () {
        $('#insuranceTableContainer').toggleClass('hidden');
        if (!$('#insuranceTableContainer').hasClass('hidden')) {
            loadInsuranceData();
        }
    });

    // Function to load insurance data via AJAX
    function loadInsuranceData() {
        var patientId = $('#toggleInsuranceBtn').data('patient-id');

        $.ajax({
            url: '/Insurance/GetByPatient', // Controller action to fetch insurance
            type: 'GET',
            data: { patientId: patientId },
            success: function (response) {
                // Clear existing rows
                $('#insuranceTableBody').empty();

                if (response.length === 0) {
                    $('#insuranceTableBody').append(
                        '<tr><td colspan="7" class="text-center py-4">No insurance records found.</td></tr>'
                    );
                } else {
                    // Populate table
                    $.each(response, function (index, insurance) {
                        $('#insuranceTableBody').append(
                            '<tr class="border-b hover:bg-gray-100">' +
                            '<td class="px-4 py-2">' + (index + 1) + '</td>' +
                            '<td class="px-4 py-2">' + insurance.providerName + '</td>' +
                            '<td class="px-4 py-2">' + insurance.policyNumber + '</td>' +
                            '<td class="px-4 py-2">' + formatDate(insurance.startDate) + '</td>' +
                            '<td class="px-4 py-2">' + formatDate(insurance.endDate) + '</td>' +
                            '<td class="px-4 py-2">' + formatDateTime(insurance.createdAt) + '</td>' +
                            '<td class="px-4 py-2">' + formatDateTime(insurance.updatedAt) + '</td>' +
                            '</tr>'
                        );
                    });
                }
            },
            error: function (err) {
                alert('Error loading insurance data');
                console.error(err);
            }
        });
    }

    // Helper functions to format dates
    function formatDate(dateString) {
        var date = new Date(dateString);
        return date.toLocaleDateString();
    }

    function formatDateTime(dateString) {
        var date = new Date(dateString);
        return date.toLocaleString();
    }
});