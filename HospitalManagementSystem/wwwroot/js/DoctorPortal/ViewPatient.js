$(document).ready(function () {

    const modal = new bootstrap.Modal(document.getElementById("prescribeTestsModal"));
    const testContainer = $("#testListContainer");

    // OPEN MODAL
    $("#btn-prescribe-tests").on("click", function () {
        modal.show();
        loadValidTests();
    });

    // CLOSE MODAL
    $("#btn-close-tests, #btn-close-tests-2").on("click", function () {
        modal.hide();
    });

    // LOAD TESTS FROM SERVER
    function loadValidTests() {
        testContainer.html(`<div class="p-2 text-muted small">Loading tests...</div>`);

        $.ajax({
            url: "/DoctorPortal/FetchValidTests",
            method: "GET",

            success: function (res) {
                testContainer.html("");

                if (!res || !res.data || res.data.length === 0) {
                    testContainer.html(`<div class="p-2 text-muted">No tests available.</div>`);
                    return;
                }

                res.data.forEach(test => {
                    testContainer.append(`
                        <label class="list-group-item d-flex justify-content-between align-items-center">
                            <div class="d-flex align-items-center gap-2">
                                <input type="checkbox" class="form-check-input test-checkbox"
                                       data-test-id="${test.labTestId}">
                                <span>${test.testName}</span>
                            </div>
                            <span class="badge bg-primary rounded-pill">₹${test.price}</span>
                        </label>
                    `);
                });
            },

            error: function () {
                testContainer.html(`<div class="p-2 text-danger">Failed to load tests.</div>`);
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
                text: "Please select at least one test."
            });
            return;
        }

        const patientId = $("#patientId").val();

        const labRequest = {
            PatientId: Number(patientId),
            DoctorId: null,  // <-- controller will override via claims
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
                    text: res.message || "Lab request submitted successfully!"
                });

                // Optional: refresh grid or clear form
                // reloadLabRequestsGrid();
            },
            error: function (xhr) {
                Swal.fire({
                    icon: "error",
                    title: "Oops!",
                    text: xhr.responseJSON?.message || "Something went wrong while submitting."
                });
            }
        });
    });

    let testRequestsGridApi;
    const patientId = $('#patientId').val();

    $('#viewTestRequestsModal').on('shown.bs.modal', function () {
        setTimeout(() => {
            testRequestsGridApi.resetRowHeights();
            testRequestsGridApi.sizeColumnsToFit();
        }, 50);
    });

    // View Test Requests Button Handler
    $('#btn-view-test-requests').on('click', function () {
        loadTestRequests();
     
        $('#viewTestRequestsModal').modal('show');
    });

    const testRequestsColumnDefs = [
        {
            headerName: 'Request ID',
            field: 'labRequestId',
            minWidth: 140,
            maxWidth: 160,
            filter: 'agNumberColumnFilter',
            cellClass: 'fw-bold text-primary px-3 py-3'
        },
        {
            headerName: 'Request Date',
            field: 'requestDate',
            minWidth: 210,
            filter: 'agDateColumnFilter',
            cellClass: 'px-3 py-3',
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
            cellClass: 'fw-semibold px-3 py-3'
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
            cellClass: 'px-3 py-3 small lh-base'
        },
        {
            headerName: 'Request Item Status',
            field: 'labRequestItemStatus',
            minWidth: 320,
            flex: 1,
            wrapText: true,
            autoHeight: true,
            filter: 'agTextColumnFilter',
            tooltipField: 'labRequestItemStatus',
            cellClass: 'px-3 py-3 small lh-base'
        },
        {
            headerName: 'Sample Type',
            field: 'sampleType',
            minWidth: 150,
            filter: 'agTextColumnFilter',
            cellClass: 'text-center px-3 py-3',
            cellRenderer: params => {
                const sampleType = params.value || 'N/A';
                const badgeClass =
                    sampleType.toLowerCase() === 'blood' ? 'bg-danger text-light' :
                        sampleType.toLowerCase() === 'urine' ? 'bg-warning text-dark' :
                            sampleType.toLowerCase() === 'n/a' ? 'bg-secondary' :
                                'bg-info text-dark';

                return `
                <span class="badge ${badgeClass} px-3 py-2 rounded-pill">
                    ${sampleType}
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
            cellClass: 'px-3 py-3 small'
        },
        {
            headerName: 'Price',
            field: 'price',
            minWidth: 130,
            maxWidth: 150,
            filter: 'agNumberColumnFilter',
            cellClass: 'text-end fw-bold text-success px-3 py-3',
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
            cellClass: 'px-2 py-2',
            cellRenderer: params => {
                const status = (params.data.labRequestItemStatus || '').toLowerCase();


                let updateBtn = '';
                if (status === 'pending') {
                    updateBtn = `
                <button 
                    class="btn btn-outline-success btn-sm update-status-btn d-flex align-items-center gap-1 px-2 py-1"
                    data-item-id="${params.value}"
                    data-lab-request-id="${params.data.labRequestId}"
                    data-lab-test-id="${params.data.labTestId}"
                    data-patient-id="${params.data.patientId}"
                    data-current-status="${params.data.labRequestItemStatus}">
                    <i class="bi bi-pencil-square"></i>
                    Update
                </button>
            `;
                }

                return `
            <div class="d-flex gap-2">
                <button 
                    class="btn btn-outline-primary btn-sm view-test-detail-btn d-flex align-items-center gap-1 px-2 py-1"
                    data-item-id="${params.value}">
                    <i class="bi bi-eye"></i>
                    View
                </button>
                ${updateBtn}
            </div>
        `;
            }
        }

    ];


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
        onGridReady(params) {
            testRequestsGridApi = params.api;
            params.api.sizeColumnsToFit();
        }
    };

    // Add export functionality
    $(document).on('click', '#btn-export-requests', function () {
        if (testRequestsGridApi) {
            testRequestsGridApi.exportDataAsCsv({
                fileName: `patient_${patientId}_lab_requests.csv`
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

        // Show loader early
        testRequestsGridApi.showLoadingOverlay();

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
                    testRequestsGridApi.showNoRowsOverlay();
                    showToast('Info', 'No test requests found for this patient', 'info');
                }
            }
        ).fail(function (jqXhr, status, error) {
            // ERROR — Already handled inside ajaxHelpers BUT  
            // You might still want grid fallback behavior here
            testRequestsGridApi.showNoRowsOverlay();
            console.error('Error loading test requests:', jqXhr.responseText);
        });
    }


    // View Test Detail Button Handler (event delegation)
    $(document).on('click', '.view-test-detail-btn', function () {
        const itemId = $(this).data('item-id');
        // TODO: Implement view test detail functionality
        showToast('Info', `View details for test item ID: ${itemId}`, 'info');
    });

    // Toast notification helper
    function showToast(title, message, type) {
        const toastHtml = `
            <div class="toast align-items-center text-white bg-${type} border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        <strong>${title}:</strong> ${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        `;

        let toastContainer = $('#toastContainer');
        if (toastContainer.length === 0) {
            $('body').append('<div id="toastContainer" class="toast-container position-fixed top-0 end-0 p-3"></div>');
            toastContainer = $('#toastContainer');
        }

        const $toast = $(toastHtml);
        toastContainer.append($toast);

        const toast = new bootstrap.Toast($toast[0], { delay: 3000 });
        toast.show();

        $toast.on('hidden.bs.toast', function () {
            $(this).remove();
        });
    }

    let updateStatusModal = new bootstrap.Modal(document.getElementById("updateStatusModal"));

    $(document).on('click', '.update-status-btn', function () {
        $("#us_LabRequestItemId").val($(this).data("item-id"));
        $("#us_LabRequestId").val($(this).data("lab-request-id"));
        $("#us_LabTestId").val($(this).data("lab-test-id"));
        $("#us_PatientId").val($(this).data("patient-id"));

        const currentStatus = $(this).data("current-status");
        $("#us_NewStatus").val(currentStatus);

        updateStatusModal.show();
    });
    $("#btn-save-status").on("click", function () {
        const payload = {
            DoctorId: 0,  // controller will override
            LabRequestItemId: Number($("#us_LabRequestItemId").val()),
            LabRequestId: Number($("#us_LabRequestId").val()),
            LabTestId: Number($("#us_LabTestId").val()),
            PatientId: Number($("#us_PatientId").val()),
            NewStatus: $("#us_NewStatus").val()
        };

        $.ajax({
            url: "/DoctorPortal/UpdateLabRequestItemStatus",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(payload),
            success: function (res) {
                updateStatusModal.hide();
                showToast("Success", "Status updated successfully", "success");
                loadTestRequests(); // refresh grid
            },
            error: function (xhr) {
                showToast("Error", xhr.responseJSON?.message || "Failed to update status", "danger");
            }
        });
    });

});
