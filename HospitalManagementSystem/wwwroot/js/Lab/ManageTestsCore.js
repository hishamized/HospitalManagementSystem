$(document).ready(function () {
    // Set CreatedAt to current UTC time when page loads
    $('#createdAt').val(new Date().toISOString());

    // ========================================
    // AG GRID CONFIGURATION
    // ========================================

    // Column Definitions
    const columnDefs = [
        {
            headerName: "ID",
            field: "id",
            sortable: true,
            filter: "agNumberColumnFilter",
            resizable: true,
            width: 90,
            checkboxSelection: true,
            headerCheckboxSelection: true
        },
        {
            headerName: "Test Name",
            field: "testName",
            sortable: true,
            filter: "agTextColumnFilter",
            resizable: true,
            flex: 1
        },
        {
            headerName: "Sample Type",
            field: "sampleType",
            sortable: true,
            filter: "agTextColumnFilter",
            resizable: true,
            flex: 1,
            cellRenderer: function (params) {
                return params.value || '<span class="text-muted">N/A</span>';
            }
        },
        {
            headerName: "Normal Range",
            field: "normalRange",
            sortable: true,
            filter: "agTextColumnFilter",
            resizable: true,
            flex: 1
        },
        {
            headerName: "Price",
            field: "price",
            sortable: true,
            filter: "agNumberColumnFilter",
            resizable: true,
            width: 120,
            cellRenderer: function (params) {
                if (params.value !== null && params.value !== undefined) {
                    return '$' + parseFloat(params.value).toFixed(2);
                }
                return '<span class="text-muted">N/A</span>';
            }
        },
        {
            headerName: "Description",
            field: "description",
            sortable: true,
            filter: "agTextColumnFilter",
            resizable: true,
            flex: 2,
            cellRenderer: function (params) {
                if (params.value) {
                    const truncated = params.value.length > 50
                        ? params.value.substring(0, 50) + '...'
                        : params.value;
                    return `<span title="${params.value}">${truncated}</span>`;
                }
                return '<span class="text-muted">N/A</span>';
            }
        },
        {
            headerName: "Status",
            field: "isActive",
            sortable: true,
            filter: true,
            resizable: true,
            width: 120,
            cellRenderer: function (params) {
                if (params.value) {
                    return '<span class="badge bg-success">Active</span>';
                } else {
                    return '<span class="badge bg-secondary">Inactive</span>';
                }
            }
        },
        {
            headerName: "Created At",
            field: "createdAt",
            sortable: true,
            filter: "agDateColumnFilter",
            resizable: true,
            width: 180,
            cellRenderer: function (params) {
                if (params.value) {
                    const date = new Date(params.value);
                    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
                }
                return '<span class="text-muted">N/A</span>';
            }
        },
        {
            headerName: "Updated At",
            field: "updatedAt",
            sortable: true,
            filter: "agDateColumnFilter",
            resizable: true,
            width: 180,
            cellRenderer: function (params) {
                if (params.value) {
                    const date = new Date(params.value);
                    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
                }
                return '<span class="text-muted">N/A</span>';
            }
        },
        {
            headerName: "Actions",
            field: "id",
            cellRenderer: function (params) {
                return `
                    <button class="btn btn-sm btn-warning me-1 edit-btn" data-id="${params.value}" data-testname="${params.data.testName}">
                        <i class="mdi mdi-pencil"></i> Edit
                    </button>
                    <button class="btn btn-sm btn-danger delete-btn" data-id="${params.value}" data-testname="${params.data.testName}">
                        <i class="mdi mdi-delete"></i> Delete
                    </button>
                `;
            },
            sortable: false,
            filter: false,
            resizable: false,
            width: 200,
            suppressMenu: true
        }
    ];

    // Grid Options
    const gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        pagination: true,
        paginationPageSize: 10,
        paginationPageSizeSelector: [10, 20, 50, 100],
        animateRows: true,
        rowSelection: 'multiple',
        getRowNodeId: data => data.id,
        defaultColDef: {
            flex: 1,
            minWidth: 100,
            sortable: true,
            filter: true,
            resizable: true
        },
        onGridReady: function () {
            loadLabTests();
        },
        // Context Menu (Right Click)
        getContextMenuItems: function (params) {
            const result = [
                {
                    name: 'Edit Test',
                    icon: '<i class="mdi mdi-pencil text-warning"></i>',
                    action: function () {
                        handleEdit(params.node.data.id, params.node.data.testName);
                    }
                },
                {
                    name: 'Delete Test',
                    icon: '<i class="mdi mdi-delete text-danger"></i>',
                    action: function () {
                        handleDelete(params.node.data.id, params.node.data.testName);
                    }
                },
                'separator',
                'copy',
                'copyWithHeaders',
                'export'
            ];
            return result;
        }
    };

    // Initialize AG Grid
    const gridDiv = document.querySelector('#labTestsGrid');
    new agGrid.Grid(gridDiv, gridOptions);

    // ========================================
    // LOAD LAB TESTS DATA
    // ========================================
    function loadLabTests() {
        $.ajax({
            url: '/Lab/FetchLabTests',
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    gridOptions.api.setRowData(response.result);
                    console.log(response);
                } else {
                    console.error('Error loading lab tests:', response.message);
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.message || 'Failed to load lab tests'
                    });
                }
            },
            error: function (xhr, status, error) {
                console.error('AJAX error loading lab tests:', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'An error occurred while loading lab tests'
                });
            }
        });
    }

    // ========================================
    // EDIT HANDLER (PLACEHOLDER)
    // ========================================
    function handleEdit(id, testName) {
        Swal.fire({
            icon: 'info',
            title: 'Edit Lab Test',
            html: `You clicked Edit for:<br><strong>ID: ${id}</strong><br><strong>Test: ${testName}</strong>`,
            confirmButtonText: 'OK'
        });
        console.log('Edit clicked for ID:', id, 'Test:', testName);
        // TODO: Implement edit functionality later
    }

    // ========================================
    // DELETE HANDLER (PLACEHOLDER)
    // ========================================
    function handleDelete(id, testName) {
        Swal.fire({
            icon: 'warning',
            title: 'Delete Lab Test',
            html: `You clicked Delete for:<br><strong>ID: ${id}</strong><br><strong>Test: ${testName}</strong>`,
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'Cancel',
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6'
        }).then((result) => {
            if (result.isConfirmed) {
                console.log('Delete confirmed for ID:', id);
                // TODO: Implement delete functionality later
                Swal.fire({
                    icon: 'info',
                    title: 'Delete Action',
                    text: 'Delete functionality will be implemented later',
                    timer: 2000,
                    showConfirmButton: false
                });
            }
        });
    }

    // ========================================
    // BUTTON EVENT HANDLERS
    // ========================================

    // Edit button click handler
    $(document).on('click', '.edit-btn', function () {
        const id = $(this).data('id');
        const testName = $(this).data('testname');
        handleEdit(id, testName);
    });

    // Delete button click handler
    $(document).on('click', '.delete-btn', function () {
        const id = $(this).data('id');
        const testName = $(this).data('testname');
        handleDelete(id, testName);
    });

    // ========================================
    // ADD LAB TEST MODAL
    // ========================================

    // Open modal when Add Test button is clicked
    $('#addTestBtn').on('click', function () {
        // Reset form before opening
        $('#addLabTestForm')[0].reset();
        $('#addLabTestForm').removeClass('was-validated');
        $('#addLabTestForm').find('.is-invalid').removeClass('is-invalid');
        $('#addLabTestForm').find('.is-valid').removeClass('is-valid');

        // Set CreatedAt to current time
        $('#createdAt').val(new Date().toISOString());

        // Re-check the IsActive checkbox
        $('#isActive').prop('checked', true);

        // Open the modal
        $('#addLabTestModal').modal('show');
    });

    // ========================================
    // HANDLE FORM SUBMISSION
    // ========================================
    $('#addLabTestForm').on('submit', function (e) {
        e.preventDefault();

        // Remove previous validation states
        $(this).find('.is-invalid').removeClass('is-invalid');
        $(this).find('.is-valid').removeClass('is-valid');

        // Get the form
        const form = this;

        // Check HTML5 validation
        if (!form.checkValidity()) {
            e.stopPropagation();
            $(form).addClass('was-validated');
            return;
        }

        // Get the anti-forgery token
        const token = $('input[name="__RequestVerificationToken"]').val();

        // Prepare form data
        const formData = {
            TestName: $('#testName').val().trim(),
            Description: $('#description').val().trim() || null,
            SampleType: $('#sampleType').val() || null,
            NormalRange: $('#normalRange').val().trim(),
            Price: $('#price').val() ? parseFloat($('#price').val()) : null,
            CreatedAt: $('#createdAt').val(),
            UpdatedAt: null,
            IsActive: $('#isActive').is(':checked')
        };

        // Disable submit button and show loading state
        const $submitBtn = $('#submitBtn');
        const originalBtnText = $submitBtn.html();
        $submitBtn.prop('disabled', true).html('<i class="mdi mdi-loading mdi-spin"></i> Saving...');

        // Send AJAX request
        $.ajax({
            url: '/Lab/AddLabTest',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            headers: {
                'RequestVerificationToken': token
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader('RequestVerificationToken', token);
            },
            success: function (response) {
                // Re-enable button
                $submitBtn.prop('disabled', false).html(originalBtnText);

                if (response.success) {
                    // Hide the modal
                    $('#addLabTestModal').modal('hide');

                    // Show success message
                    Swal.fire({
                        icon: 'success',
                        title: 'Success!',
                        text: 'Lab test added successfully.',
                        timer: 2000,
                        showConfirmButton: false
                    });

                    // Reset form
                    $('#addLabTestForm')[0].reset();
                    $('#addLabTestForm').removeClass('was-validated');

                    // Reset CreatedAt to new time
                    $('#createdAt').val(new Date().toISOString());

                    // Re-check the IsActive checkbox
                    $('#isActive').prop('checked', true);

                    // Refresh AG Grid
                    loadLabTests();
                } else {
                    // Show error message from server
                    Swal.fire({
                        icon: 'error',
                        title: 'Failed',
                        text: response.message || 'Failed to add lab test. Please try again.'
                    });
                }
            },
            error: function (xhr, status, error) {
                // Re-enable button
                $submitBtn.prop('disabled', false).html(originalBtnText);

                // Parse error response
                let errorMessage = 'An error occurred while adding the lab test.';

                if (xhr.responseJSON) {
                    if (xhr.responseJSON.message) {
                        errorMessage = xhr.responseJSON.message;
                    } else if (xhr.responseJSON.errors) {
                        // Handle validation errors
                        const errors = xhr.responseJSON.errors;
                        errorMessage = Object.values(errors).flat().join('<br>');
                    }
                } else if (xhr.responseText) {
                    errorMessage = xhr.responseText;
                }

                // Show error message
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    html: errorMessage,
                    confirmButtonText: 'OK'
                });

                // Log error for debugging
                console.error('Error adding lab test:', {
                    status: xhr.status,
                    statusText: xhr.statusText,
                    response: xhr.responseJSON || xhr.responseText,
                    error: error
                });
            }
        });
    });

    // ========================================
    // FORM VALIDATION & HELPERS
    // ========================================

    // Real-time validation feedback
    $('#testName, #normalRange').on('blur', function () {
        if ($(this).val().trim()) {
            $(this).removeClass('is-invalid').addClass('is-valid');
        } else {
            $(this).removeClass('is-valid').addClass('is-invalid');
        }
    });

    // Format price input
    $('#price').on('blur', function () {
        const value = parseFloat($(this).val());
        if (!isNaN(value)) {
            $(this).val(value.toFixed(2));
        }
    });

    // Reset form when modal is closed
    $('#addLabTestModal').on('hidden.bs.modal', function () {
        $('#addLabTestForm')[0].reset();
        $('#addLabTestForm').removeClass('was-validated');
        $('#addLabTestForm').find('.is-invalid').removeClass('is-invalid');
        $('#addLabTestForm').find('.is-valid').removeClass('is-valid');
        $('#submitBtn').prop('disabled', false).html('<i class="mdi mdi-content-save"></i> Save Lab Test');
    });

    // ========================================
    // EXPOSE REFRESH FUNCTION GLOBALLY
    // ========================================
    window.refreshLabTestsGrid = loadLabTests;
});