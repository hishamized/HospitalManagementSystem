function hideModal(id) {
    const modal = $(id);
    modal.addClass("hidden");
}
$(document).ready(function () {

    // Modals (Tailwind: toggle 'hidden')
    const testsModal = $("#prescribeTestsModal");
    const viewTestRequestsModal = $("#viewTestRequestsModal");
    const updateStatusModal = $("#updateStatusModal");

    // Containers
    const testContainer = $("#testListContainer");

    // Helper: close a modal element (add 'hidden')
    function closeModal($modal) {
        $modal.addClass("hidden");
    }

    // Helper: open a modal element (remove 'hidden')
    function openModal($modal) {
        $modal.removeClass("hidden");
    }

    // Close when clicking overlay (assumes overlay is first child or uses absolute inset element)
    $(document).on("click", "#prescribeTestsModal > .absolute, #viewTestRequestsModal > .absolute, #updateStatusModal > .absolute", function (e) {
        // if click hits overlay layer (not the dialog)
        const $target = $(e.target);
        // overlay element typically covers entire modal; we close on click
        if ($target.is(".absolute") || $target.is("[data-modal-overlay]")) {
            closeModal($(this).closest("[id]"));
        }
    });

    // Close on Escape key
    $(document).on("keydown", function (e) {
        if (e.key === "Escape") {
            if (!testsModal.hasClass("hidden")) closeModal(testsModal);
            if (!viewTestRequestsModal.hasClass("hidden")) closeModal(viewTestRequestsModal);
            if (!updateStatusModal.hasClass("hidden")) closeModal(updateStatusModal);
        }
    });

    // OPEN PRESCRIBE TESTS MODAL
    $("#btn-prescribe-tests").on("click", function () {
        openModal(testsModal);
        loadValidTests();
    });

    // CLOSE PRESCRIBE TESTS MODAL
    $("#btn-close-tests, #btn-close-tests-2").on("click", function () {
        closeModal(testsModal);
    });

    // LOAD TESTS FROM SERVER
    function loadValidTests() {
        testContainer.html(`<div class="p-3 text-sm text-gray-400">Loading tests...</div>`);

        $.ajax({
            url: "/DoctorPortal/FetchValidTests",
            method: "GET",
            success: function (res) {
                testContainer.html("");

                if (!res || !res.data || res.data.length === 0) {
                    testContainer.html(`<div class="p-3 text-sm text-gray-400">No tests available.</div>`);
                    return;
                }

                res.data.forEach(test => {
                    testContainer.append(`
                        <label class="block w-full cursor-pointer transition-all duration-200 hover:bg-gray-800 px-3 py-2 rounded flex items-center justify-between">
                            <div class="flex items-center gap-3">
                                <input type="checkbox" class="test-checkbox w-5 h-5 rounded border-gray-600 bg-gray-800 text-purple-600 focus:ring-2 focus:ring-purple-500 focus:ring-offset-2 focus:ring-offset-gray-900 cursor-pointer"
                                       data-test-id="${test.labTestId}">
                                <span class="text-white font-medium">${escapeHtml(test.testName)}</span>
                            </div>
                            <span class="inline-flex items-center rounded-full bg-purple-600 px-3 py-1 text-xs font-semibold text-white">₹${escapeHtml(test.price)}</span>
                        </label>
                    `);
                });
            },
            error: function () {
                testContainer.html(`<div class="p-3 text-sm text-red-400">Failed to load tests.</div>`);
            }
        });
    }

    // SUBMIT LAB REQUEST
    $("#btn-submit-tests").on("click", function () {

        const selectedTests = $(".test-checkbox:checked").map(function () {
            return {
                LabTestId: Number($(this).data("test-id")),
                Status: "Pending",
                CreatedAt: new Date().toISOString(),
                UpdatedAt: null,
                IsActive: true
            };
        }).get();

        if (selectedTests.length === 0) {
            Swal.fire({
                icon: "warning",
                title: "No Test Selected",
                text: "Please select at least one test.",
                confirmButtonColor: "#9333ea",
                background: "#1f2937",
                color: "#fff"
            });
            return;
        }

        const patientId = $("#patientId").val();

        const labRequest = {
            PatientId: Number(patientId),
            DoctorId: null,  // controller will override via claims
            RequestDate: new Date().toISOString(),
            Status: "Pending",
            Notes: $("#labRequestNotes").val() || null,
            CreatedAt: new Date().toISOString(),
            UpdatedAt: null,
            IsActive: true
        };

        const finalPayload = {
            LabRequest: labRequest,
            LabRequestItems: selectedTests
        };

        Swal.fire({
            title: "Submitting...",
            text: "Please wait while we submit the lab request.",
            allowOutsideClick: false,
            showConfirmButton: false,
            background: "#1f2937",
            color: "#fff",
            didOpen: () => {
                Swal.showLoading();
            }
        });

        $.ajax({
            url: "/DoctorPortal/SubmitLabRequest",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(finalPayload),
            success: function (res) {
                Swal.fire({
                    icon: "success",
                    title: "Success",
                    text: res.message || "Lab request submitted successfully!",
                    confirmButtonColor: "#9333ea",
                    background: "#1f2937",
                    color: "#fff"
                }).then(() => {
                    // close Tailwind modal
                    closeModal(testsModal);
                    // Clear selections
                    $(".test-checkbox").prop('checked', false);
                    $("#labRequestNotes").val('');
                    // Refresh test requests if grid exists
                    if (typeof testRequestsGridApi !== "undefined" && testRequestsGridApi) {
                        loadTestRequests();
                    }
                });
            },
            error: function (xhr) {
                Swal.fire({
                    icon: "error",
                    title: "Oops!",
                    text: xhr.responseJSON?.message || "Something went wrong while submitting.",
                    confirmButtonColor: "#9333ea",
                    background: "#1f2937",
                    color: "#fff"
                });
            }
        });
    });

    // AG Grid and Test Requests
    let testRequestsGridApi;
    const patientId = $('#patientId').val();

    // View Test Requests Button Handler
    $('#btn-view-test-requests').on('click', function () {
        loadTestRequests();
        openModal(viewTestRequestsModal);

        // Adjust grid after modal is visible
        setTimeout(() => {
            if (testRequestsGridApi) {
                try {
                    testRequestsGridApi.resetRowHeights();
                    testRequestsGridApi.sizeColumnsToFit();
                } catch (e) {
                    console.warn("AG Grid adjust failed:", e);
                }
            }
        }, 150);
    });

    // AG Grid Column Definitions with Tailwind styling (Font Awesome icons inside templates)
    const testRequestsColumnDefs = [
        {
            headerName: 'Request ID',
            field: 'labRequestId',
            minWidth: 140,
            maxWidth: 160,
            filter: 'agNumberColumnFilter',
            cellClass: 'font-bold text-purple-400'
        },
        {
            headerName: 'Request Date',
            field: 'requestDate',
            minWidth: 210,
            filter: 'agDateColumnFilter',
            valueFormatter: params => {
                if (!params.value) return '';
                const date = new Date(params.value);
                return date.toLocaleDateString('en-US', {
                    year: 'numeric',
                    month: 'short',
                    day: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit'
                });
            }
        },
        {
            headerName: 'Test Name',
            field: 'testName',
            minWidth: 260,
            filter: 'agTextColumnFilter',
            cellClass: 'font-semibold text-white'
        },
        {
            headerName: 'Description',
            field: 'description',
            minWidth: 320,
            flex: 1,
            wrapText: true,
            autoHeight: true,
            filter: 'agTextColumnFilter',
            tooltipField: 'description',
            cellClass: 'text-sm text-gray-300 leading-relaxed'
        },
        {
            headerName: 'Request Item Status',
            field: 'labRequestItemStatus',
            minWidth: 200,
            filter: 'agTextColumnFilter',
            tooltipField: 'labRequestItemStatus',
            cellRenderer: params => {
                const status = (params.value || 'pending').toLowerCase();
                let badgeClass = '';
                let statusText = params.value || 'Pending';

                switch (status) {
                    case 'pending':
                        badgeClass = 'bg-yellow-600 text-white';
                        break;
                    case 'complete':
                        badgeClass = 'bg-green-600 text-white';
                        break;
                    case 'resultawaited':
                        badgeClass = 'bg-blue-600 text-white';
                        break;
                    case 'finished':
                        badgeClass = 'bg-purple-600 text-white';
                        break;
                    case 'cancelled':
                        badgeClass = 'bg-red-600 text-white';
                        break;
                    case 'rejected':
                        badgeClass = 'bg-gray-600 text-white';
                        break;
                    default:
                        badgeClass = 'bg-gray-600 text-white';
                }

                return `
                    <span class="inline-flex items-center rounded-full px-3 py-1 text-xs font-semibold ${badgeClass}">
                        ${escapeHtml(statusText)}
                    </span>
                `;
            }
        },
        {
            headerName: 'Sample Type',
            field: 'sampleType',
            minWidth: 150,
            filter: 'agTextColumnFilter',
            cellRenderer: params => {
                const sampleType = params.value || 'N/A';
                let badgeClass = '';

                switch (sampleType.toLowerCase()) {
                    case 'blood':
                        badgeClass = 'bg-red-600 text-white';
                        break;
                    case 'urine':
                        badgeClass = 'bg-yellow-600 text-white';
                        break;
                    case 'n/a':
                        badgeClass = 'bg-gray-600 text-white';
                        break;
                    default:
                        badgeClass = 'bg-blue-600 text-white';
                }

                return `
                    <span class="inline-flex items-center rounded-full px-3 py-1.5 text-xs font-semibold ${badgeClass}">
                        ${escapeHtml(sampleType)}
                    </span>
                `;
            }
        },
        {
            headerName: 'Normal Range',
            field: 'normalRange',
            minWidth: 200,
            filter: 'agTextColumnFilter',
            tooltipField: 'normalRange',
            cellClass: 'text-sm text-gray-300'
        },
        {
            headerName: 'Price',
            field: 'price',
            minWidth: 130,
            maxWidth: 150,
            filter: 'agNumberColumnFilter',
            cellClass: 'text-right font-bold text-green-400',
            valueFormatter: params => params.value ? `$${params.value.toFixed(2)}` : ''
        },
        {
            headerName: 'Actions',
            field: 'labRequestItemId',
            pinned: 'right',
            minWidth: 200,
            maxWidth: 220,
            filter: false,
            sortable: false,
            cellRenderer: params => {
                const status = (params.data.labRequestItemStatus || '').toLowerCase();

                let updateBtn = '';
                if (status === 'pending') {
                    updateBtn = `
                        <button 
                            class="update-status-btn inline-flex items-center gap-1.5 rounded-lg border border-green-600 bg-transparent px-3 py-1.5 text-xs font-medium text-green-400 transition-all duration-200 hover:bg-green-600 hover:text-white focus:outline-none focus:ring-2 focus:ring-green-500"
                            data-item-id="${params.value}"
                            data-lab-request-id="${escapeHtml(params.data.labRequestId)}"
                            data-lab-test-id="${escapeHtml(params.data.labTestId)}"
                            data-patient-id="${escapeHtml(params.data.patientId)}"
                            data-current-status="${escapeHtml(params.data.labRequestItemStatus)}">
                            <i class="fa-solid fa-pen-to-square"></i>
                            <span>Update</span>
                        </button>
                    `;
                }

                return `
                    <div class="flex gap-2">
                        <button 
                            class="view-test-detail-btn inline-flex items-center gap-1.5 rounded-lg border border-purple-600 bg-transparent px-3 py-1.5 text-xs font-medium text-purple-400 transition-all duration-200 hover:bg-purple-600 hover:text-white focus:outline-none focus:ring-2 focus:ring-purple-500"
                            data-item-id="${params.value}">
                            <i class="fa-solid fa-eye"></i>
                            <span>View</span>
                        </button>
                        ${updateBtn}
                    </div>
                `;
            }
        }
    ];

    // AG Grid Options
    const testRequestsGridOptions = {
        columnDefs: testRequestsColumnDefs,
        defaultColDef: {
            resizable: true,
            sortable: true,
            filter: true,
            floatingFilter: true,
            minWidth: 120
        },
        rowSelection: 'single',
        animateRows: true,
        pagination: true,
        paginationPageSize: 15,
        paginationPageSizeSelector: [10, 15, 25, 50],
        enableCellTextSelection: true,
        tooltipShowDelay: 300,
        headerHeight: 52,
        rowHeight: 50,
        overlayLoadingTemplate: '<span class="text-gray-400">Loading test requests...</span>',
        overlayNoRowsTemplate: '<span class="text-gray-400">No test requests found</span>',
        onGridReady(params) {
            testRequestsGridApi = params.api;
            params.api.sizeColumnsToFit();
        }
    };

    // Add export functionality
    $(document).on('click', '#btn-export-requests', function () {
        if (testRequestsGridApi) {
            testRequestsGridApi.exportDataAsCsv({
                fileName: `patient_${patientId}_lab_requests_${new Date().toISOString().split('T')[0]}.csv`,
                columnKeys: ['labRequestId', 'requestDate', 'testName', 'description', 'labRequestItemStatus', 'sampleType', 'normalRange', 'price']
            });
            showToast('Success', 'Test requests exported successfully', 'success');
        }
    });

    // Initialize AG Grid for Test Requests
    const testRequestsGridDiv = document.querySelector('#testRequestsGrid');
    if (testRequestsGridDiv) {
        new agGrid.Grid(testRequestsGridDiv, testRequestsGridOptions);
    }

    // Load Test Requests Data
    function loadTestRequests() {
        if (!testRequestsGridApi) {
            console.error('Grid API not initialized');
            return;
        }

        // Show loader
        testRequestsGridApi.showLoadingOverlay();

        // Check if ajaxHelpers is available
        if (typeof ajaxHelpers !== 'undefined' && ajaxHelpers.ajaxCall) {
            ajaxHelpers.ajaxCall(
                "POST",
                "/DoctorPortal/FetchLabRequestsWithItemsByPatient",
                { patientId: patientId },
                "json",
                function (response) {
                    // SUCCESS
                    if (response.success && response.data) {
                        testRequestsGridApi.setRowData(response.data);
                        testRequestsGridApi.hideOverlay();
                        showToast('Success', `Loaded ${response.data.length} test request(s)`, 'success');
                    } else {
                        testRequestsGridApi.setRowData([]);
                        testRequestsGridApi.showNoRowsOverlay();
                        showToast('Info', 'No test requests found for this patient', 'info');
                    }
                }
            ).fail(function (jqXhr, status, error) {
                testRequestsGridApi.setRowData([]);
                testRequestsGridApi.showNoRowsOverlay();
                console.error('Error loading test requests:', jqXhr.responseText);
                showToast('Error', 'Failed to load test requests', 'danger');
            });
        } else {
            // Fallback to regular AJAX if ajaxHelpers is not available
            $.ajax({
                url: "/DoctorPortal/FetchLabRequestsWithItemsByPatient",
                method: "POST",
                contentType: "application/json",
                data: JSON.stringify({ patientId: patientId }),
                success: function (response) {
                    if (response.success && response.data) {
                        testRequestsGridApi.setRowData(response.data);
                        testRequestsGridApi.hideOverlay();
                        showToast('Success', `Loaded ${response.data.length} test request(s)`, 'success');
                    } else {
                        testRequestsGridApi.setRowData([]);
                        testRequestsGridApi.showNoRowsOverlay();
                        showToast('Info', 'No test requests found for this patient', 'info');
                    }
                },
                error: function (jqXhr) {
                    testRequestsGridApi.setRowData([]);
                    testRequestsGridApi.showNoRowsOverlay();
                    console.error('Error loading test requests:', jqXhr.responseText);
                    showToast('Error', 'Failed to load test requests', 'danger');
                }
            });
        }
    }

    // View Test Detail Button Handler (event delegation)
    $(document).on('click', '.view-test-detail-btn', function () {
        const itemId = $(this).data('item-id');

        Swal.fire({
            title: 'Test Details',
            text: `View details for test item ID: ${itemId}`,
            icon: 'info',
            confirmButtonColor: "#9333ea",
            background: "#1f2937",
            color: "#fff"
        });

        // TODO: Implement view test detail functionality
        // You can open another modal or navigate to a detail page
    });

    // Enhanced Toast notification helper with Tailwind styling
    function showToast(title, message, type) {
        // Map type to Tailwind colors
        const typeColors = {
            'success': 'bg-green-600',
            'danger': 'bg-red-600',
            'warning': 'bg-yellow-600',
            'info': 'bg-blue-600'
        };

        const typeIcons = {
            'success': 'fa-check-circle',
            'danger': 'fa-exclamation-circle',
            'warning': 'fa-exclamation-triangle',
            'info': 'fa-info-circle'
        };

        const bgColor = typeColors[type] || 'bg-gray-600';
        const icon = typeIcons[type] || 'fa-bell';

        const toastHtml = `
            <div class="toast-item mb-2 animate-slide-in ${bgColor} rounded-lg shadow-lg text-white p-4 flex items-start gap-3 max-w-md" role="alert">
                <i class="fa-solid ${icon} text-xl mt-0.5"></i>
                <div class="flex-1">
                    <strong class="font-semibold">${escapeHtml(title)}</strong>
                    <p class="text-sm mt-1 opacity-90">${escapeHtml(message)}</p>
                </div>
                <button type="button" class="toast-close ml-2 text-white hover:text-gray-200 transition-colors" aria-label="Close">
                    <i class="fa-solid fa-times"></i>
                </button>
            </div>
        `;

        let toastContainer = $('#toastContainer');
        if (toastContainer.length === 0) {
            $('body').append('<div id="toastContainer" class="fixed top-4 right-4 z-50 space-y-2"></div>');
            toastContainer = $('#toastContainer');
        }

        const $toast = $(toastHtml);
        toastContainer.append($toast);

        // Close button handler
        $toast.find('.toast-close').on('click', function () {
            $toast.addClass('animate-slide-out');
            setTimeout(() => $toast.remove(), 300);
        });

        // Auto remove after 4 seconds
        setTimeout(function () {
            $toast.addClass('animate-slide-out');
            setTimeout(() => $toast.remove(), 300);
        }, 4000);
    }

    // Update Status Button Handler (opens updateStatusModal)
    $(document).on('click', '.update-status-btn', function () {
        $("#us_LabRequestItemId").val($(this).data("item-id"));
        $("#us_LabRequestId").val($(this).data("lab-request-id"));
        $("#us_LabTestId").val($(this).data("lab-test-id"));
        $("#us_PatientId").val($(this).data("patient-id"));

        const currentStatus = $(this).data("current-status");
        $("#us_NewStatus").val(currentStatus);

        openModal(updateStatusModal);
    });

    // Save Status Button Handler
    $("#btn-save-status").on("click", function () {
        const payload = {
            DoctorId: 0,  // controller will override
            LabRequestItemId: Number($("#us_LabRequestItemId").val()),
            LabRequestId: Number($("#us_LabRequestId").val()),
            LabTestId: Number($("#us_LabTestId").val()),
            PatientId: Number($("#us_PatientId").val()),
            NewStatus: $("#us_NewStatus").val()
        };

        // Validate payload
        if (!payload.LabRequestItemId || !payload.NewStatus) {
            showToast("Warning", "Please select a valid status", "warning");
            return;
        }

        // Show loading
        Swal.fire({
            title: "Updating Status...",
            text: "Please wait",
            allowOutsideClick: false,
            showConfirmButton: false,
            background: "#1f2937",
            color: "#fff",
            didOpen: () => {
                Swal.showLoading();
            }
        });

        $.ajax({
            url: "/DoctorPortal/UpdateLabRequestItemStatus",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(payload),
            success: function (res) {
                Swal.close();
                closeModal(updateStatusModal);
                showToast("Success", res.message || "Status updated successfully", "success");
                loadTestRequests(); // refresh grid
            },
            error: function (xhr) {
                Swal.close();
                showToast("Error", xhr.responseJSON?.message || "Failed to update status", "danger");
            }
        });
    });

    // Add CSS animations for toasts if not already present
    if (!document.getElementById('toast-animations-style')) {
        const style = document.createElement('style');
        style.id = 'toast-animations-style';
        style.textContent = `
            @keyframes slide-in {
                from {
                    transform: translateX(100%);
                    opacity: 0;
                }
                to {
                    transform: translateX(0);
                    opacity: 1;
                }
            }

            @keyframes slide-out {
                from {
                    transform: translateX(0);
                    opacity: 1;
                }
                to {
                    transform: translateX(100%);
                    opacity: 0;
                }
            }

            .animate-slide-in {
                animation: slide-in 0.3s ease-out forwards;
            }

            .animate-slide-out {
                animation: slide-out 0.3s ease-in forwards;
            }
        `;
        document.head.appendChild(style);
    }

    // Simple HTML-escape helper to avoid injection when inserting raw content
    function escapeHtml(str) {
        if (str === null || str === undefined) return "";
        return String(str)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

});
