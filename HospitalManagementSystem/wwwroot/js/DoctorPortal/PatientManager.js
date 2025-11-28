$(document).ready(function () {
    let gridApi;
    let gridColumnApi;

    // AG Grid column definitions with improved styling
    const columnDefs = [
        {
            headerName: 'Patient Code',
            field: 'patientCode',
            minWidth: 150,
            sortable: true,
            filter: true,
            cellClass: 'font-semibold text-purple-400'
        },
        {
            headerName: 'Full Name',
            field: 'fullName',
            minWidth: 200,
            sortable: true,
            filter: true,
            cellClass: 'font-medium'
        },
        {
            headerName: 'Email',
            field: 'email',
            minWidth: 250,
            cellRenderer: params => `<i class="fas fa-envelope mr-2 text-blue-400"></i><span class="text-gray-300">${params.value}</span>`,
            cellClass: 'flex items-center'
        },
        {
            headerName: 'Contact',
            field: 'contactNumber',
            minWidth: 170,
            cellRenderer: params => `<i class="fas fa-phone mr-2 text-green-400"></i><span class="text-gray-300">${params.value}</span>`,
            cellClass: 'flex items-center'
        },
        {
            headerName: 'Blood Group',
            field: 'bloodGroup',
            maxWidth: 130,
            cellRenderer: params => `<span class="inline-flex items-center rounded-full bg-red-600 px-3 py-1 text-xs font-semibold text-white">${params.value}</span>`,
            cellClass: 'flex items-center justify-center'
        },
        {
            headerName: 'Emergency Contact',
            field: 'emergencyContactNumber',
            minWidth: 180,
            cellRenderer: params => `<i class="fas fa-phone-volume mr-2 text-yellow-400"></i><span class="text-gray-300">${params.value}</span>`,
            cellClass: 'flex items-center'
        },
        {
            headerName: 'Address',
            field: 'address',
            minWidth: 300,
            cellRenderer: params => `<i class="fas fa-map-marker-alt mr-2 text-gray-400"></i><span class="text-gray-300">${params.value || 'N/A'}</span>`,
            cellClass: 'flex items-center'
        },
        {
            headerName: 'Actions',
            field: 'patientId',
            pinned: 'right',
            minWidth: 150,
            maxWidth: 150,
            sortable: false,
            filter: false,
            cellRenderer: params => `
                <form method="post" action="/DoctorPortal/ViewPatient" class="inline-block">
                    <input type="hidden" name="patientId" value="${params.value}" />
                    <input name="__RequestVerificationToken" type="hidden" value="${window.__RequestVerificationToken}" />
                    <button type="submit" 
                            class="inline-flex items-center gap-1.5 rounded-lg bg-purple-600 px-3 py-1.5 text-sm font-medium text-white transition-all duration-200 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-purple-500 focus:ring-offset-2 focus:ring-offset-gray-800">
                        <i class="fas fa-eye"></i>
                        <span>View</span>
                    </button>
                </form>`,
            cellClass: 'flex items-center justify-center'
        }
    ];

    // AG Grid options with improved settings
    const gridOptions = {
        columnDefs: columnDefs,
        defaultColDef: {
            resizable: true,
            sortable: true,
            filter: true,
            floatingFilter: true, // Enable column filters
            flex: 1, // Auto-size columns
        },
        rowSelection: 'single',
        animateRows: true,
        pagination: true,
        paginationPageSize: 20,
        paginationPageSizeSelector: [10, 20, 50, 100],
        domLayout: 'normal', // Keep normal for scrolling
        enableCellTextSelection: true,
        suppressRowClickSelection: true,
        rowHeight: 50, // Increased row height for better readability
        headerHeight: 56,
        getContextMenuItems: getContextMenuItems,
        overlayLoadingTemplate: '<span class="ag-overlay-loading-center">Loading patients...</span>',
        overlayNoRowsTemplate: '<span class="ag-overlay-no-rows-center">No patients found</span>',
        onGridReady: function (params) {
            gridApi = params.api;
            gridColumnApi = params.columnApi;
            loadPatientsData();
        },
        onFirstDataRendered: function (params) {
            params.api.sizeColumnsToFit();
        },
        // Excel export configuration
        defaultExcelExportParams: {
            fileName: `patients_list_${new Date().toISOString().split('T')[0]}.xlsx`,
            sheetName: 'Patients',
            author: 'Hospital Management System',
            headerRowHeight: 30,
            rowHeight: 25,
        },
        // CSV export configuration
        defaultCsvExportParams: {
            fileName: `patients_list_${new Date().toISOString().split('T')[0]}.csv`,
            columnSeparator: ','
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
                    name: 'View Patient Details',
                    icon: '<i class="fas fa-eye text-purple-500"></i>',
                    action: function () {
                        viewPatient(patientId);
                    }
                },
                'separator',
                {
                    name: 'Copy Patient Code',
                    icon: '<i class="fas fa-copy text-blue-500"></i>',
                    action: function () {
                        navigator.clipboard.writeText(params.node.data.patientCode);
                        showToast('Copied', 'Patient code copied to clipboard', 'info');
                    }
                },
                'separator',
                'copy',
                'copyWithHeaders',
                'separator',
                {
                    name: 'Export Selection',
                    icon: '<i class="fas fa-file-export text-green-500"></i>',
                    subMenu: [
                        {
                            name: 'Export as Excel',
                            action: function () { exportToExcel(); }
                        },
                        {
                            name: 'Export as CSV',
                            action: function () { exportToCsv(); }
                        }
                    ]
                }
            ];
        }
        return ['copy', 'copyWithHeaders', 'export'];
    }

    // View patient function
    function viewPatient(patientId) {
        const form = document.createElement('form');
        form.method = 'POST';
        form.action = '/DoctorPortal/ViewPatient';

        const patientIdInput = document.createElement('input');
        patientIdInput.type = 'hidden';
        patientIdInput.name = 'patientId';
        patientIdInput.value = patientId;

        const tokenInput = document.createElement('input');
        tokenInput.type = 'hidden';
        tokenInput.name = '__RequestVerificationToken';
        tokenInput.value = window.__RequestVerificationToken;

        form.appendChild(patientIdInput);
        form.appendChild(tokenInput);
        document.body.appendChild(form);
        form.submit();
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
                if (response.success && response.data && response.data.length > 0) {
                    gridApi.setRowData(response.data);
                    gridApi.hideOverlay();

                    // Show success message
                    showToast('Success', `Loaded ${response.data.length} patient(s) successfully`, 'success');
                } else {
                    gridApi.setRowData([]);
                    gridApi.showNoRowsOverlay();
                    showToast('Info', 'No patients assigned to you', 'info');
                }
            },
            error: function (xhr, status, error) {
                gridApi.setRowData([]);
                gridApi.showNoRowsOverlay();

                let errorMessage = 'Failed to load patients';
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                } else if (error) {
                    errorMessage += ': ' + error;
                }

                showToast('Error', errorMessage, 'danger');
                console.error('Error loading patients:', { xhr, status, error });
            }
        });
    }

    // Enhanced toast notification helper with Tailwind styling
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
                <i class="fas ${icon} text-xl mt-0.5"></i>
                <div class="flex-1">
                    <strong class="font-semibold">${title}</strong>
                    <p class="text-sm mt-1 opacity-90">${message}</p>
                </div>
                <button type="button" class="toast-close ml-2 text-white hover:text-gray-200 transition-colors" aria-label="Close">
                    <i class="fas fa-times"></i>
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

    // Export to Excel function (FIXED)
    window.exportToExcel = function () {
        try {
            const fileName = `patients_list_${new Date().toISOString().split('T')[0]}.xlsx`;

            gridApi.exportDataAsExcel({
                fileName: fileName,
                sheetName: 'Patients',
                author: 'Hospital Management System',
                columnKeys: ['patientCode', 'fullName', 'email', 'contactNumber', 'bloodGroup', 'emergencyContactNumber', 'address'],
                processCellCallback: function (params) {
                    // Remove HTML tags for clean export
                    if (params.value && typeof params.value === 'string') {
                        return params.value.replace(/<[^>]*>/g, '');
                    }
                    return params.value;
                }
            });

            showToast('Success', 'Excel file exported successfully', 'success');
        } catch (error) {
            console.error('Excel export error:', error);
            showToast('Error', 'Failed to export Excel file: ' + error.message, 'danger');
        }
    };

    // Export to CSV function
    window.exportToCsv = function () {
        try {
            const fileName = `patients_list_${new Date().toISOString().split('T')[0]}.csv`;

            gridApi.exportDataAsCsv({
                fileName: fileName,
                columnKeys: ['patientCode', 'fullName', 'email', 'contactNumber', 'bloodGroup', 'emergencyContactNumber', 'address'],
                processCellCallback: function (params) {
                    // Remove HTML tags for clean export
                    if (params.value && typeof params.value === 'string') {
                        return params.value.replace(/<[^>]*>/g, '');
                    }
                    return params.value;
                }
            });

            showToast('Success', 'CSV file exported successfully', 'success');
        } catch (error) {
            console.error('CSV export error:', error);
            showToast('Error', 'Failed to export CSV file: ' + error.message, 'danger');
        }
    };

    // Add CSS animations for toasts
    const style = document.createElement('style');
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

        .ag-theme-alpine-dark .ag-header-cell-label {
            font-weight: 600;
            color: #a78bfa !important;
        }

        .ag-theme-alpine-dark .ag-cell {
            display: flex;
            align-items: center;
        }
    `;
    document.head.appendChild(style);
});