$(document).ready(function () {

    // ============================
    // Patient Visits AG Grid Setup
    // ============================

    const columnDefs = [
        { 
            headerName: "Actions", 
            field: "actions",
            width: 180,
            cellRenderer: function (params) {
                return `
                    <button 
                        class="btn btn-danger btn-sm discharge-summary-btn"
                        data-visitid="${params.data.id}">
                        <i class="bi bi-box-arrow-right me-1"></i> Discharge Summary
                    </button>`;

            },
            sortable: false,
            filter: false,
            pinned: 'left'
        },
        { headerName: "Visit ID", field: "id", width: 120 },
        { headerName: "Visit Type", field: "visitType" },
        { headerName: "Visit Date", field: "visitDate" },
        { headerName: "Doctor", field: "doctorName" },
        { headerName: "Room", field: "roomNumber" },
        { headerName: "Admission Date", field: "admissionDate" },
        { headerName: "Discharge Date", field: "dischargeDate" },
        { headerName: "Notes", field: "notes" }
    ];


    const gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        animateRows: true,
        defaultColDef: {
            sortable: true,
            filter: true,
            resizable: true
        },

        // Context Menu
        getContextMenuItems(params) {
            return [
                {
                    name: 'Discharge Summary',
                    action: function () {
                        alert("Discharge Summary clicked for Visit: " + params.node.data.id);
                    }
                },
                {
                    name: 'View Bills',
                    action: function () {
                        const visitId = params.node.data.id;
                        loadBillsPopup(visitId);  // ↓ Call popup logic
                    }
                },
                'separator',
                'copy',
                'copyWithHeaders',
                'export'
            ];
        }
    };

    // Init visits grid
    new agGrid.Grid(document.getElementById('patientVisitsGrid'), gridOptions);

    // Load visits
    $.ajax({
        url: '/PatientPortal/FetchPatientVisits',
        method: 'GET',
        success: function (response) {

            if (response.success === true) {
                console.log(response)
                const rows = response.result;

                if (rows.length === 0) {
                    $("#noRecordsMessage").show();
                    gridOptions.api.setRowData([]);
                } else {
                    $("#noRecordsMessage").hide();
                    gridOptions.api.setRowData(rows);
                }

            } else {
                console.log(response.message);
            }
        },
        error: function (response) {
            console.log(response.error);
        }
    });
1
    // ==================================
    // Bills Popup Logic + Bills AG Grid
    // ==================================

    // Bills grid columns
    const billColumnDefs = [
        {
            headerName: "Action",
            field: "action",
            width: 120,
            cellRenderer: function (params) {
                const status = params.data.paymentStatus?.toLowerCase();

                if (status === "paid") {
                    return `<button class="btn btn-secondary" disabled>Paid</button>`;
                }

                return `<button class="btn-pay-now btn btn-success" 
                data-bill-id="${params.data.id}" 
                data-visit-id="${params.data.visitId}">
                Pay Now
            </button>`;
            },
            sortable: false,
            filter: false,
            pinned: 'left'
        },
        { headerName: "Bill ID", field: "id", width: 110 },
        { headerName: "Visit ID", field: "visitId", width: 110 },
        { headerName: "Bill Date", field: "billDate" },
        { headerName: "Total", field: "totalAmount" },
        { headerName: "Discount", field: "discountAmount" },
        { headerName: "Net Amount", field: "netAmount" },
        { headerName: "Status", field: "paymentStatus" },
        { headerName: "Mode", field: "paymentMode" },
        { headerName: "Room Charges", field: "roomCharges" },
        { headerName: "Procedure Charges", field: "procedureCharges" },
        { headerName: "Medication Charges", field: "medicationCharges" },
        { headerName: "Consultation Charges", field: "consultationCharges" },
        { headerName: "OPD Fee", field: "opdConsultationFee" },
        { headerName: "Notes", field: "notes" }
    ];

    const billGridOptions = {
        columnDefs: billColumnDefs,
        rowData: [],
        animateRows: true,
        defaultColDef: {
            sortable: true,
            filter: true,
            resizable: true
        }
    };

    // init bills grid
    new agGrid.Grid(document.getElementById('patientBillsGrid'), billGridOptions);
    // Event delegation for Pay Now button clicks
    $(document).on('click', '.btn-pay-now', function () {
        const billId = $(this).data('bill-id');
        const visitId = $(this).data('visit-id');

        handlePayNow(billId, visitId);
    });

    // Handle Pay Now action
    function handlePayNow(billId, visitId) {
        console.log("Pay Now clicked for Bill ID:", billId, "Visit ID:", visitId);
        $('#PaymentFormVisitId').val(visitId);

        // Get the bill row data to populate amount
        const rowNode = billGridOptions.api.getRowNode(billId);
        let netAmount = 0;

        if (rowNode) {
            netAmount = rowNode.data.netAmount || 0;
        } else {
            // Fallback: search through all rows
            billGridOptions.api.forEachNode(node => {
                if (node.data.id === billId) {
                    netAmount = node.data.netAmount || 0;
                }
            });
        }

        // Close bills modal
        $("#billsModal").hide();

        // Populate payment modal
        $("#paymentBillId").text(billId);
        $("#paymentVisitId").text(visitId);
        $("#paymentAmount").text("₹" + parseFloat(netAmount).toFixed(2));

        // Store data in hidden attributes for later use
        $("#paymentModal").data("bill-id", billId);
        $("#paymentModal").data("visit-id", visitId);
        $("#paymentModal").data("amount", netAmount);

        // Reset form
        $("#paymentForm")[0].reset();
        $("#cardDetailsSection").hide();
        $("#upiDetailsSection").hide();
        $("#netBankingDetailsSection").hide();

        // Show payment modal
        $("#paymentModal").css("display", "flex");
    }

    // Close payment modal
    $("#closePaymentModal").click(function () {
        $("#paymentModal").hide();
        // Show bills modal again
        $("#billsModal").css("display", "flex");
    });

    // Open popup + load bills
    function loadBillsPopup(visitId) {

        $.ajax({
            url: '/PatientPortal/GetPatientBills',
            method: 'GET',
            data: { visitId: visitId },

            success: function (response) {

                if (response.success === true) {

                    const rows = response.result;

                    billGridOptions.api.setRowData(rows);

                    // Show modal
                    $("#billsModal").css("display", "flex");

                } else {
                    alert(response.message);
                }
            },

            error: function (response) {
                console.log(response.error);
                alert("Unable to load bills.");
            }
        });
    }

    // Close popup button
    $("#closeBillsModal").click(function () {
        $("#billsModal").hide();
    });
    // Payment method selection handler
    $("input[name='paymentMethod']").change(function () {
        const selectedMethod = $(this).val();

        // Hide all payment detail sections
        $("#cardDetailsSection").hide();
        $("#upiDetailsSection").hide();
        $("#netBankingDetailsSection").hide();

        // Show relevant section
        if (selectedMethod === "credit_card" || selectedMethod === "debit_card") {
            $("#cardDetailsSection").show();
            // Make card fields required
            $("#cardNumber, #cardExpiry, #cardCvv").prop("required", true);
            $("#upiId, #bankName").prop("required", false);
        } else if (selectedMethod === "upi") {
            $("#upiDetailsSection").show();
            // Make UPI field required
            $("#upiId").prop("required", true);
            $("#cardNumber, #cardExpiry, #cardCvv, #bankName").prop("required", false);
        } else if (selectedMethod === "net_banking") {
            $("#netBankingDetailsSection").show();
            // Make bank field required
            $("#bankName").prop("required", true);
            $("#cardNumber, #cardExpiry, #cardCvv, #upiId").prop("required", false);
        }
    });

    // Card number formatting
    $("#cardNumber").on("input", function () {
        let value = $(this).val().replace(/\s/g, "");
        let formattedValue = value.match(/.{1,4}/g)?.join(" ") || value;
        $(this).val(formattedValue);
    });

    // Expiry date formatting
    $("#cardExpiry").on("input", function () {
        let value = $(this).val().replace(/\D/g, "");
        if (value.length >= 2) {
            value = value.substring(0, 2) + "/" + value.substring(2, 4);
        }
        $(this).val(value);
    });

    // CVV - only numbers
    $("#cardCvv").on("input", function () {
        $(this).val($(this).val().replace(/\D/g, ""));
    });

    // Payment form submission
    $("#paymentForm").submit(function (e) {
        e.preventDefault();

        const billId = $("#paymentModal").data("bill-id");
        const visitId = $("#paymentModal").data("visit-id");
        const amount = $("#paymentModal").data("amount");

        const paymentData = {
            billId: billId,
            visitId: visitId,
            amount: amount,
            paymentMode: $("input[name='paymentMethod']:checked").val(),
            paymentStatus: "Processing"
        };

        // Add payment-specific fields
        if (paymentData.paymentMethod === "credit_card" || paymentData.paymentMethod === "debit_card") {
            paymentData.cardNumber = $("#cardNumber").val();
            paymentData.cardExpiry = $("#cardExpiry").val();
            paymentData.cardCvv = $("#cardCvv").val();
        } else if (paymentData.paymentMethod === "upi") {
            paymentData.upiId = $("#upiId").val();
        } else if (paymentData.paymentMethod === "net_banking") {
            paymentData.bankName = $("#bankName").val();
        }

        console.log("Payment Data:", paymentData);

        var token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            url: '/Payment/ProcessPayment',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(paymentData),
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    $("#paymentModal").hide();
                    $("#paymentSuccessModal").modal("show");
                    // Reload bills to update payment status
                    loadBillsPopup(visitId);
                } else {
                    alert("Payment failed: " + response.message);
                }
            },
            error: function () {
                alert("Payment processing error. Please try again.");
            }
        });
    });

    $(document).on('click', '.discharge-summary-btn', function () {
        console.log("Clicked")
        const visitId = $(this).data('visitid');
        $.ajax({
            url: '/PatientPortal/GenerateDischargeSummary',
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
