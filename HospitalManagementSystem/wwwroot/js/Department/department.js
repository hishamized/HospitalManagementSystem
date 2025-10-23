$(document).ready(function () {
    // Show Add Department modal
    $('#btnAddDepartment').click(function () {
        $('#addDepartmentModal').modal('show');
        // Clear previous errors
        $('#nameError').text('');
        $('#descriptionError').text('');
        $('#formError').text('');
        $('#addDepartmentForm')[0].reset();
        $('#departmentName').removeClass('is-invalid');
        $('#departmentDescription').removeClass('is-invalid');
    });

    // Submit Add Department form via AJAX
    $('#addDepartmentForm').submit(function (e) {
        e.preventDefault();

        var dto = {
            Name: $('#departmentName').val(),
            Description: $('#departmentDescription').val()
        };

        $.ajax({
            url: '/Department/Add',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (response) {
                if (response.success) {
                    $('#addDepartmentModal').modal('hide');
                    loadDepartmentsGrid(); // refresh grid after adding
                } else {
                    if (response.errors) {
                        console.log(response.errors);
                        response.errors.forEach(function (err) {
                            if (err.PropertyName === "Name") {
                                $('#nameError').text(err.ErrorMessage).show();
                                $('#departmentName').addClass('is-invalid');
                            }
                            if (err.PropertyName === "Description") {
                                $('#descriptionError').text(err.ErrorMessage).show();
                                $('#departmentDescription').addClass('is-invalid');
                            }
                        });
                    } else {
                        $('#formError').text(response.message);
                        console.error(response.message);
                    }
                }
            },
            error: function (xhr, status, error) {
                $('#formError').text('An unexpected error occurred: ' + error);
                console.error('AJAX error:', xhr.responseText);
            }
        });
    });

    // AG Grid column definitions
    var columnDefs = [
        { headerName: "ID", field: "id", sortable: true, filter: true, resizable: true, checkboxSelection: true },
        { headerName: "Name", field: "name", sortable: true, filter: true, resizable: true },
        { headerName: "Description", field: "description", sortable: true, filter: true, resizable: true },
        {
            headerName: "Actions",
            field: "id",
            cellRenderer: function (params) {
                return `
                    <button class="btn btn-sm btn-primary btn-edit" data-id="${params.value}">Edit</button>
                    <button class="btn btn-sm btn-danger btn-delete" data-id="${params.value}">Delete</button>
                `;
            },
            sortable: false,
            filter: false,
            resizable: false,
            width: 150
        }
    ];

    // AG Grid options
    var gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        pagination: true,
        paginationPageSize: 10,
        defaultColDef: {
            flex: 1,
            minWidth: 100,
            sortable: true,
            filter: true,
            resizable: true
        },
        rowSelection: 'single',
        animateRows: true,
        getRowNodeId: data => data.id,
        onGridReady: function () {
            loadDepartmentsGrid();
        },
        getContextMenuItems: function (params) {
            var result = [
                {
                    name: 'Edit Department',
                    action: function () {
                        editDepartment(params.node.data);
                    }
                },
                {
                    name: 'Delete Department',
                    action: function () {
                        deleteDepartment(params.node.data);
                    }
                },
                'separator',
                'copy',
                'copyWithHeaders',
                'paste'
            ];
            return result;
        }
    };

    // Initialize AG Grid
    var eGridDiv = document.querySelector('#departmentsGrid');
    var grid = new agGrid.Grid(eGridDiv, gridOptions);

    // Load departments via AJAX GET
    function loadDepartmentsGrid() {
        $.ajax({
            url: '/Department/GetAll',
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    gridOptions.api.setRowData(response.data);
                } else {
                    console.error('Error loading departments:', response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error('AJAX error loading departments:', xhr.responseText);
            }
        });
    }

    // Edit department handler
    function editDepartment(data) {
        console.log('Editing department:', data);

        // Populate modal fields with row data (no server call)
        $('#editDepartmentId').val(data.id);
        $('#editDepartmentName').val(data.name);
        $('#editDepartmentDescription').val(data.description || '');

        // Clear old errors
        $('#editNameError').text('');
        $('#editDescriptionError').text('');
        $('#editFormError').text('');

        // Show modal
        $('#editDepartmentModal').modal('show');
    }
    // Handle Edit Department form submission
    $('#editDepartmentForm').submit(function (e) {
        e.preventDefault();

        var dto = {
            Id: $('#editDepartmentId').val(),
            Name: $('#editDepartmentName').val(),
            Description: $('#editDepartmentDescription').val()
        };

        $.ajax({
            url: '/Department/Edit',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (response) {
                if (response.success) {
                    // Close modal
                    $('#editDepartmentModal').modal('hide');

                    // Refresh AG Grid
                    loadDepartmentsGrid();

                    // Optional success toast
                    alert(response.message);
                } else {
                    // Display validation errors if any
                    if (response.errors && response.errors.length > 0) {
                        console.log('Validation errors:', response.errors);

                        response.errors.forEach(function (err) {
                            if (!err.propertyName) return; // safety check

                            // Normalize property name (take last segment after dot)
                            var prop = err.propertyName.includes('.') ? err.propertyName.split('.').pop() : err.propertyName;

                            if (prop === "Name") {
                                $('#editNameError').text(err.errorMessage).show();
                                $('#editDepartmentName').addClass('is-invalid');
                            }
                            if (prop === "Description") {
                                $('#editDescriptionError').text(err.errorMessage).show();
                                $('#editDepartmentDescription').addClass('is-invalid');
                            }
                        });
                    } else {
                        // Generic error
                        $('#editFormError').text(response.message).show();
                        console.error('Edit error:', response.message, response.details || '');
                    }
                }
            },
            error: function (xhr, status, error) {
                $('#editFormError').text('An unexpected error occurred: ' + error).show();
                console.error('AJAX error:', xhr.responseText);
            }
        });
    });


    // Delete department handler
    function deleteDepartment(data) {
        if (!data || !data.id) return;

        // Show confirmation
        if (!confirm(`Are you sure you want to delete department "${data.name}"?`)) return;

        $.ajax({
            url: '/Department/Delete',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data.id),
            success: function (response) {
                if (response.success) {
                    // Optional: show success toast or alert
                    alert(response.message);

                    // Refresh AG Grid
                    loadDepartmentsGrid();
                } else {
                    // Show error message
                    alert(response.message || 'Failed to delete department.');
                    console.error('Delete error:', response.details || '');
                }
            },
            error: function (xhr, status, error) {
                alert('An unexpected error occurred while deleting the department.');
                console.error('AJAX error:', xhr.responseText);
            }
        });
    }


    // Event delegation for Edit/Delete buttons inside grid
    $('#departmentsGrid').on('click', '.btn-edit', function () {
        var id = $(this).data('id');
        var rowData = gridOptions.api.getRowNode(id)?.data;
        if (rowData) editDepartment(rowData);
    });

    $('#departmentsGrid').on('click', '.btn-delete', function () {
        var id = $(this).data('id');
        var rowData = gridOptions.api.getRowNode(id)?.data;
        if (rowData) deleteDepartment(rowData);
    });
});
