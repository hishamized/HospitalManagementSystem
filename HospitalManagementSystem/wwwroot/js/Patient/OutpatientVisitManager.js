// Show modal
function showModal(modalId) {
    document.getElementById(modalId).classList.remove('hidden');
    document.body.classList.add('overflow-hidden'); // Prevent background scroll
}

// Hide modal
function closeModal(modalId) {
    document.getElementById(modalId).classList.add('hidden');
    document.body.classList.remove('overflow-hidden');
}
$(function () {
    const $container = $("#outpatientContainer");
    const patientId = $container.data("patientId");
    const visitId = $container.data("visitId");

    if (!patientId) {
        console.error("PatientId not found in container dataset");
        return;
    }

    // ------------------------------
    // AG Grid Setup
    // ------------------------------
    const columnDefs = [
        { headerName: "Bill ID", field: "id", width: 100 },
        { headerName: "Patient ID", field: "patientId", width: 110 },
        { headerName: "Visit ID", field: "visitId", width: 100 },
        { headerName: "Visit Type", field: "visitType", width: 120 },
        {
            headerName: "Bill Date",
            field: "billDate",
            width: 130,
            valueFormatter: p => p.value ? new Date(p.value).toLocaleDateString() : ""
        },
        {
            headerName: "Total Amount",
            field: "totalAmount",
            width: 130,
            cellClass: "fw-bold text-success",
            valueFormatter: p => p.value ? parseFloat(p.value).toFixed(2) : "0.00"
        },
        {
            headerName: "Discount",
            field: "discountAmount",
            width: 110,
            valueFormatter: p => p.value ? parseFloat(p.value).toFixed(2) : "-"
        },
        {
            headerName: "Net Amount",
            field: "netAmount",
            width: 130,
            cellClass: "fw-semibold text-primary",
            valueFormatter: p => p.value ? parseFloat(p.value).toFixed(2) : "0.00"
        },
        {
            headerName: "Payment Status",
            field: "paymentStatus",
            width: 140,
            cellRenderer: p => {
                const val = (p.value || "").toLowerCase();
                const cls = val === "paid" ? "bg-success"
                    : val === "unpaid" ? "bg-danger"
                        : val === "partial" ? "bg-warning"
                            : "bg-secondary";
                return `<span class="badge ${cls}">${p.value || ""}</span>`;
            }
        },
        { headerName: "Payment Mode", field: "paymentMode", width: 130 },
        {
            headerName: "OPD Fee",
            field: "opdConsultationFee",
            width: 120,
            valueFormatter: p => p.value ? parseFloat(p.value).toFixed(2) : "-"
        },
        {
            headerName: "Created At",
            field: "createdAt",
            width: 160,
            valueFormatter: p => p.value ? new Date(p.value).toLocaleString() : ""
        },
        {
            headerName: "Active",
            field: "isActive",
            width: 90,
            cellRenderer: p => p.value
                ? '<span class="badge bg-success">Yes</span>'
                : '<span class="badge bg-danger">No</span>'
        },
        {
            headerName: "Notes",
            field: "notes",
            width: 90
        },
        {
            headerName: "Actions",
            width: 180,
            pinned: "right",
            cellRenderer: p => {
                const id = p.data?.id;
                return `
                <div class="flex space-x-2">
                    <button class="flex items-center justify-center px-3 py-1 rounded-md border border-purple-500 text-purple-500 hover:bg-purple-600 hover:text-white transition-colors duration-200"
                            data-id="${id}" data-action="edit" title="Edit">
                        <i class="fa-solid fa-pencil"></i>
                    </button>
                    <button class="flex items-center justify-center px-3 py-1 rounded-md border border-red-500 text-red-500 hover:bg-red-600 hover:text-white transition-colors duration-200"
                            data-id="${id}" data-action="delete" title="Delete">
                        <i class="fa-solid fa-trash"></i>
                    </button>
                </div>
            `;
            }
        }
    ];

    const gridOptions = {
        columnDefs,
        defaultColDef: { sortable: true, filter: true, resizable: true },
        pagination: true,
        paginationPageSize: 10,
        rowSelection: "single",
        getRowNodeId: d => d.id?.toString(),
        // IMPORTANT: Add onGridReady callback
        onGridReady: function (params) {
            console.log('Grid is ready, loading bills...');
            fetchBills();
        }
    };

    const gridDiv = document.querySelector("#outpatientBillGrid");
    new agGrid.Grid(gridDiv, gridOptions);

    // ------------------------------
    // Load Bills
    // ------------------------------
    function fetchBills() {
        $.get(`/Bill/GetBillsByPatientId?patientId=${patientId}`)
            .done(res => {
                console.log('Bills response:', res);
                const data = Array.isArray(res)
                    ? res
                    : res?.data || res?.result || [];

                console.log('Setting row data:', data);
                gridOptions.api.setRowData(data);
            })
            .fail(() => alert("Error loading bills."));
    }

    // Remove the immediate fetchBills() call - it will be called in onGridReady

    // ------------------------------
    // Add Bill Modal
    // ------------------------------
    $("#btnAddBill").on("click", () => {
        $("#patientId").val(patientId);
        $("#visitId").val(visitId);
        showModal('addOutpatientBillModal');
    });

    // Auto calc totals (Add)
    $("#opdConsultationFee, #medicationCharges, #procedureCharges, #discountAmount").on("input", function () {
        const consult = +$("#opdConsultationFee").val() || 0;
        const meds = +$("#medicationCharges").val() || 0;
        const proc = +$("#procedureCharges").val() || 0;
        const discount = +$("#discountAmount").val() || 0;
        const total = consult + meds + proc;
        const net = Math.max(total - discount, 0);
        $("#totalAmount").val(total.toFixed(2));
        $("#netAmount").val(net.toFixed(2));
    });

    $("#saveOutpatientBill").on("click", function () {
        const token = $("#addOutpatientBillForm input[name='__RequestVerificationToken']").val();
        const dto = {
            patientId,
            visitId,
            visitType: $("#visitType").val(),
            billDate: $("#billDate").val(),
            totalAmount: +$("#totalAmount").val() || 0,
            discountAmount: +$("#discountAmount").val() || 0,
            netAmount: +$("#netAmount").val() || 0,
            paymentStatus: $("#paymentStatus").val(),
            paymentMode: $("#paymentMode").val() || "Not Defined",
            opdConsultationFee: +$("#opdConsultationFee").val() || 0,
            medicationCharges: +$("#medicationCharges").val() || 0,
            procedureCharges: +$("#procedureCharges").val() || 0,
            notes: $("#notes").val()
        };

        $.ajax({
            url: "/Bill/AddBill",
            type: "POST",
            contentType: "application/json",
            headers: { "RequestVerificationToken": token },
            data: JSON.stringify(dto)
        })
            .done(() => {
                closeModal('addOutpatientBillModal');
                fetchBills();
            })
            .fail(xhr => alert(xhr.responseJSON?.message || "Failed to add bill."));
    });

    // ------------------------------
    // Edit Bill Modal
    // ------------------------------
    function editBill(id) {
        console.log('Attempting to edit bill with ID:', id, 'Type:', typeof id);

        // First, let's check if the API is ready
        if (!gridOptions.api) {
            console.error('Grid API is not available');
            alert('Grid is not ready. Please try again.');
            return;
        }

        // Log all available nodes for debugging
        const allNodes = [];
        gridOptions.api.forEachNode(node => {
            allNodes.push({ id: node.id, data: node.data });
        });
        console.log('All available nodes:', allNodes);

        // Convert id to string to match getRowNodeId configuration
        const nodeId = id?.toString();
        console.log('Looking for node with ID:', nodeId);

        const node = gridOptions.api.getRowNode(nodeId);

        if (!node) {
            console.error("Bill not found for ID:", id);
            console.error("Available node IDs:", allNodes.map(n => n.id));

            // Fallback: search manually
            let foundData = null;
            gridOptions.api.forEachNode(n => {
                if (n.data?.id == id) { // Use == for loose comparison
                    foundData = n.data;
                }
            });

            if (foundData) {
                console.log('Found data via manual search:', foundData);
                populateEditForm(foundData);
                showModal('editBillModal');
                return;
            }

            alert("Bill not found in grid.");
            return;
        }

        console.log('Found node:', node);
        populateEditForm(node.data);
        showModal('editBillModal');
    }

    function populateEditForm(data) {
        $("#editBillId").val(data.id);
        $("#editPatientId").val(data.patientId);
        $("#editVisitId").val(data.visitId ?? 0);
        $("#editBillDate").val(data.billDate?.split("T")[0]);
        $("#editPaymentStatus").val(data.paymentStatus);
        $("#editPaymentMode").val(data.paymentMode);
        $("#editOpdConsultationFee").val(data.opdConsultationFee || 0);
        $("#editMedicationCharges").val(data.medicationCharges || 0);
        $("#editProcedureCharges").val(data.procedureCharges || 0);
        $("#editDiscountAmount").val(data.discountAmount || 0);
        $("#editTotalAmount").val(data.totalAmount || 0);
        $("#editNetAmount").val(data.netAmount || 0);
        $("#editNotes").val(data.notes || '');
    }

    // ------------------------------
    // Edit/Delete Events
    // ------------------------------
    $("#outpatientBillGrid").on("click", "button", function () {
        const id = $(this).data("id");
        const action = $(this).data("action");
        console.log('Button clicked - ID:', id, 'Action:', action);

        if (action === "edit") {
            editBill(id);
        } else if (action === "delete") {
            deleteBill(id);
        }
    });

    // Auto recalc (Edit)
    $("#editOpdConsultationFee, #editMedicationCharges, #editProcedureCharges, #editDiscountAmount").on("input", function () {
        const consult = +$("#editOpdConsultationFee").val() || 0;
        const meds = +$("#editMedicationCharges").val() || 0;
        const proc = +$("#editProcedureCharges").val() || 0;
        const discount = +$("#editDiscountAmount").val() || 0;
        const total = consult + meds + proc;
        const net = Math.max(total - discount, 0);
        $("#editTotalAmount").val(total.toFixed(2));
        $("#editNetAmount").val(net.toFixed(2));
    });

    $("#editBillForm").on("submit", function (e) {
        e.preventDefault();
        const token = $("#editBillForm input[name='__RequestVerificationToken']").val();
        const dto = {
            Id: +$("#editBillId").val(),
            PatientId: +$("#editPatientId").val(),
            VisitId: +$("#editVisitId").val(),
            BillDate: $("#editBillDate").val(),
            PaymentStatus: $("#editPaymentStatus").val(),
            PaymentMode: $("#editPaymentMode").val(),
            ProcedureCharges: +$("#editProcedureCharges").val() || 0,
            MedicationCharges: +$("#editMedicationCharges").val() || 0,
            OpdConsultationFee: +$("#editOpdConsultationFee").val() || 0,
            DiscountAmount: +$("#editDiscountAmount").val() || 0,
            TotalAmount: +$("#editTotalAmount").val() || 0,
            NetAmount: +$("#editNetAmount").val() || 0,
            Notes: $("#editNotes").val()
        };

        $.ajax({
            url: "/Bill/EditBill",
            type: "PUT",
            contentType: "application/json",
            headers: { "RequestVerificationToken": token },
            data: JSON.stringify(dto)
        })
            .done(() => {
                closeModal('editBillModal');
                fetchBills();
                alert("Bill updated successfully!");
            })
            .fail(xhr => alert(xhr.responseJSON?.message || "Failed to update bill."));
    });

    // ------------------------------
    // Delete Bill
    // ------------------------------
    function deleteBill(id) {
        if (!confirm("Delete this bill?")) return;
        $.ajax({
            url: `/Bill/DeleteBill?id=${id}`,
            type: "DELETE"
        })
            .done(() => {
                gridOptions.api.applyTransaction({ remove: [{ id }] });
                fetchBills();
                alert("Bill deleted.");
            })
            .fail(() => alert("Failed to delete bill."));
    }

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
                showModal('finalBillModal');
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