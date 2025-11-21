$(document).ready(function () {
    let gridApi;
    let gridColumnApi;

    // AG Grid column definitions
    const columnDefs = [
        { headerName: 'Patient Code', field: 'patientCode', minWidth: 150, sortable: true, filter: true },
        { headerName: 'Full Name', field: 'fullName', minWidth: 200, sortable: true, filter: true },
        {
            headerName: 'Email',
            field: 'email',
            minWidth: 250,
            cellRenderer: params => `<i class="bi bi-envelope me-1 text-primary"></i>${params.value}`
        },
        {
            headerName: 'Contact',
            field: 'contactNumber',
            minWidth: 170,
            cellRenderer: params => `<i class="bi bi-telephone me-1 text-primary"></i>${params.value}`
        },
        {
            headerName: 'Blood Group',
            field: 'bloodGroup',
            maxWidth: 120,
            cellRenderer: params => `<span class="badge bg-danger">${params.value}</span>`
        },
        {
            headerName: 'Emergency Contact',
            field: 'emergencyContactNumber',
            minWidth: 180,
            cellRenderer: params => `<i class="bi bi-phone-vibrate me-1 text-warning"></i>${params.value}`
        },
        {
            headerName: 'Address',
            field: 'address',
            minWidth: 300,
            cellRenderer: params => `<i class="bi bi-geo-alt me-1 text-muted"></i>${params.value}`
        },
        {
            headerName: 'Actions',
            field: 'patientId',
            pinned: 'right',
            maxWidth: 160,
            cellRenderer: params => `
            <form method="post" action="/DoctorPortal/ViewPatient">
                <input type="hidden" name="patientId" value="${params.value}" />
                <input name="__RequestVerificationToken" type="hidden" value="${window.__RequestVerificationToken}" />
                <button type="submit" class="btn btn-primary btn-sm">
                    <i class="bi bi-eye"></i> View
                </button>
            </form>`
        }
    ];


    // AG Grid options
    const gridOptions = {
        columnDefs: columnDefs,
        defaultColDef: {
            resizable: true,
            sortable: true,
            filter: true,
        },
        rowSelection: 'single',
        animateRows: true,
        pagination: true,
        paginationPageSize: 20,
        paginationPageSizeSelector: [10, 20, 50, 100],
        domLayout: 'normal',
        enableCellTextSelection: true,
        suppressRowClickSelection: true,
        getContextMenuItems: getContextMenuItems,
        onGridReady: function (params) {
            gridApi = params.api;
            gridColumnApi = params.columnApi;
            loadPatientsData();
        },
        onFirstDataRendered: function (params) {
            params.api.sizeColumnsToFit();
        }
    };

    // Initialize AG Grid
    const gridDiv = document.querySelector('#patientGrid');
    new agGrid.Grid(gridDiv, gridOptions);

    // Context menu items
    function getContextMenuItems(params) {
        if (params.node) {
            const patientId = params.node.data.patientId;
            return [
                {
                    name: 'View Patient',
                    icon: '<i class="bi bi-eye"></i>',
                    action: function () {
                        viewPatient(patientId);
                    }
                },
                'separator',
                'copy',
                'copyWithHeaders',
                'export'
            ];
        }
        return ['copy', 'copyWithHeaders', 'export'];
    }

    // Load patients data via AJAX
    function loadPatientsData() {
        $.ajax({
            url: '/DoctorPortal/FetchPatientsByDoctor',
            type: 'GET',
            dataType: 'json',
            beforeSend: function () {
                gridApi.showLoadingOverlay();
            },
            success: function (response) {
                if (response.success && response.data) {
                    gridApi.setRowData(response.data);
                    gridApi.hideOverlay();

                    // Show success message
                    showToast('Success', `Loaded ${response.data.length} patients`, 'success');
                } else {
                    gridApi.showNoRowsOverlay();
                    showToast('Warning', 'No patients found', 'warning');
                }
            },
            error: function (xhr, status, error) {
                gridApi.showNoRowsOverlay();
                showToast('Error', 'Failed to load patients: ' + error, 'danger');
                console.error('Error loading patients:', error);
            }
        });
    }

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

    // Export functionality (optional enhancement)
    window.exportToExcel = function () {
        gridApi.exportDataAsExcel({
            fileName: 'patients_list.xlsx',
            sheetName: 'Patients'
        });
    };

    window.exportToCsv = function () {
        gridApi.exportDataAsCsv({
            fileName: 'patients_list.csv'
        });
    };
});