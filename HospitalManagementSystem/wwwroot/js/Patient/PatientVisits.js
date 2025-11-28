$(document).ready(function () {
    // 1️⃣ Column Definitions for AG Grid
    var columnDefs = [
        { headerName: "Visit ID", field: "id", width: 100, filter: true, sortable: true },
        { headerName: "Patient ID", field: "patientId", width: 100, filter: true, sortable: true },
        { headerName: "Patient", field: "patientName", width: 200, filter: true, sortable: true },
        { headerName: "Visit Type", field: "visitType", width: 150, filter: true, sortable: true },
        { headerName: "Visit Date", field: "visitDate", width: 150, filter: 'agDateColumnFilter', sortable: true },
        { headerName: "Doctor Name", field: "doctorName", width: 200, filter: true },
        { headerName: "Admission", field: "admissionDate", width: 150, filter: 'agDateColumnFilter', sortable: true },
        { headerName: "Discharge", field: "dischargeDate", width: 150, filter: 'agDateColumnFilter', sortable: true },
        { headerName: "Room", field: "roomNumber", width: 100, filter: true },
        { headerName: "Treatment Details", field: "treatmentDetails", width: 250, filter: true },
        { headerName: "Notes", field: "notes", width: 250, filter: true },
        {
            headerName: "Actions",
            field: "id",
            width: 300,
            cellRenderer: function (params) {
                return `
            <div class="flex gap-2">
                <button 
                    class="px-3 py-1 text-sm rounded-lg bg-blue-600 text-white hover:bg-blue-700 view-btn"
                    data-id="${params.value}" 
                    data-patientid="${params.data.patientId}" 
                    data-visittype="${params.data.visitType}">
                    View
                </button>

                <button 
                    class="px-3 py-1 text-sm rounded-lg bg-yellow-600 text-black hover:bg-yellow-700 edit-btn"
                    data-id="${params.value}">
                    Edit
                </button>

                <button 
                    class="px-3 py-1 text-sm rounded-lg bg-red-600 text-white hover:bg-red-700 delete-btn"
                    data-id="${params.value}">
                    Delete
                </button>
            </div>
        `;
            },
            sortable: false,
            filter: false
        }

    ];



    var gridOptions = {
        columnDefs: columnDefs,
        defaultColDef: {
            filter: true,
            sortable: true,
            resizable: true,
            minWidth: 100
        },
        pagination: true,
        paginationPageSize: 20,
        getRowNodeId: data => data.id,
        rowSelection: 'single',
        animateRows: true,
        domLayout: 'autoHeight', // or 'normal' if you want fixed height + scrollbar
    };


    // 3️⃣ Initialize Grid
    var eGridDiv = document.querySelector('#patientVisitGrid');
    new agGrid.Grid(eGridDiv, gridOptions);

    // 4️⃣ Load Data via AJAX
    function loadPatientVisits() {
        $.ajax({
            url: '/PatientVisit/GetAll',
            type: 'GET',
            success: function (data) {
                gridOptions.api.setRowData(data);
            },
            error: function (xhr, status, error) {
                console.error('Error loading patient visits:', status, error);
            }
        });
    }

    loadPatientVisits();


    // 6️⃣ Delete Button Click
    $('#patientVisitGrid').on('click', '.delete-btn', function () {
        var visitId = $(this).data('id');
        selectedVisitId = visitId; // store ID if needed

        // Show Tailwind modal
        $('#deleteVisitModal').removeClass('hidden');
    });


    function getDoctorList(selectId, selectedDoctorId) {
        $.ajax({
            url: '/Doctor/GetAllDoctors',
            type: 'GET',
            success: function (res) {
                if (res.success) {
                    var data = res.data;
                    var select = $(selectId);
                    select.empty().append('<option value="">Select Doctor</option>');

                    $.each(data, function (i, doctor) {
                        select.append(
                            `<option value="${doctor.id}" ${doctor.id == selectedDoctorId ? 'selected' : ''}>
                            ${doctor.fullName}
                         </option>`
                        );
                    });
                }
            }
        });
    }


    $('#doctorNameSelect').on('change', function () {
        $('#DoctorId').val($(this).val());
        $('#DoctorName').val($(this).find("option:selected").text().trim());
    });


    $('#editDoctorSelect').on('change', function () {
        $('#editDoctorId').val($(this).val());
        $('#editDoctorName').val($(this).find("option:selected").text().trim());
    });


    // 1️⃣ Show modal on button click
    $("#btnAddVisit").on("click", function () {

        getDoctorList('#doctorNameSelect', '');
        loadPatients();
        getDoctorList('#doctorNameSelect', '');
        $("#visitModalLabel").text("Add Patient Visit");
        $("#visitForm")[0].reset();
        $(".inpatient-field").hide();

        // Show Tailwind modal
        $("#visitModal").removeClass("hidden");
    });


    // 2️⃣ Toggle inpatient-specific fields
    $("#visitType").on("change", function () {
        if ($(this).val() === "Inpatient") {
            $(".inpatient-field").slideDown();
        } else {
            $(".inpatient-field").slideUp();
        }
    });
    // Add this below your #visitType change handler
$("#editVisitType").on("change", function () {
    if ($(this).val() === "Inpatient") {
        $("#editVisitModal .inpatient-field").slideDown();
    } else {
        $("#editVisitModal .inpatient-field").slideUp();
    }
});


    // 3️⃣ Load patient list from controller
    function loadPatients() {
        $.ajax({
            url: '/Patient/GetAllPatients',
            type: 'GET',
            success: function (data) {
                var select = $("#patientId");
                select.empty().append('<option value="">Select Patient</option>');
                $.each(data, function (i, patient) {
                    select.append('<option value="' + patient.id + '">' + patient.fullName + '</option>');
                });
            },
            error: function () {
                alert("Failed to load patients. Please refresh the page.");
            }
        });
    }

    // Load patients when modal opens
    $('#visitModal').on('show.bs.modal', function () {
        loadPatients();
    });

    // 4️⃣ AJAX to add patient visit
    $('#saveVisitBtn').on('click', function () {
        var form = $('#visitForm')[0];
        var formData = new FormData(form);

        // OVERRIDE FORM DATA HERE
        var selectedDoctorId = $('#doctorNameSelect').val();
        var selectedDoctorName = $('#doctorNameSelect option:selected').text().trim();


        formData.set("DoctorId", selectedDoctorId);
        formData.set("DoctorName", selectedDoctorName);

        $.ajax({
            url: '/PatientVisit/Add',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.success) {
                    //console.log('Patient visit added successfully:', res.visitId);
                    // Hide modal (Tailwind)
                    document.getElementById("visitModal").classList.add("hidden");

                    loadPatientVisits();
                    // TODO: Refresh AG Grid here
                } else {
                    console.warn('Failed to add patient visit:', res.message);
                }
            },
            error: function (xhr, status, error) {
                var response = JSON.parse(xhr.responseText);

                var message = response.message || "An unexpected error occurred.";

                // If there are validation errors, combine them all
                if (response.errors && response.errors.length > 0) {
                    message = "";
                    for (var i = 0; i < response.errors.length; i++) {
                        message += "- " + response.errors[i].errorMessage + "\n";
                    }
                }
                console.error('AJAX Error:', status, error, xhr.responseText);
                $("#ErrorContainer").text(message).removeClass("d-none");
            }
        });
    });

    // Delete Record
    var selectedVisitId = null;

    $('#patientVisitGrid').on('click', '.delete-btn', function () {
        selectedVisitId = $(this).data('id');

        // Show Tailwind modal (remove hidden)
        $('#deleteVisitModal').removeClass('hidden');
    });


    $('#confirmDeleteVisitBtn').on('click', function () {
        if (!selectedVisitId) return;

        $.ajax({
            url: '/PatientVisit/Delete',
            type: 'POST',
            data: { id: selectedVisitId },
            success: function (res) {
                if (res.success) {
                    loadPatientVisits();
                    // Remove from AG Grid
                    var rowNode = gridOptions.api.getRowNode(selectedVisitId.toString());
                    if (rowNode) gridOptions.api.applyTransaction({ remove: [rowNode.data] });
                } else {
                    console.warn('Delete failed:', res.message);
                }
                $('#deleteVisitModal').addClass('hidden');
            },
            error: function (xhr, status, error) {
                console.error('AJAX Delete Error:', status, error, xhr.responseText);
            }
        });
    });

    // When Edit button is clicked
    $(document).on('click', '.edit-btn', function () {
        const visitId = $(this).data('id');



        // Find the row data
        let visitData;
        gridOptions.api.forEachNode(node => {
            if (node.data.id == visitId) visitData = node.data;
        });
        if (!visitData) return;

        //console.log(visitData);
        // Populate modal fields
        $('#editVisitId').val(visitData.id);

        getDoctorList('#editDoctorSelect', visitData.doctorId);


        // Fix Patient select
        const patientSelect = $('#editPatientId');
        patientSelect.empty(); // remove previous options
        patientSelect.append(`<option value="${visitData.patientId}" selected>${visitData.patientName}</option>`);
        patientSelect.prop('disabled', true); // prevent changing

        // Other fields
        $('#editVisitType').val(visitData.visitType);
        $('#editVisitDate').val(toDateTimeLocalFormat(visitData.visitDate));
        $('#editDoctorName').val(visitData.doctorName ?? '');
        $('#editDoctorId').val(visitData.doctorId ?? '');
        $('#editAdmissionDate').val(toDateTimeLocalFormat(visitData.admissionDate));
        $('#editDischargeDate').val(toDateTimeLocalFormat(visitData.dischargeDate));
        $('#editRoomNumber').val(visitData.roomNumber ?? '');
        $('#editTreatmentDetails').val(visitData.treatmentDetails ?? '');
        $('#editNotes').val(visitData.notes ?? '');

        // Show/hide inpatient fields
        if (visitData.visitType === 'Inpatient') $('.inpatient-field').show();
        else $('.inpatient-field').hide();

        // Show modal
        document.getElementById("editVisitModal").classList.remove("hidden");

    });

    // When update button is clicked
    $('#updatePatientVisitBtn').click(function () {
        const dto = {
            Id: $('#editVisitId').val(),
            PatientId: $('#editPatientId').val(),
            VisitType: $('#editVisitType').val(),
            VisitDate: $('#editVisitDate').val(),
            DoctorId: $('#editDoctorId').val(),
            DoctorName: $('#editDoctorName').val().trim(),
            AdmissionDate: $('#editAdmissionDate').val() || null,
            DischargeDate: $('#editDischargeDate').val() || null,
            RoomNumber: $('#editRoomNumber').val() || null,
            TreatmentDetails: $('#editTreatmentDetails').val() || null,
            Notes: $('#editNotes').val() || null,
            UpdatedAt: new Date().toISOString()
        };

        $('#editVisitError').text('');

        $.ajax({
            url: '/PatientVisit/UpdatePatientVisit',
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (response) {
                // Check the 'success' property
                if (response && response.success) {
                    loadPatientVisits();
                    document.getElementById('editVisitModal').classList.add('hidden');
                    fetchPatientVisitList();
                    alert(response.message || 'Patient visit updated successfully!');
                } else {
                    $('#editVisitError').text(response.message || 'Update failed. No rows were affected.');
                }
            },
            error: function (xhr) {
                let errorMessage = 'An unexpected error occurred. Please try again.';

                // If server sent a JSON response
                if (xhr.responseJSON) {
                    const response = xhr.responseJSON;

                    // If validation-style message exists inside details
                    if (response.details && response.details.includes('Doctor name')) {
                        // Extract the clean message using regex
                        const match = response.details.match(/--\s.*?:\s(.*?)\. Severity/i);
                        if (match && match[1]) {
                            errorMessage = match[1].trim() + '.';
                        } else {
                            errorMessage = response.message || 'Validation failed.';
                        }
                    }
                    else if (response.errors && Array.isArray(response.errors)) {
                        // Handle array-based validation errors
                        errorMessage = response.errors.map(e => e.errorMessage).join('\n');
                    }
                    else if (response.message) {
                        errorMessage = response.message;
                    }
                }

                // Otherwise, use plain text response
                else if (xhr.responseText) {
                    try {
                        const parsed = JSON.parse(xhr.responseText);
                        errorMessage = parsed.message || errorMessage;
                    } catch {
                        errorMessage = xhr.responseText;
                    }
                }

                $('#editVisitError')
                    .removeClass('text-success')
                    .addClass('text-danger fw-semibold')
                    .text(errorMessage);
            }

        });
    });
    document.addEventListener('click', function (e) {
        // Handle View button
        if (e.target.classList.contains('view-btn')) {
            const id = e.target.getAttribute('data-id');
            const patientId = e.target.getAttribute('data-patientid');
            const visitType = e.target.getAttribute('data-visittype');

            if (visitType.toLowerCase() === 'inpatient') {
                handleInpatientView(id, patientId);
            } else if (visitType.toLowerCase() === 'outpatient') {
                handleOutpatientView(id, patientId);
            } else {
                console.warn("Unknown visit type:", visitType);
            }
        }
    });

    function handleInpatientView(visitId, patientId) {
        //console.log("View Inpatient record => Visit ID:", visitId, "| Patient ID:", patientId);

        // ✅ Redirect to the InpatientVisitManager view with both IDs
        const url = `/PatientVisit/InpatientVisitManager?patientId=${patientId}&visitId=${visitId}`;
        window.open(url, '_blank'); // opens in new tab (as you mentioned earlier)
    }


    function handleOutpatientView(visitId, patientId) {
        //console.log("View Outpatient record => Visit ID:", id, "| Patient ID:", patientId);
        const url = `/PatientVisit/OutpatientVisitManager?patientId=${patientId}&visitId=${visitId}`;
        window.open(url, '_blank');
    }

    function toDateTimeLocalFormat(dateString) {
        if (!dateString) return "";

        const date = new Date(dateString);

        const year = date.getFullYear();
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        const day = date.getDate().toString().padStart(2, '0');

        const hours = date.getHours().toString().padStart(2, '0');
        const minutes = date.getMinutes().toString().padStart(2, '0');

        return `${year}-${month}-${day}T${hours}:${minutes}`;
    }

});