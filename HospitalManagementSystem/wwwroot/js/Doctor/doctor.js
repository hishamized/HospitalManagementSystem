$(document).ready(function () {

    // === Elements ===
    const gridDiv = document.querySelector('#doctorGrid');
    const messageContainer = $('#messageContainer');

    // === AG Grid Column Definitions ===
    const columnDefs = [
        { headerName: "Doctor Code", field: "doctorCode", sortable: true, filter: true, resizable: true },
        { headerName: "Full Name", field: "fullName", sortable: true, filter: true, resizable: true },
        { headerName: "Gender", field: "gender", sortable: true, filter: true, resizable: true },
        { headerName: "Specialization", field: "specialization", sortable: true, filter: true, resizable: true },
        { headerName: "Qualification", field: "qualification", sortable: true, filter: true, resizable: true },
        { headerName: "Experience (Years)", field: "experienceYears", sortable: true, filter: true, resizable: true },
        { headerName: "Email", field: "email", sortable: true, filter: true, resizable: true },
        { headerName: "Phone", field: "phoneNumber", sortable: true, filter: true, resizable: true },
        { headerName: "City", field: "city", sortable: true, filter: true, resizable: true },
        { headerName: "Slot", field: "slotName", sortable: true, filter: true, resizable: true },           // 🆕
        { headerName: "Department", field: "departmentName", sortable: true, filter: true, resizable: true }, // 🆕
        {
            headerName: "Actions",
            cellRenderer: function (params) {
                return `
                        <button class="btn btn-sm btn-warning me-1 edit-btn" data-id="${params.data.id}">Edit</button>
                        <button class="btn btn-sm btn-danger delete-btn" data-id="${params.data.id}">Delete</button>
                    `;
            },
            width: 180,
            suppressMenu: true
        }
    ];

    // === AG Grid Options ===
    const gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        pagination: true,
        paginationPageSize: 10,
        animateRows: true,
        getRowNodeId: data => data.id,
        defaultColDef: { flex: 1, minWidth: 120 },
        onGridReady: function () {
            loadDoctors();
        },
        getContextMenuItems: function (params) {
            // Add Edit/Delete to right-click context menu
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

    // === Edit/Delete Handlers ===
    $(document).on('click', '.edit-btn', function () {
        const doctorId = $(this).data('id');
        handleEdit(doctorId);
    });

    $(document).on('click', '.delete-btn', function () {
        const doctorId = $(this).data('id');
        handleDelete(doctorId);
    });

    function handleEdit(id) {
        const rowNode = gridOptions.api.getRowNode(id.toString());
        if (!rowNode) return;

        const doctor = rowNode.data;

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

        // Load slots and departments with current selections
        loadSlotsAndDepartments(doctor.slotId, doctor.departmentId);

        // Show modal
        const modal = new bootstrap.Modal(document.getElementById('editDoctorModal'));
        modal.show();
    }



    function handleDelete(id) {
        if (!confirm("Are you sure you want to delete this doctor?")) {
            return; // User cancelled
        }

        $.ajax({
            url: `/Doctor/DeleteDoctor/${id}`, // Controller action route
            type: 'DELETE',
            success: function (response) {
                if (response.success) {
                    // Optionally, show success toast/message
                    showMessage(response.message, 'success');

                    // Refresh AG Grid or remove row directly
                    loadDoctors();
                } else {
                    // Show server-returned error
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
                <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                    ${message}
                    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                </div>`;
        messageContainer.html(alertHtml);
    }

    function clearMessage() {
        messageContainer.html('');
    }

    // Initialize Bootstrap modal
    var addDoctorModal = new bootstrap.Modal(document.getElementById('addDoctorModal'));

    function loadSlotsAndDepartments(selectedSlotId = null, selectedDeptId = null) {
    // Load Slots
    $.ajax({
        url: '/Slot/GetAllSlots',
        type: 'GET',
        success: function (res) {
            if (res.success) {
                const slotSelect = $('#editSlotId');
                const mainSlotSelect = $('#SlotId');

                slotSelect.empty().append('<option value="">Select Slot</option>');
                res.data.forEach(slot => {
                    const selected = slot.id == selectedSlotId ? 'selected' : '';
                    slotSelect.append(`<option value="${slot.id}" ${selected}>${slot.reportingTime} (${slot.daysOfWeek})</option>`);
                });

                mainSlotSelect.empty().append('<option value="">Select Slot</option>');
                res.data.forEach(slot => {
                    const selected = slot.id == selectedSlotId ? 'selected' : '';
                    mainSlotSelect.append(`<option value="${slot.id}" ${selected}>${slot.reportingTime} (${slot.daysOfWeek})</option>`);
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
            if (res.success) {
                const deptSelect = $('#editDepartmentId');
                const mainDeptSelect = $('#DepartmentId');


                deptSelect.empty().append('<option value="">Select Department</option>');
                res.data.forEach(dep => {
                    const selected = dep.id == selectedDeptId ? 'selected' : '';
                    deptSelect.append(`<option value="${dep.id}" ${selected}>${dep.name}</option>`);
                });

                mainDeptSelect.empty().append('<option value="">Select Department</option>');
                res.data.forEach(dep => {
                    const selected = dep.id == selectedDeptId ? 'selected' : '';
                    mainDeptSelect.append(`<option value="${dep.id}" ${selected}>${dep.name}</option>`);
                });
            }
        },
        error: function () {
            console.error('Failed to load departments');
        }
    });
    }

    // Show modal on Add button click
    $('#btnAddDoctor').click(function () {
        $('#addDoctorForm')[0].reset();      // Reset form
        $('#doctorFormErrors').addClass('d-none').text(''); // Clear previous errors
        addDoctorModal.show();

        // Populate dropdowns
        loadSlotsAndDepartments();
    });


    // Handle Save button click
    $('#saveDoctorBtn').click(function () {
        var formData = $('#addDoctorForm').serialize(); // Serialize form data

        $.ajax({
            url: '/Doctor/AddDoctor', // Controller method
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    addDoctorModal.hide();   // Hide modal
                    loadDoctors();
                    alert('Doctor added successfully!');
                } else {
                    // Show errors in modal
                    $('#doctorFormErrors').removeClass('d-none').text(response.message || 'An error occurred.');
                }
            },
            error: function (xhr, status, error) {
                $('#doctorFormErrors').removeClass('d-none').text('Server error: ' + error);
            }
        });
    });

    $('#editDoctorBtn').on('click', function () {
        const dto = {
            id: $('#editDoctorId').val(),
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


        // Clear previous errors
        $('#editDoctorErrors').html('').addClass('d-none');

        $.ajax({
            url: '/Doctor/EditDoctor',
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (response) {
                if (response.success) {
                    // Close modal
                    const modalEl = document.getElementById('editDoctorModal');
                    const modal = bootstrap.Modal.getInstance(modalEl);
                    modal.hide();

                    // Optionally, refresh the grid or update row directly
                    loadDoctors(); // reload data from server

                    // Optionally, show success toast
                    showMessage(response.message, 'success');
                } else {
                    // Show server-returned error
                    $('#editDoctorErrors').html(response.message).removeClass('d-none');
                }
            },
            error: function (xhr) {
                let errorMsg = "An error occurred while updating the doctor.";

                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMsg = xhr.responseJSON.message;
                }

                $('#editDoctorErrors').html(errorMsg).removeClass('d-none');
                console.error("AJAX Error:", xhr);
            }
        });
    });

});
