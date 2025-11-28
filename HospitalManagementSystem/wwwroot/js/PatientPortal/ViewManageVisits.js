function revealModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.remove('hidden');
        document.body.classList.add('overflow-hidden');
    }
}

function wrapModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.add('hidden');
        modal.style.display = "none";
        document.body.classList.remove('overflow-hidden'); // Restore body scroll
    }
}


// ViewManageVisits.js
$(document).ready(function () {

    // ---------- Utility ----------
    function safeApiCall(apiFn) {
        try { apiFn(); } catch (e) { console.warn("AG Grid API not ready yet.", e); }
    }

    // ---------- Column Definitions (Visits) ----------
    const columnDefs = [
        {
            headerName: "Actions",
            field: "actions",
            width: 300,
            cellRenderer: function (params) {
                // Tailwind-style action buttons (no Bootstrap)
                return `
                    <div class="flex gap-2 items-center">
                        <button
                            class="discharge-summary-btn inline-flex items-center gap-2 rounded-md bg-red-600 px-3 py-1 text-xs font-medium text-white hover:bg-red-700"
                            data-visitid="${params.data?.id ?? ''}">
                            <i class="fa-solid fa-file-medical"></i>
                            <span>Discharge</span>
                        </button>
                        <button
                            class="view-bills-btn inline-flex items-center gap-2 rounded-md bg-purple-600 px-3 py-1 text-xs font-medium text-white hover:bg-purple-700"
                            data-visitid="${params.data?.id ?? ''}">
                            <i class="fa-solid fa-file-invoice-dollar"></i>
                            <span>Bills</span>
                        </button>
                    </div>
                `;
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
        { headerName: "Notes", field: "notes", wrapText: true, autoHeight: true }
    ];

    const gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        animateRows: true,
        defaultColDef: {
            sortable: true,
            filter: true,
            resizable: true,
            minWidth: 80
        },
        onGridReady: function (params) {
            // Ensure grid sizes its columns once ready
            setTimeout(() => safeApiCall(() => params.api.sizeColumnsToFit()), 50);
        },
        getContextMenuItems(params) {
            return [
                {
                    name: 'Discharge Summary',
                    action: function () {
                        const visitId = params.node?.data?.id;
                        if (visitId) openDischargeSummary(visitId);
                    }
                },
                {
                    name: 'View Bills',
                    action: function () {
                        const visitId = params.node?.data?.id;
                        if (visitId) loadBillsPopup(visitId);
                    }
                },
                'separator',
                'copy',
                'copyWithHeaders',
                'export'
            ];
        }
    };

    // Initialize visits grid
    const visitsGridDiv = document.getElementById('patientVisitsGrid');
    new agGrid.Grid(visitsGridDiv, gridOptions);

    // Resize handling (debounced)
    let resizeTimer;
    $(window).on('resize', function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(() => {
            safeApiCall(() => gridOptions.api.doLayout());
            safeApiCall(() => gridOptions.api.sizeColumnsToFit());
            safeApiCall(() => billGridOptions.api.doLayout && billGridOptions.api.sizeColumnsToFit && billGridOptions.api.sizeColumnsToFit());
        }, 150);
    });

    // ---------- Load Visits ----------
    function loadVisits() {
        $.ajax({
            url: '/PatientPortal/FetchPatientVisits',
            method: 'GET',
            success: function (response) {
                if (response?.success === true) {
                    const rows = response.result || [];
                    if (rows.length === 0) {
                        $("#noRecordsMessage").removeClass('hidden');
                        safeApiCall(() => gridOptions.api.setRowData([]));
                    } else {
                        $("#noRecordsMessage").addClass('hidden');
                        safeApiCall(() => gridOptions.api.setRowData(rows));
                        // after setting data make sure columns fit
                        setTimeout(() => safeApiCall(() => gridOptions.api.sizeColumnsToFit()), 80);
                    }
                } else {
                    console.warn("FetchPatientVisits failed:", response?.message);
                }
            },
            error: function (err) {
                console.error("Error fetching visits:", err);
            }
        });
    }

    loadVisits();

    // ---------- Bills grid ----------
    const billColumnDefs = [
        {
            headerName: "Action",
            field: "action",
            width: 140,
            cellRenderer: function (params) {
                const status = (params.data?.paymentStatus || '').toLowerCase();
                if (status === "paid") {
                    return `<button class="inline-flex items-center gap-2 rounded-md bg-gray-600 px-3 py-1 text-xs text-white" disabled>Paid</button>`;
                }
                // btn-pay-now used for event delegation
                return `<button class="btn-pay-now inline-flex items-center gap-2 rounded-md bg-green-600 px-3 py-1 text-xs text-white hover:bg-green-700"
                            data-bill-id="${params.data?.id ?? ''}"
                            data-visit-id="${params.data?.visitId ?? ''}">
                            <i class="fa-solid fa-credit-card"></i><span>Pay</span>
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
        { headerName: "Notes", field: "notes" }
    ];

    const billGridOptions = {
        columnDefs: billColumnDefs,
        rowData: [],
        animateRows: true,
        defaultColDef: {
            sortable: true,
            filter: true,
            resizable: true,
            minWidth: 80
        },
        onGridReady: function (params) {
            setTimeout(() => safeApiCall(() => params.api.sizeColumnsToFit()), 60);
        }
    };

    // Initialize bills grid
    const billsGridDiv = document.getElementById('patientBillsGrid');
    new agGrid.Grid(billsGridDiv, billGridOptions);

    // ---------- Modal helpers (jQuery-driven) ----------
    function showModal(modalSelector) {
        // For Tailwind/Alpine markup we force display with jQuery
        const $m = $(modalSelector);
        $m.css('display', 'flex').attr('aria-hidden', 'false');
        // ensure inner grid recalculates if this modal contains a grid
        if (modalSelector === '#billsModal') {
            setTimeout(() => {
                safeApiCall(() => billGridOptions.api.doLayout());
                safeApiCall(() => billGridOptions.api.sizeColumnsToFit());
            }, 80);
        }
        if (modalSelector === '#paymentModal') {
            // nothing grid-related but keep consistent
        }
        // prevent body scroll
        $('body').addClass('overflow-hidden');
    }

    function hideModal(modalSelector) {
        $(modalSelector).hide().attr('aria-hidden', 'true');
        $('body').removeClass('overflow-hidden');
    }

    // Close buttons bindings
    $('#closeBillsModal').on('click', function () { hideModal('#billsModal'); });
    $('#closePaymentModal').on('click', function () {
        hideModal('#paymentModal');
        // reopen bills when closing payment (same behavior as old)
        showModal('#billsModal');
    });

    // Click handler for View Bills (from action button)
    $(document).on('click', '.view-bills-btn', function () {
        const visitId = $(this).data('visitid');
        if (visitId) loadBillsPopup(visitId);
    });

    // Discharge button click handler (from action column)
    $(document).on('click', '.discharge-summary-btn', function () {
        const visitId = $(this).data('visitid');
        if (!visitId) return;
        $.ajax({
            url: '/PatientPortal/GenerateDischargeSummary',
            method: 'GET',
            data: { VisitId: visitId },
            success: function (response) {
                if (response?.success) {
                    populateDischargeModal(response.data);
                    // bootstrap modal replaced with simple show since you kept bootstrap discharge modal
                    // If you switched discharge modal to Tailwind, use showModal('#dischargeSummaryModal') instead.
                    showModal('#dischargeSummaryModal');

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

    // ---------- Load bills and open bills modal ----------
    function loadBillsPopup(visitId) {
        $.ajax({
            url: '/PatientPortal/GetPatientBills',
            method: 'GET',
            data: { visitId: visitId },
            success: function (response) {
                if (response?.success === true) {
                    const rows = response.result || [];
                    safeApiCall(() => billGridOptions.api.setRowData(rows));
                    // store current visit on modal for reference
                    $('#billsModal').data('visit-id', visitId);
                    showModal('#billsModal');
                } else {
                    alert(response?.message || 'Unable to fetch bills.');
                }
            },
            error: function (err) {
                console.error("GetPatientBills err:", err);
                alert("Unable to load bills.");
            }
        });
    }

    // ---------- Pay Now handling (delegated) ----------
    $(document).on('click', '.btn-pay-now', function () {
        const billId = $(this).data('bill-id');
        const visitId = $(this).data('visit-id');
        handlePayNow(billId, visitId);
    });

    function findBillRowData(billId) {
        if (!billGridOptions.api) return null;

        // First try getRowNode (if your rowId is set to the bill id)
        try {
            const rn = billGridOptions.api.getRowNode && billGridOptions.api.getRowNode(billId);
            if (rn && rn.data) return rn.data;
        } catch (e) {
            // ignore
        }

        // Otherwise iterate
        let found = null;
        billGridOptions.api.forEachNode(node => {
            if (node && node.data && ('' + node.data.id) === ('' + billId)) {
                found = node.data;
            }
        });
        return found;
    }

    function handlePayNow(billId, visitId) {
        $('#PaymentFormVisitId').val(visitId || '');

        let netAmount = 0;
        const billData = findBillRowData(billId);
        if (billData) netAmount = billData.netAmount || 0;

        // Close bills modal (jQuery)
        hideModal('#billsModal');

        // Populate payment modal fields
        $('#paymentBillId').text(billId || '-');
        $('#paymentVisitId').text(visitId || '-');
        $('#paymentAmount').text("₹" + (parseFloat(netAmount) || 0).toFixed(2));

        // store metadata
        $('#paymentModal').data('bill-id', billId);
        $('#paymentModal').data('visit-id', visitId);
        $('#paymentModal').data('amount', netAmount);

        // Reset and hide all payment detail sections
        $('#paymentForm')[0].reset();
        $('#cardDetailsSection, #upiDetailsSection, #netBankingDetailsSection').hide();

        // Show payment modal
        showModal('#paymentModal');
    }

    // ---------- Payment method selection ----------
    $(document).on('change', "input[name='paymentMethod']", function () {
        const selectedMethod = $(this).val();
        $('#cardDetailsSection, #upiDetailsSection, #netBankingDetailsSection').hide();

        if (selectedMethod === "credit_card" || selectedMethod === "debit_card") {
            $('#cardDetailsSection').show();
            $('#cardNumber, #cardExpiry, #cardCvv').prop('required', true);
            $('#upiId, #bankName').prop('required', false);
        } else if (selectedMethod === "upi") {
            $('#upiDetailsSection').show();
            $('#upiId').prop('required', true);
            $('#cardNumber, #cardExpiry, #cardCvv, #bankName').prop('required', false);
        } else if (selectedMethod === "net_banking") {
            $('#netBankingDetailsSection').show();
            $('#bankName').prop('required', true);
            $('#cardNumber, #cardExpiry, #cardCvv, #upiId').prop('required', false);
        }
    });

    // ---------- Card/Expiry/CVV formatting ----------
    $('#cardNumber').on('input', function () {
        let value = $(this).val().replace(/\s/g, "");
        $(this).val(value.match(/.{1,4}/g)?.join(" ") || value);
    });

    $('#cardExpiry').on('input', function () {
        let value = $(this).val().replace(/\D/g, "");
        if (value.length >= 3) value = value.substring(0, 2) + "/" + value.substring(2, 4);
        $(this).val(value);
    });

    $('#cardCvv').on('input', function () {
        $(this).val($(this).val().replace(/\D/g, ""));
    });

    // ---------- Payment submit ----------
    $('#paymentForm').on('submit', function (e) {
        e.preventDefault();

        const billId = $('#paymentModal').data('bill-id');
        const visitId = $('#paymentModal').data('visit-id');
        const amount = $('#paymentModal').data('amount');

        const paymentMode = $("input[name='paymentMethod']:checked").val();

        const paymentData = {
            billId: billId,
            visitId: visitId,
            amount: amount,
            paymentMode: paymentMode,
            paymentStatus: "Processing"
        };

        if (paymentMode === "credit_card" || paymentMode === "debit_card") {
            paymentData.cardNumber = $('#cardNumber').val();
            paymentData.cardExpiry = $('#cardExpiry').val();
            paymentData.cardCvv = $('#cardCvv').val();
        } else if (paymentMode === "upi") {
            paymentData.upiId = $('#upiId').val();
        } else if (paymentMode === "net_banking") {
            paymentData.bankName = $('#bankName').val();
        }

        console.log("Payment Data:", paymentData);

        const token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            url: '/Payment/ProcessPayment',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(paymentData),
            headers: { 'RequestVerificationToken': token },
            success: function (response) {
                if (response?.success) {
                    hideModal('#paymentModal');
                    // show success using Tailwind or bootstrap fallback
                    if (typeof bootstrap !== 'undefined') {
                        $('#paymentSuccessModal').modal('show');
                    } else {
                        // a minimal success toast/fallback
                        showTinySuccessToast("Payment successful.");
                    }
                    // reload bills to show updated status
                    loadBillsPopup(visitId);
                } else {
                    alert("Payment failed: " + (response?.message || 'Unknown'));
                }
            },
            error: function () {
                alert("Payment processing error. Please try again.");
            }
        });
    });

    // small toast fallback (very minimal)
    function showTinySuccessToast(msg) {
        const $t = $('<div class="fixed bottom-6 right-6 z-60 rounded bg-green-600 text-white px-4 py-2 shadow-lg">' + msg + '</div>');
        $('body').append($t);
        setTimeout(() => $t.fadeOut(300, () => $t.remove()), 1800);
    }

    // ---------- Discharge summary render ----------
    function populateDischargeModal(data) {
        let roundsHtml = '';
        if (data?.doctorRounds?.length > 0) {
            roundsHtml = data.doctorRounds.map(r => `
                <tr class="border-b">
                    <td class="px-2 py-1">${r.roundDate ? new Date(r.roundDate).toLocaleString() : ''}</td>
                    <td class="px-2 py-1">${r.doctorName || ''}</td>
                    <td class="px-2 py-1">${r.diagnosis || ''}</td>
                    <td class="px-2 py-1">${r.prescriptions || ''}</td>
                    <td class="px-2 py-1">${r.treatmentPlan || ''}</td>
                    <td class="px-2 py-1">${r.isCritical ? 'Yes' : 'No'}</td>
                </tr>
            `).join('');
        } else {
            roundsHtml = `<tr><td colspan="6" class="text-center text-gray-400">No doctor rounds recorded.</td></tr>`;
        }

        const html = `
            <div id="summaryToPrint">
                <h4 class="mb-3">${data.fullName} (${data.patientCode})</h4>
                <p><strong>Gender:</strong> ${data.gender} | <strong>DOB:</strong> ${new Date(data.dateOfBirth).toLocaleDateString()}</p>
                <p><strong>Contact:</strong> ${data.contactNumber} | <strong>Address:</strong> ${data.address || '-'}</p>

                <hr class="my-3"/>

                <h5>Visit Details</h5>
                <p><strong>Visit ID:</strong> ${data.visitId} | <strong>Visit Type:</strong> ${data.visitType}</p>
                <p><strong>Admission:</strong> ${data.admissionDate ? new Date(data.admissionDate).toLocaleDateString() : '-'} |
                   <strong>Discharge:</strong> ${data.dischargeDate ? new Date(data.dischargeDate).toLocaleDateString() : '-'}</p>
                <p><strong>Room:</strong> ${data.roomNumber || '-'} | <strong>Ward:</strong> ${data.wardName || '-'} | <strong>Bed:</strong> ${data.bedNumber || '-'}</p>

                <hr class="my-3"/>

                <h5>Doctor Details</h5>
                <p><strong>Doctor:</strong> ${data.doctorName || '-'} (ID: ${data.doctorId || '-'})</p>
                <p><strong>Treatment Summary:</strong> ${data.treatmentDetails || '-'}</p>
                <p><strong>Notes:</strong> ${data.notes || '-'}</p>

                <hr class="my-3"/>

                <h5>Doctor Rounds</h5>
                <table class="min-w-full border">
                    <thead class="bg-gray-100 text-left">
                        <tr>
                            <th class="px-2 py-1">Date</th>
                            <th class="px-2 py-1">Doctor</th>
                            <th class="px-2 py-1">Diagnosis</th>
                            <th class="px-2 py-1">Prescriptions</th>
                            <th class="px-2 py-1">Treatment Plan</th>
                            <th class="px-2 py-1">Critical</th>
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

    // ---------- PDF download ----------
    $('#downloadPdfBtn').on('click', function () {
        const element = document.getElementById('summaryToPrint');
        if (!element) return alert("Nothing to print.");
        html2pdf().set({
            margin: 0.5,
            filename: 'DischargeSummary.pdf',
            image: { type: 'jpeg', quality: 0.98 },
            html2canvas: { scale: 2 },
            jsPDF: { unit: 'in', format: 'a4', orientation: 'portrait' }
        }).from(element).save();
    });

}); // document.ready end
