$(document).ready(function () {
    const patientId = $('#PatientId').val();
    if (!patientId) {
        console.error("No Patient ID found in page.");
        return;
    }

    // Column definitions
    const columnDefs = [
        { headerName: "Bill ID", field: "id", width: 100, pinned: "left" },
        { headerName: "Bill Date", field: "billDate", width: 130 },
        { headerName: "Visit Type", field: "visitType", width: 130 },
        { headerName: "Total Amount", field: "totalAmount", width: 140 },
        { headerName: "Discount", field: "discountAmount", width: 120 },
        { headerName: "Net Amount", field: "netAmount", width: 140 },
        { headerName: "Payment Status", field: "paymentStatus", width: 140 },
        { headerName: "Payment Mode", field: "paymentMode", width: 130 },
        { headerName: "Room Charges", field: "roomCharges", width: 140 },
        { headerName: "Procedure Charges", field: "procedureCharges", width: 160 },
        { headerName: "Medication Charges", field: "medicationCharges", width: 160 },
        { headerName: "Consultation Charges", field: "consultationCharges", width: 160 },
        { headerName: "OPD Fee", field: "opdConsultationFee", width: 130 },
        { headerName: "Notes", field: "notes", width: 200 },
        { headerName: "Created At", field: "createdAt", width: 160 },
        {
            headerName: "Actions",
            field: "actions",
            pinned: "right",
            width: 150,
            cellRenderer: params => {
                return `
                    <button class="btn btn-sm btn-warning me-1 edit-bill" data-id="${params.data.id}">
                        <i class="bi bi-pencil"></i> 
                    </button>
                    <button class="btn btn-sm btn-danger delete-bill" data-id="${params.data.id}">
                        <i class="bi bi-trash"></i>
                    </button>
                `;
            }
        }
    ];

    // Grid options
    const gridOptions = {
        columnDefs,
        defaultColDef: {
            resizable: true,
            sortable: true,
            filter: true,
        },
        rowHeight: 50, 
        rowData: [],
        domLayout: "autoHeight",
        suppressHorizontalScroll: false,
        animateRows: true,
        onGridReady: function () {
            loadBillsGrid();
        }
    };

    // Initialize grid
    const gridDiv = document.querySelector('#billsGrid');
    new agGrid.Grid(gridDiv, gridOptions);

    // Fetch bills
    function loadBillsGrid() {
        $.ajax({
            url: `/Bill/GetBillsByPatientId?patientId=${patientId}`,
            type: 'GET',
            beforeSend: function () {
                $('#btnReloadBills').prop('disabled', true).html('<i class="bi bi-arrow-clockwise spin"></i> Loading...');
            },
            success: function (response) {
                if (response.success) {
                    gridOptions.api.setRowData(response.data);
                } else {
                    alert(response.message || "Failed to fetch bills.");
                }
            },
            error: function (xhr) {
                console.error(xhr);
                alert("Error fetching bills: " + (xhr.responseJSON?.message || xhr.statusText));
            },
            complete: function () {
                $('#btnReloadBills').prop('disabled', false).html('<i class="bi bi-arrow-clockwise"></i> Refresh');
            }
        });
    }

    // Refresh grid manually
    $('#btnReloadBills').on('click', loadBillsGrid);

    // Handle Delete button
    $(document).on('click', '.delete-bill', function () {
        const id = $(this).data('id');
        if (confirm("Are you sure you want to delete this bill?")) {
            $.ajax({
                url: `/Bill/DeleteBill/${id}`,
                type: 'DELETE',
                success: function (response) {
                    if (response.success) {
                        alert("Bill deleted successfully!");
                        loadBillsGrid();
                    } else {
                        alert(response.message || "Failed to delete bill.");
                    }
                },
                error: function (xhr) {
                    alert("Error deleting bill: " + (xhr.responseJSON?.message || xhr.statusText));
                }
            });
        }
    });

    // Open the modal
    $('#btnAddBill').on('click', function () {
        $('#addBillModal').modal('show');
    });

    // --- Dynamic total calculation ---
    function calculateBillTotals() {
        const room = parseFloat($('#RoomCharges').val()) || 0;
        const procedure = parseFloat($('#ProcedureCharges').val()) || 0;
        const medication = parseFloat($('#MedicationCharges').val()) || 0;
        const consultation = parseFloat($('#ConsultationCharges').val()) || 0;
        const opd = parseFloat($('#OpdConsultationFee').val()) || 0;
        const discount = parseFloat($('#DiscountAmount').val()) || 0;

        // Calculate totals
        const total = room + procedure + medication + consultation + opd;
        const net = total - discount;

        // Update fields
        $('#TotalAmount').val(total.toFixed(2));
        $('#NetAmount').val(net >= 0 ? net.toFixed(2) : '0.00');
    }

    // Listen for changes in all relevant fields
    $('#RoomCharges, #ProcedureCharges, #MedicationCharges, #ConsultationCharges, #OpdConsultationFee, #DiscountAmount')
        .on('input', calculateBillTotals);
    // Handle Save button click
    $('#btnSaveBill').on('click', function () {
        const billData = {
            patientId: parseInt($('#PatientId').val()),
            visitId: parseInt($('#VisitId').val()),
            visitType: $('#VisitType').val(),
            billDate: $('#BillDate').val(),
            totalAmount: parseFloat($('#TotalAmount').val()) || 0,
            discountAmount: parseFloat($('#DiscountAmount').val()) || 0,
            netAmount: parseFloat($('#NetAmount').val()) || 0,
            paymentStatus: $('#PaymentStatus').val(),
            paymentMode: $('#PaymentMode').val(),
            roomCharges: parseFloat($('#RoomCharges').val()) || null,
            procedureCharges: parseFloat($('#ProcedureCharges').val()) || null,
            medicationCharges: parseFloat($('#MedicationCharges').val()) || null,
            consultationCharges: parseFloat($('#ConsultationCharges').val()) || null,
            opdConsultationFee: parseFloat($('#OpdConsultationFee').val()) || null,
            notes: $('#Notes').val()
        };

        // Basic validation
        if (!billData.totalAmount || billData.totalAmount <= 0) {
            alert("Please enter a valid total amount.");
            return;
        }
        var token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: '/Bill/AddBill',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(billData),
            headers: {
                'RequestVerificationToken': token // important!
            },
            beforeSend: function () {
                $('#btnSaveBill').prop('disabled', true).text('Saving...');
            },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#addBillModal').modal('hide');
                    $('#addBillForm')[0].reset();
                    loadBillsGrid(); // Optional: refresh AG Grid
                } else {
                    alert(response.message || "Failed to add bill.");
                }
            },
            error: function (xhr) {
                alert("Error: " + (xhr.responseJSON?.message || "Server error."));
                console.log(xhr.responseJSON?.message);
            },
            complete: function () {
                $('#btnSaveBill').prop('disabled', false).text('Save Bill');
            }
        });
    });

 
    // ======== EDIT BILL FUNCTIONALITY ========

    // When "Edit" button is clicked
    $(document).on('click', '.edit-bill', function () {
        const billId = $(this).data('id');

        // ✅ Get row node directly (assuming you set unique id as `data.id`)
        const rowNode = gridOptions.api.getRowNode(billId);

        // If you didn't define getRowId in gridOptions, do this instead:
        let selectedRow = null;
        gridOptions.api.forEachNode(node => {
            if (node.data.id == billId) selectedRow = node.data;
        });

        if (!selectedRow) {
            alert("Error: Could not find bill data for editing.");
            return;
        }

        // ✅ Prefill edit modal with selectedRow data
        $('#EditBillId').val(selectedRow.id);
        $('#EditPatientId').val(selectedRow.patientId);
        $('#EditVisitId').val(selectedRow.visitId);
        $('#EditBillDate').val(selectedRow.billDate?.split('T')[0] || '');
        $('#EditPaymentStatus').val(selectedRow.paymentStatus);
        $('#EditPaymentMode').val(selectedRow.paymentMode);
        $('#EditRoomCharges').val(selectedRow.roomCharges);
        $('#EditProcedureCharges').val(selectedRow.procedureCharges);
        $('#EditMedicationCharges').val(selectedRow.medicationCharges);
        $('#EditConsultationCharges').val(selectedRow.consultationCharges);
        $('#EditOpdConsultationFee').val(selectedRow.opdConsultationFee);
        $('#EditDiscountAmount').val(selectedRow.discountAmount);
        $('#EditTotalAmount').val(selectedRow.totalAmount);
        $('#EditNetAmount').val(selectedRow.netAmount);
        $('#EditNotes').val(selectedRow.notes);

        // ✅ Show modal
        $('#editBillModal').modal('show');
    });

    // Dynamic total calculation (Edit form)
    function calculateEditBillTotals() {
        const room = parseFloat($('#EditRoomCharges').val()) || 0;
        const procedure = parseFloat($('#EditProcedureCharges').val()) || 0;
        const medication = parseFloat($('#EditMedicationCharges').val()) || 0;
        const consultation = parseFloat($('#EditConsultationCharges').val()) || 0;
        const opd = parseFloat($('#EditOpdConsultationFee').val()) || 0;
        const discount = parseFloat($('#EditDiscountAmount').val()) || 0;

        const total = room + procedure + medication + consultation + opd;
        const net = total - discount;

        $('#EditTotalAmount').val(total.toFixed(2));
        $('#EditNetAmount').val(net >= 0 ? net.toFixed(2) : '0.00');
    }

    $('#EditRoomCharges, #EditProcedureCharges, #EditMedicationCharges, #EditConsultationCharges, #EditOpdConsultationFee, #EditDiscountAmount')
        .on('input', calculateEditBillTotals);

    // Handle Update Bill AJAX
    $('#btnUpdateBill').on('click', function () {
        const billData = {
            id: parseInt($('#EditBillId').val()),
            patientId: parseInt($('#EditPatientId').val()),
            visitId: parseInt($('#EditVisitId').val()),
            billDate: $('#EditBillDate').val(),
            paymentStatus: $('#EditPaymentStatus').val(),
            paymentMode: $('#EditPaymentMode').val(),
            roomCharges: parseFloat($('#EditRoomCharges').val()) || 0,
            procedureCharges: parseFloat($('#EditProcedureCharges').val()) || 0,
            medicationCharges: parseFloat($('#EditMedicationCharges').val()) || 0,
            consultationCharges: parseFloat($('#EditConsultationCharges').val()) || 0,
            opdConsultationFee: parseFloat($('#EditOpdConsultationFee').val()) || 0,
            discountAmount: parseFloat($('#EditDiscountAmount').val()) || 0,
            totalAmount: parseFloat($('#EditTotalAmount').val()) || 0,
            netAmount: parseFloat($('#EditNetAmount').val()) || 0,
            notes: $('#EditNotes').val()
        };

        var token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            url: '/Bill/EditBill',
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(billData),
            headers: {
                'RequestVerificationToken': token
            },
            beforeSend: function () {
                $('#btnUpdateBill').prop('disabled', true).text('Updating...');
            },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#editBillModal').modal('hide');
                    loadBillsGrid();
                } else {
                    alert(response.message || "Failed to update bill.");
                }
            },
            error: function (xhr) {
                alert("Error updating bill: " + (xhr.responseJSON?.message || xhr.statusText));
            },
            complete: function () {
                $('#btnUpdateBill').prop('disabled', false).text('Update Bill');
            }
        });
    });

    // Show modal
    $("#btnAllotBed").on("click", function () {
        $("#allotBedModal").modal("show");
        loadWards();
    });

    // Load wards
    function loadWards() {
        $.ajax({
            url: '/Ward/GetAll',
            type: 'GET',
            success: function (wards) {
                var group = wards.data;
                var wardSelect = $("#wardSelect");
                wardSelect.empty().append('<option value="">-- Select Ward --</option>');

                $.each(group, function (i, ward) {
                    wardSelect.append(`<option value="${ward.id}">${ward.wardName} (${ward.wardType})</option>`);
                });
            },
            error: function () {
                alert("Failed to load wards.");
            }
        });
    }

    // When ward selected

    $("#wardSelect").on("change", function () {
        var wardId = $(this).val();
        var bedSelect = $("#bedSelect");

        if (wardId) {
            bedSelect.prop("disabled", false);
            bedSelect.empty().append('<option value="">-- Select Bed --</option>');

            // Fetch beds for selected ward
            $.ajax({
                url: '/Bed/GetByWardId?wardId=' + wardId,
                type: 'GET',
                success: function (data) {
                    if (data && data.length > 0) {
                        $.each(data, function (i, bed) {
                            bedSelect.append(`<option value="${bed.id}">
                            ${bed.bedNumber} : ${bed.id} (${bed.bedType} - ${bed.status})
                        </option>`);
                        });
                    } else {
                        bedSelect.append('<option value="">No available beds</option>');
                    }
                },
                error: function (xhr) {
                    console.error(xhr.responseText);
                    alert("Failed to load beds for the selected ward.");
                }
            });
        } else {
            bedSelect.prop("disabled", true);
            bedSelect.empty().append('<option value="">-- Select Bed --</option>');
        }
    });
    $('#allotBedForm').on('submit', function (e) {
        e.preventDefault();

        const dto = {
            patientId: $('#PatientId').val(),
            wardId: $('#wardSelect').val(),
            bedId: $('#bedSelect').val(),
            notes: $('#notes').val(),
            assignedAt: new Date().toISOString()
        };

        $.ajax({
            url: '/Bed/AllotBed',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (response) {
                $('#allotBedModal').modal('hide');
                alert(response);
                $('#inpatientVisitGrid').agGridGridOptions.api.refreshServerSideStore(); // if using server-side grid
            },
            error: function (xhr) {
                let message = 'An error occurred.';
                if (xhr.responseText) message = xhr.responseText;
                else if (xhr.responseJSON) message = xhr.responseJSON;
                $('#errorMessage').remove();
                $('#allotBedModal .modal-body').prepend(
                    `<div id="errorMessage" class="alert alert-danger mb-2">${message}</div>`
                );
            }
        });
    });
    $(document).on('click', '.check-bed-btn', function () {
        const patientId = $(this).data('patient-id');

        $.ajax({
            url: `/Bed/CheckBed`,
            type: 'GET',
            data: { patientId: patientId },
            beforeSend: function () {
                // You could show a loader or disable the button temporarily
            },
            success: function (response) {
                if (response.success) {
                    const bedData = response.data;
                    console.log(bedData);
                    if (bedData.bedCode == null || bedData.wardCode == null) {
                        alert(response.message || 'No bed data available.');
                        return;
                    } else {
                        $('#removeWardBed')
                            .attr('data-has-bed', 1)
                            .attr('data-patient-bed-ward-id', bedData.patientBedWardId);

                        checkRemoveButton();
                    }
                    // Trigger modal/pop-up logic (will handle separately)
                    // Example: populate modal and show
                    showBedPopup(bedData);
                } else {
                    alert("There was an error fetching data " + response);
                }
            },
            error: function (xhr) {
                alert('An unexpected error occurred while checking bed status.');
                console.error(xhr.responseText);
            }
        });
    });
    function showBedPopup(bedData) {
        $('#wardName').text(bedData.wardName || 'N/A');
        $('#wardCapacity').text(bedData.wardCapacity || 0);
        $('#wardOccupied').text(bedData.occupiedBeds || 0);

        const totalBeds = bedData.wardCapacity || 0;
        const occupiedBeds = bedData.occupiedBeds || 0;
        const patientBedCode = bedData.bedCode;
        const bedContainer = $('#bedGridContainer');
        bedContainer.empty();

        for (let i = 1; i <= totalBeds; i++) {
            let bedStatusClass = 'bg-available';
            let bedLabel = `Bed ${i}`;

            if (bedData.hasBedAllotted && i === bedData.bedPosition) {
                bedStatusClass = 'bg-patient';
                bedLabel = `${bedData.bedCode || 'Bed'} (${bedData.bedNumber || i})`;
            } else if (i <= occupiedBeds) {
                bedStatusClass = 'bg-occupied';
            }

            const bedDiv = $(`<div class="bed-box ${bedStatusClass}">${bedLabel}</div>`);
            bedContainer.append(bedDiv);
        }

        const modal = new bootstrap.Modal(document.getElementById('checkBedModal'));
        modal.show();
    }
    $('#removeWardBed').on('click', function () {
        const patientBedWardId = $(this).attr('data-patient-bed-ward-id');

        if (!patientBedWardId) {
            alert('No bed record found for this patient.');
            return;
        }

        $.ajax({
            url: '/Bed/RemovePatientFromBed',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ patientBedWardId }),
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#removeWardBed').data('hasBed', 0).hide();
                } else {
                    alert(response.message || 'Failed to remove bed assignment.');
                }
            },
            error: function (xhr) {
                alert('An error occurred while removing bed: ' + xhr.responseText);
            }
        });
    });

    function checkRemoveButton() {
        const btn = $('#removeWardBed');
        const hasBed = btn.attr('data-has-bed');

        if (hasBed == 0 || hasBed === false) {
            btn.hide();
        } else {
            btn.show();
        }
    }
    checkRemoveButton();
    // Show modal on button click

    // Show modal and populate doctors
    $('#btnAddDoctorRound').on('click', function () {
        const patientId = $('#PatientId').val(); // get hidden patientId
        if (!patientId) {
            alert('Patient ID not found!');
            return;
        }
        // Fetch doctors via AJAX
        $.ajax({
            url: `/Doctor/GetDoctorsByPatient`,
            type: 'GET',
            data: { patientId: patientId },
            success: function (data) {
                console.log(data)
                const $doctorSelect = $('#DoctorId');
                $doctorSelect.empty();
                $doctorSelect.append('<option value="">Select Doctor</option>');

                $.each(data, function (i, doctor) {
                    $doctorSelect.append(`<option value="${doctor.id}">${doctor.fullName} (${doctor.specialization})</option>`);
                });

                // Show modal after dropdown is populated
                $('#doctorRoundModal').modal('show');
            },
            error: function (xhr, status, error) {
                console.error(error);
                alert('Failed to fetch doctors. Please try again.');
            }
        });
    });

    // Close modal
    $('#btnCloseDoctorRound').on('click', function () {
        $('#doctorRoundModal').modal('hide');
    });

    // Optional: clear form on close
    $('#doctorRoundModal').on('hidden.bs.modal', function () {
        $('#doctorRoundForm')[0].reset();
        $('#DoctorId').empty().append('<option value="">Select Doctor</option>');
    });
    // Close modal on close button click
    $('#btnCloseDoctorRound').on('click', function () {
        $('#doctorRoundModal').modal('hide');
    });

    // Optional: Clear form when modal is closed
    $('#doctorRoundModal').on('hidden.bs.modal', function () {
        $('#doctorRoundForm')[0].reset();
    });

    // Form submission
    $('#doctorRoundForm').submit(function (e) {
        e.preventDefault();
        $('#doctorRoundForm').prop('disabled', true);

        // Remove previous errors
        $('#doctorRoundForm .is-invalid').removeClass('is-invalid');
        $('#doctorRoundForm .invalid-feedback').remove();

        var form = $(this);
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        $.ajax({
            type: 'POST',
            url: '/Doctor/AddDoctorRound',
            data: form.serialize(),
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    $('#doctorRoundForm').prop('disabled', false);
                    alert(response.message); // replace with toast or nicer UI if you like
                    $('#doctorRoundModal').modal('hide');
                    $('#doctorRoundForm')[0].reset();
                    // Optionally refresh doctor rounds table here
                } else {
                    alert('Error: ' + response.message);
                }
            },
            error: function (xhr) {
                $('#doctorRoundForm').prop('disabled', false);
                if (xhr.status === 400) {
                    // Display model state errors
                    var errors = xhr.responseJSON;
                    $.each(errors, function (key, value) {
                        var input = $('[name="' + key + '"]');
                        input.addClass('is-invalid');
                        input.after('<div class="invalid-feedback">' + value[0] + '</div>');
                    });
                } else {
                    alert('An unexpected error occurred.');
                }
            }
        });
    });
    // Column definitions for Doctor Rounds
    var roundColumnDefs = [
        { headerName: "Doctor", field: "doctorName", sortable: true, filter: true },
        { headerName: "Ward", field: "wardName", sortable: true, filter: true },
        { headerName: "Round Date", field: "roundDate", sortable: true, filter: true },
        { headerName: "Observations", field: "observations" },
        { headerName: "Diagnosis", field: "diagnosis" },
        { headerName: "Prescriptions", field: "prescriptions" },
        { headerName: "Tests Recommended", field: "testsRecommended" },
        { headerName: "Treatment Plan", field: "treatmentPlan" },
        { headerName: "Follow-up Instructions", field: "followUpInstructions" },
        { headerName: "Critical", field: "isCritical", cellRenderer: params => params.value ? "Yes" : "No" },
        {
            headerName: "Actions",
            cellRenderer: function (params) {
                const id = params.data?.id ?? 0; // safe fallback
                return `
            <button class="btn btn-sm btn-primary me-1 btn-edit-round" 
                data-id="${id}" data-row-index="${params.rowIndex}">Edit</button>
            <button class="btn btn-sm btn-danger btn-delete-round" data-id="${id}">Delete</button>
        `;
            }
        }




    ];


    var roundGridOptions = {
        columnDefs: roundColumnDefs,
        defaultColDef: { resizable: true },
        rowData: [],
        animateRows: true,
        pagination: true,
        paginationPageSize: 10
    };

    // Initialize AG Grid
    var eRoundGridDiv = document.querySelector('#doctorRoundHistoryGrid');
    new agGrid.Grid(eRoundGridDiv, roundGridOptions);

    // Fetch and display rounds when button clicked
    $('#btnViewRoundHistory').click(function () {
        var patientId = $('#PatientId').val();
        if (!patientId) {
            alert("Patient ID not found.");
            return;
        }

        $.ajax({
            url: '/Doctor/GetDoctorRoundsByPatient',
            type: 'GET',
            data: { patientId: patientId },
            success: function (response) {
                console.log(response)
                if (response.success) {
                    roundGridOptions.api.setRowData(response.data);
                    $('#doctorRoundHistoryModal').modal('show');
                } else {
                    alert(response.message || "Failed to fetch round history.");
                }
            },
            error: function (xhr) {
                var errMsg = xhr.responseJSON?.message || "An error occurred while fetching round history.";
                alert(errMsg);
            }
        });
    });

    // Clear grid data when modal closes
    $('#doctorRoundHistoryModal').on('hidden.bs.modal', function () {
        roundGridOptions.api.setRowData([]);
    });


    // Open Edit Round Modal on button click
    $(document).on('click', '.btn-edit-round', function () {
        const rowIndex = $(this).data('row-index');
        const rowNode = roundGridOptions.api.getDisplayedRowAtIndex(rowIndex);

        if (!rowNode || !rowNode.data) {
            alert("Row data not found!");
            return;
        }

        const rowData = rowNode.data;

        // Prefill form
        $('#EditRoundId').val(rowData.id);
        $('#EditDoctorName').val(rowData.doctorName);
        $('#EditWardName').val(rowData.wardName);
        $('#EditRoundDate').val(rowData.roundDate ? new Date(rowData.roundDate).toISOString().slice(0, 16) : '');
        $('#EditObservations').val(rowData.observations);
        $('#EditDiagnosis').val(rowData.diagnosis);
        $('#EditPrescriptions').val(rowData.prescriptions);
        $('#EditTestsRecommended').val(rowData.testsRecommended);
        $('#EditTreatmentPlan').val(rowData.treatmentPlan);
        $('#EditFollowUpInstructions').val(rowData.followUpInstructions);
        $('#EditIsCritical').val(rowData.isCritical ? "true" : "false");

        // Close previous modal and open edit modal
        $('#doctorRoundHistoryModal').modal('hide');
        $('#editDoctorRoundModal').modal('show');
    });


    // Close Edit Modal
    $('#btnCloseEditDoctorRound').on('click', function () {
        $('#editDoctorRoundModal').modal('hide');
    });

    // Submit Edit Doctor Round Form
    $('#editDoctorRoundForm').submit(function (e) {
        e.preventDefault();

        var form = $(this);
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        $.ajax({
            url: '/Doctor/UpdateDoctorRound', // your backend endpoint
            type: 'POST',
            data: form.serialize(),
            headers: { 'RequestVerificationToken': token },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#editDoctorRoundModal').modal('hide');

                    // Update row in AG Grid without another server call
                    let rowNode = gridOptions.api.getRowNode(response.data.id);
                    if (rowNode) {
                        rowNode.setData(response.data);
                    }
                } else {
                    alert(response.message || "Failed to update round.");
                }
            },
            error: function (xhr) {
                alert(xhr.responseJSON?.message || "Server error occurred.");
            }
        });
    });

    // Handle delete button click
    $(document).on('click', '.btn-delete-round', function () {
        const id = $(this).data('id');

        if (!id) {
            alert("Invalid record selected for deletion.");
            return;
        }

        if (!confirm("Are you sure you want to delete this entry?")) {
            return;
        }

        $.ajax({
            url: `/Doctor/DeleteDoctorRound/${id}`, // Controller + Action
            type: 'DELETE',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success) {
                    alert(response.message || "Entry deleted successfully!");

                    // Refresh AG Grid without reloading the whole page
                    if (typeof roundGridOptions !== 'undefined' && roundGridOptions.api) {
                        roundGridOptions.api.applyTransaction({ remove: [roundGridOptions.api.getRowNode(id).data] });
                    }
                } else {
                    alert(response.message || "Failed to delete entry.");
                }
            },
            error: function (xhr) {
                let errorMsg = "An unexpected error occurred.";
                if (xhr.responseJSON?.message) {
                    errorMsg = xhr.responseJSON.message;
                } else if (xhr.responseText) {
                    errorMsg = xhr.responseText;
                }
                alert(errorMsg);
            }
        });
    });

    $('#dischargePatientBtn').on('click', function () {
        const VisitId = $(this).data('visitid');
        $.ajax({
            url: '/PatientVisit/DischargePatient',
            method: 'GET',
            data: { VisitId: VisitId },
            success: function (response) {
                if (response.Success == true) {
                    alert(response.Message); 
                    $('#dischargePatientBtn').hide();
                }
            },
            error: function (xhr) {
                let errorMessage = xhr.response;
                console.log(errorMessage);
            }
        });
    });

    $('#GenerateFinalBillBtn').on('click', function () {
        const VisitId = $(this).data('visitid');
        const PatientId = $(this).data('patientid');

        $.ajax({
            url: '/Bill/GetFinalBill',
            method: 'GET',
            data: {
                patientId: PatientId,
                visitId: VisitId
            },
            beforeSend: function () {
                $('#GenerateFinalBillBtn').prop('disabled', true).html('<i class="bi bi-hourglass-split"></i> Loading...');
            },
            success: function (response) {
                if (response.success && response.data) {
                    console.log(response.data);
                    populateFinalBillModal(response.data);
                    $('#finalBillModal').modal('show');
                } else {
                    alert(response.message || "Failed to generate bill.");
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', xhr.responseJSON);
                alert("Server Error: " + (xhr.responseJSON?.message || error));
            },
            complete: function () {
                $('#GenerateFinalBillBtn').prop('disabled', false).html('<i class="bi bi-receipt"></i> Generate Final Bill');
            }
        });
    });

    function populateFinalBillModal(data) {
        // Charges
        $('#billRoomCharges').text((data.roomChargesFinal || 0).toFixed(2));
        $('#billConsultationCharges').text((data.consultationChargesFinal || 0).toFixed(2));
        $('#billProcedureCharges').text((data.procedureChargesFinal || 0).toFixed(2));
        $('#billMedicationCharges').text((data.medicationChargesFinal || 0).toFixed(2));
        $('#billOpdConsultationFee').text((data.opdConsultationFeeFinal || 0).toFixed(2));

        const subtotal = (data.roomCharges || 0) + (data.consultationChargesFinal || 0) +
            (data.procedureChargesFinal || 0) + (data.medicationChargesFinal || 0) +
            (data.opdConsultationFeeFinal || 0);
        $('#billSubtotal').text(subtotal.toFixed(2));
        $('#billDiscountAmount').text((data.discountAmountFinal || 0).toFixed(2));
        $('#billNetAmount').text((data.netAmountFinal || 0).toFixed(2));

        // Payment Info
        const paymentStatusBadge = $('#billPaymentStatus');
        paymentStatusBadge.text(data.paymentStatus || '-');

        // Color code payment status
        paymentStatusBadge.removeClass('bg-success bg-warning bg-danger');
        if (data.paymentStatus === 'Paid') {
            paymentStatusBadge.addClass('bg-success');
        } else if (data.paymentStatus === 'Pending') {
            paymentStatusBadge.addClass('bg-warning');
        } else {
            paymentStatusBadge.addClass('bg-danger');
        }

        $('#billPaymentMode').text(data.paymentMode || '-');

        // Notes
        if (data.notes && data.notes.trim() !== '') {
            $('#billNotes').text(data.notes);
            $('#billNotesSection').show();
        } else {
            $('#billNotesSection').hide();
        }

        // Generated Date
        $('#billGeneratedDate').text(new Date().toLocaleString());

        // Store data for PDF generation
        $('#finalBillModal').data('billData', data);
    }

    // Download PDF Button
    $('#btnDownloadBill').on('click', function () {
        const billData = $('#finalBillModal').data('billData');

        if (!billData) {
            alert('Bill data not available. Please regenerate the bill.');
            return;
        }

        // Using html2pdf library (you'll need to include this in your layout)
        const element = document.getElementById('billContent');
        const opt = {
            margin: 10,
            filename: `Bill_${billData.patientName}_${billData.visitId}.pdf`,
            image: { type: 'jpeg', quality: 0.98 },
            html2canvas: { scale: 2 },
            jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
        };

        html2pdf().set(opt).from(element).save();
    });

    $('#dischargeSummaryBtn').on('click', function () {
        const visitId = $(this).data('visitid');
        $.ajax({
            url: '/Patient/GenerateDischargeSummary',
            method: 'GET',
            data: { VisitId: visitId },
            success: function (response) {
                if (response.success) {
                    const data = response.data;
                    populateDischargeModal(data);
                    const modal = new bootstrap.Modal(document.getElementById('dischargeSummaryModal'));
                    modal.show();
                } else {
                    alert("No discharge summary found.");
                }
            },
            error: function (xhr) {
                console.error(xhr.responseText);
                alert("Error loading discharge summary.");
            }
        });
    });

    function populateDischargeModal(data) {
        let roundsHtml = '';
        if (data.doctorRounds && data.doctorRounds.length > 0) {
            roundsHtml = data.doctorRounds.map(r => `
                <tr>
                    <td>${r.roundDate ? new Date(r.roundDate).toLocaleString() : ''}</td>
                    <td>${r.doctorName || ''}</td>
                    <td>${r.diagnosis || ''}</td>
                    <td>${r.prescriptions || ''}</td>
                    <td>${r.treatmentPlan || ''}</td>
                    <td>${r.isCritical ? 'Yes' : 'No'}</td>
                </tr>
            `).join('');
        } else {
            roundsHtml = `<tr><td colspan="6" class="text-center text-muted">No doctor rounds recorded.</td></tr>`;
        }

        const html = `
            <div id="summaryToPrint">
                <h4 class="mb-3">${data.fullName} (${data.patientCode})</h4>
                <p><strong>Gender:</strong> ${data.gender} | <strong>DOB:</strong> ${new Date(data.dateOfBirth).toLocaleDateString()}</p>
                <p><strong>Contact:</strong> ${data.contactNumber} | <strong>Address:</strong> ${data.address || '-'}</p>

                <hr/>
                <h5>Visit Details</h5>
                <p><strong>Visit ID:</strong> ${data.visitId} | <strong>Visit Type:</strong> ${data.visitType}</p>
                <p><strong>Admission:</strong> ${data.admissionDate ? new Date(data.admissionDate).toLocaleDateString() : '-'} |
                   <strong>Discharge:</strong> ${data.dischargeDate ? new Date(data.dischargeDate).toLocaleDateString() : '-'}</p>
                <p><strong>Room:</strong> ${data.roomNumber || '-'} | <strong>Ward:</strong> ${data.wardName || '-'} | <strong>Bed:</strong> ${data.bedNumber || '-'}</p>

                <hr/>
                <h5>Doctor Details</h5>
                <p><strong>Doctor:</strong> ${data.doctorName || '-'} (ID: ${data.doctorId || '-'})</p>
                <p><strong>Treatment Summary:</strong> ${data.treatmentDetails || '-'}</p>
                <p><strong>Notes:</strong> ${data.notes || '-'}</p>

                <hr/>
                <h5>Doctor Rounds</h5>
                <table class="table table-bordered table-striped">
                    <thead class="table-dark">
                        <tr>
                            <th>Date</th>
                            <th>Doctor</th>
                            <th>Diagnosis</th>
                            <th>Prescriptions</th>
                            <th>Treatment Plan</th>
                            <th>Critical</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${roundsHtml}
                    </tbody>
                </table>
            </div>
        `;

        $('#dischargeSummaryContent').html(html);
    }

    $('#downloadPdfBtn').on('click', function () {
        const element = document.getElementById('summaryToPrint');
        const opt = {
            margin: 0.5,
            filename: 'DischargeSummary.pdf',
            image: { type: 'jpeg', quality: 0.98 },
            html2canvas: { scale: 2 },
            jsPDF: { unit: 'in', format: 'a4', orientation: 'portrait' }
        };
        html2pdf().set(opt).from(element).save();
    });
});

