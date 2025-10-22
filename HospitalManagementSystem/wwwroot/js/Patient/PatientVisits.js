$(document).ready(function () {
    // 1️⃣ Column Definitions for AG Grid
    var columnDefs = [
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
            width: 150,
            cellRenderer: function (params) {
                return `
                <button class="btn btn-sm btn-warning edit-btn" data-id="${params.value}">Edit</button>

                <button class="btn btn-sm btn-danger delete-btn" data-id="${params.value}">Delete</button>
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
        console.log('Delete visit:', visitId);
        // Open delete confirmation modal
        $('#deleteVisitModal').modal('show');
    });

    // 1️⃣ Show modal on button click
    $("#btnAddVisit").on("click", function () {
        $("#visitModalLabel").text("Add Patient Visit");
        $("#visitForm")[0].reset();
        $(".inpatient-field").hide();
        $("#visitModal").modal("show");
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

        $.ajax({
            url: '/PatientVisit/Add',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.success) {
                    console.log('Patient visit added successfully:', res.visitId);
                    $('#visitModal').modal('hide');
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
        $('#deleteVisitModal').modal('show');
    });


    $('#confirmDeleteVisitBtn').on('click', function () {
        if (!selectedVisitId) return;

        $.ajax({
            url: '/PatientVisit/Delete',
            type: 'POST',
            data: { id: selectedVisitId },
            success: function (res) {
                if (res.success) {
                    console.log('Patient visit deleted successfully:', selectedVisitId);
                    // Remove from AG Grid
                    var rowNode = gridOptions.api.getRowNode(selectedVisitId.toString());
                    if (rowNode) gridOptions.api.applyTransaction({ remove: [rowNode.data] });
                } else {
                    console.warn('Delete failed:', res.message);
                }
                $('#deleteVisitModal').modal('hide');
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
        console.log(visitData);
        gridOptions.api.forEachNode(node => {
            if (node.data.id == visitId) visitData = node.data;
        });
        if (!visitData) return;

        // Populate modal fields
        $('#editVisitId').val(visitData.id);

        // Fix Patient select
        const patientSelect = $('#editPatientId');
        patientSelect.empty(); // remove previous options
        patientSelect.append(`<option value="${visitData.patientId}" selected>${visitData.fullName}</option>`);
        patientSelect.prop('disabled', true); // prevent changing

        // Other fields
        $('#editVisitType').val(visitData.visitType);
        $('#editVisitDate').val(visitData.visitDate.split('T')[0]);
        $('#editDoctorName').val(visitData.doctorName ?? '');
        $('#editAdmissionDate').val(visitData.admissionDate ? visitData.admissionDate.split('T')[0] : '');
        $('#editDischargeDate').val(visitData.dischargeDate ? visitData.dischargeDate.split('T')[0] : '');
        $('#editRoomNumber').val(visitData.roomNumber ?? '');
        $('#editTreatmentDetails').val(visitData.treatmentDetails ?? '');
        $('#editNotes').val(visitData.notes ?? '');

        // Show/hide inpatient fields
        if (visitData.visitType === 'Inpatient') $('.inpatient-field').show();
        else $('.inpatient-field').hide();

        // Show modal
        $('#editVisitModal').modal('show');
    });

    // When update button is clicked
    $('#updatePatientVisitBtn').click(function () {
        const dto = {
            Id: $('#editVisitId').val(),
            PatientId: $('#editPatientId').val(),
            VisitType: $('#editVisitType').val(),
            VisitDate: $('#editVisitDate').val(),
            DoctorId: $('#editDoctorId').val() || null,
            DoctorName: $('#editDoctorName').val() || null,
            AdmissionDate: $('#editAdmissionDate').val() || null,
            DischargeDate: $('#editDischargeDate').val() || null,
            RoomNumber: $('#editRoomNumber').val() || null,
            TreatmentDetails: $('#editTreatmentDetails').val() || null,
            Notes: $('#editNotes').val() || null
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
                    $('#editVisitModal').modal('hide');
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


});