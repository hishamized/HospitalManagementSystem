$(document).ready(function () {

    // === Elements ===
    const gridDiv = document.querySelector('#doctorGrid');
    const messageContainer = $('#messageContainer');

    // === AG Grid Column Definitions ===
    const columnDefs = [
        { headerName: "Doctor Code", field: "doctorCode", sortable: true, filter: true, resizable: true, width: 150 },
        { headerName: "Full Name", field: "fullName", sortable: true, filter: true, resizable: true, flex: 1 },
        { headerName: "Gender", field: "gender", sortable: true, filter: true, resizable: true, width: 100 },
        { headerName: "Specialization", field: "specialization", sortable: true, filter: true, resizable: true, flex: 1 },
        { headerName: "Qualification", field: "qualification", sortable: true, filter: true, resizable: true, flex: 1 },
        { headerName: "Experience", field: "experienceYears", sortable: true, filter: true, resizable: true, width: 120 },
        { headerName: "Email", field: "email", sortable: true, filter: true, resizable: true, flex: 1 },
        { headerName: "Phone", field: "phoneNumber", sortable: true, filter: true, resizable: true, width: 150 },
        { headerName: "City", field: "city", sortable: true, filter: true, resizable: true, width: 120 },
        { headerName: "Slot", field: "slotName", sortable: true, filter: true, resizable: true, width: 150 },
        { headerName: "Department", field: "departmentName", sortable: true, filter: true, resizable: true, width: 150 },
        {
            headerName: "Actions",
            width: 180,
            cellRenderer: function (params) {
                return `
                    <div class="flex gap-2">
                        <button 
                            class="px-3 py-1 text-sm rounded-lg bg-purple-600 hover:bg-purple-700 text-white transition edit-btn"
                            data-id="${params.data.id}">
                            <i class="fas fa-edit"></i> Edit
                        </button>
                        <button 
                            class="px-3 py-1 text-sm rounded-lg bg-red-600 hover:bg-red-700 text-white transition delete-btn"
                            data-id="${params.data.id}">
                            <i class="fas fa-trash"></i> Delete
                        </button>
                    </div>
                `;
            },
            suppressMenu: true,
            sortable: false,
            filter: false
        }
    ];

    // === AG Grid Options ===
    const gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        pagination: true,
        paginationPageSize: 20,
        animateRows: true,
        rowHeight: 50,
        getRowNodeId: data => data.id,
        defaultColDef: {
            minWidth: 100,
            sortable: true,
            filter: true,
            resizable: true
        },
        onGridReady: function (params) {
            params.api.sizeColumnsToFit();
            loadDoctors();
        },
        onGridSizeChanged: function (params) {
            params.api.sizeColumnsToFit();
        },
        getContextMenuItems: function (params) {
            return [
                {
                    name: 'Edit Doctor',
                    action: function () {
                        handleEdit(params.node.data.id);
                    }
                },
                {
                    name: 'Delete Doctor',
                    action: function () {
                        handleDelete(params.node.data.id);
                    }
                },
                'separator',
                'copy',
                'export'
            ];
        }
    };

    // === Initialize Grid ===
    new agGrid.Grid(gridDiv, gridOptions);

    // === Load Doctors via AJAX ===
    function loadDoctors() {
        $.ajax({
            url: '/Doctor/GetAllDoctors',
            method: 'GET',
            dataType: 'json',
            beforeSend: function () {
                showMessage('Loading doctors, please wait...', 'info');
            },
            success: function (response) {
                console.log('Doctors response:', response);
                if (response.success && response.data && response.data.length > 0) {
                    gridOptions.api.setRowData(response.data);
                    clearMessage();
                } else {
                    gridOptions.api.setRowData([]);
                    showMessage('No doctors found in the database.', 'warning');
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", error);
                showMessage('An error occurred while fetching doctors. Please try again later.', 'danger');
            }
        });
    }

    // === Edit/Delete Button Handlers ===
    $(document).on('click', '.edit-btn', function (e) {
        e.preventDefault();
        const doctorId = parseInt($(this).data('id'));
        console.log('Edit clicked for ID:', doctorId);

        let foundData = null;
        gridOptions.api.forEachNode(function (node) {
            if (node.data && node.data.id === doctorId) {
                foundData = node.data;
            }
        });

        if (foundData) {
            handleEdit(foundData);
        } else {
            console.error('Could not find doctor with ID:', doctorId);
        }
    });

    $(document).on('click', '.delete-btn', function (e) {
        e.preventDefault();
        const doctorId = parseInt($(this).data('id'));
        console.log('Delete clicked for ID:', doctorId);

        let foundData = null;
        gridOptions.api.forEachNode(function (node) {
            if (node.data && node.data.id === doctorId) {
                foundData = node.data;
            }
        });

        if (foundData) {
            handleDelete(foundData.id);
        } else {
            console.error('Could not find doctor with ID:', doctorId);
        }
    });

    // === Handle Edit ===
    function handleEdit(doctor) {
        console.log('Editing doctor:', doctor);

        $('#editDoctorId').val(doctor.id);
        const names = doctor.fullName.split(' ');
        $('#editFirstName').val(names[0] || '');
        $('#editLastName').val(names.slice(1).join(' ') || '');
        $('#editGender').val(doctor.gender);
        $('#editEmail').val(doctor.email);
        $('#editPhoneNumber').val(doctor.phoneNumber);
        $('#editSpecialization').val(doctor.specialization);
        $('#editQualification').val(doctor.qualification);
        $('#editExperienceYears').val(doctor.experienceYears);
        $('#editCity').val(doctor.city);

        // Clear errors
        $('#editDoctorErrors').addClass('hidden').html('');

        // Load slots and departments with current selections
        loadSlotsAndDepartments(doctor.slotId, doctor.departmentId, true);

        // Show modal
        window.showModal('editDoctorModal');
    }

    // === Handle Delete ===
    function handleDelete(id) {
        if (!confirm("Are you sure you want to delete this doctor?")) {
            return;
        }

        $.ajax({
            url: `/Doctor/DeleteDoctor/${id}`,
            type: 'DELETE',
            success: function (response) {
                if (response.success) {
                    showMessage(response.message, 'success');
                    loadDoctors();
                } else {
                    showMessage(response.message, 'danger');
                }
            },
            error: function (xhr) {
                let errorMsg = "An error occurred while deleting the doctor.";
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMsg = xhr.responseJSON.message;
                }
                showMessage(errorMsg, 'danger');
                console.error("AJAX Error:", xhr);
            }
        });
    }

    // === Helper Functions ===
    function showMessage(message, type) {
        const alertHtml = `
            <div class="bg-${type === 'success' ? 'green' : type === 'danger' ? 'red' : type === 'warning' ? 'yellow' : 'blue'}-600/20 border border-${type === 'success' ? 'green' : type === 'danger' ? 'red' : type === 'warning' ? 'yellow' : 'blue'}-500 text-${type === 'success' ? 'green' : type === 'danger' ? 'red' : type === 'warning' ? 'yellow' : 'blue'}-300 p-4 rounded-lg mb-4 flex items-center justify-between">
                <span>${message}</span>
                <button onclick="this.parentElement.remove()" class="text-${type === 'success' ? 'green' : type === 'danger' ? 'red' : type === 'warning' ? 'yellow' : 'blue'}-300 hover:text-white">
                    <i class="fas fa-times"></i>
                </button>
            </div>`;
        messageContainer.html(alertHtml);

        // Auto-hide after 5 seconds
        setTimeout(clearMessage, 5000);
    }

    function clearMessage() {
        messageContainer.html('');
    }

    // === Load Slots and Departments ===
    function loadSlotsAndDepartments(selectedSlotId = null, selectedDeptId = null, isEdit = false) {
        // Load Slots
        $.ajax({
            url: '/Slot/GetAllSlots',
            type: 'GET',
            success: function (res) {
                console.log('Slots response:', res);
                const slots = res.data || res;

                const slotSelect = isEdit ? $('#editSlotId') : $('#SlotId');
                slotSelect.empty().append('<option value="">Select Slot</option>');

                if (Array.isArray(slots)) {
                    slots.forEach(slot => {
                        const selected = slot.id == selectedSlotId ? 'selected' : '';
                        slotSelect.append(`<option value="${slot.id}" ${selected}>${slot.reportingTime} - ${slot.leavingTime}</option>`);
                    });
                }
            },
            error: function () {
                console.error('Failed to load slots');
            }
        });

        // Load Departments
        $.ajax({
            url: '/Department/GetAll',
            type: 'GET',
            success: function (res) {
                console.log('Departments response:', res);
                const departments = res.data || res;

                const deptSelect = isEdit ? $('#editDepartmentId') : $('#DepartmentId');
                deptSelect.empty().append('<option value="">Select Department</option>');

                if (Array.isArray(departments)) {
                    departments.forEach(dep => {
                        const selected = dep.id == selectedDeptId ? 'selected' : '';
                        deptSelect.append(`<option value="${dep.id}" ${selected}>${dep.name}</option>`);
                    });
                }
            },
            error: function () {
                console.error('Failed to load departments');
            }
        });
    }

    // === Show Add Modal ===
    $('#btnAddDoctor').click(function () {
        $('#addDoctorForm')[0].reset();
        $('#doctorFormErrors').addClass('hidden').html('');
        window.showModal('addDoctorModal');
        loadSlotsAndDepartments();
    });

    // === Save New Doctor ===
    $('#saveDoctorBtn').click(function () {
        var formData = $('#addDoctorForm').serialize();

        $.ajax({
            url: '/Doctor/AddDoctor',
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    window.hideModal('addDoctorModal');
                    loadDoctors();
                    showMessage('Doctor added successfully!', 'success');
                } else {
                    $('#doctorFormErrors').removeClass('hidden').html(response.message || 'An error occurred.');
                }
            },
            error: function (xhr, status, error) {
                $('#doctorFormErrors').removeClass('hidden').html('Server error: ' + error);
            }
        });
    });

    // === Update Doctor ===
    $('#editDoctorBtn').on('click', function () {
        const dto = {
            id: parseInt($('#editDoctorId').val()),
            firstName: $('#editFirstName').val(),
            lastName: $('#editLastName').val(),
            gender: $('#editGender').val(),
            email: $('#editEmail').val(),
            phoneNumber: $('#editPhoneNumber').val(),
            specialization: $('#editSpecialization').val(),
            qualification: $('#editQualification').val(),
            experienceYears: parseInt($('#editExperienceYears').val()) || 0,
            city: $('#editCity').val(),
            slotId: parseInt($('#editSlotId').val()) || null,
            departmentId: parseInt($('#editDepartmentId').val()) || null
        };

        console.log('Updating doctor:', dto);

        // Clear previous errors
        $('#editDoctorErrors').html('').addClass('hidden');

        $.ajax({
            url: '/Doctor/EditDoctor',
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (response) {
                if (response.success) {
                    window.hideModal('editDoctorModal');
                    loadDoctors();
                    showMessage(response.message || 'Doctor updated successfully!', 'success');
                } else {
                    $('#editDoctorErrors').html(response.message).removeClass('hidden');
                }
            },
            error: function (xhr) {
                let errorMsg = "An error occurred while updating the doctor.";
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMsg = xhr.responseJSON.message;
                }
                $('#editDoctorErrors').html(errorMsg).removeClass('hidden');
                console.error("AJAX Error:", xhr);
            }
        });
    });

});