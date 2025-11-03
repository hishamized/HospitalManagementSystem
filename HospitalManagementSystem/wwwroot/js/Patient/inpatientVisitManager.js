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

});
