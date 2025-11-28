$(document).ready(function () {
    // Show Add Department modal
    $('#btnAddDepartment').click(function () {
        window.showModal('addDepartmentModal');
        // Clear previous errors
        $('#nameError').addClass('hidden').text('');
        $('#descriptionError').addClass('hidden').text('');
        $('#formError').addClass('hidden').text('');
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
                    window.hideModal('addDepartmentModal');
                    loadDepartmentsGrid();
                    toastr.success('Department added successfully!');
                } else {
                    if (response.errors) {
                        console.log(response.errors);
                        response.errors.forEach(function (err) {
                            if (err.PropertyName === "Name") {
                                $('#nameError').removeClass('hidden').text(err.ErrorMessage);
                                $('#departmentName').addClass('is-invalid');
                            }
                            if (err.PropertyName === "Description") {
                                $('#descriptionError').removeClass('hidden').text(err.ErrorMessage);
                                $('#departmentDescription').addClass('is-invalid');
                            }
                        });
                    } else {
                        $('#formError').removeClass('hidden').text(response.message);
                        console.error(response.message);
                    }
                }
            },
            error: function (xhr, status, error) {
                $('#formError').removeClass('hidden').text('An unexpected error occurred: ' + error);
                console.error('AJAX error:', xhr.responseText);
            }
        });
    });

    // AG Grid column definitions
    var columnDefs = [
        { headerName: "ID", field: "id", sortable: true, filter: true, resizable: true, width: 100 },
        { headerName: "Name", field: "name", sortable: true, filter: true, resizable: true, flex: 1 },
        { headerName: "Description", field: "description", sortable: true, filter: true, resizable: true, flex: 1 },
        {
            headerName: 'Actions',
            width: 180,
            cellRenderer: (params) => {
                return `
                    <div class="flex gap-2">
                        <button 
                            class="px-3 py-1 text-sm rounded-lg bg-purple-600 hover:bg-purple-700 text-white transition edit-btn"
                            data-id="${params.data.id}">
                            <i class="fas fa-edit"></i> Edit
                        </button>
                        <button 
                            class="px-3 py-1 text-sm rounded-lg bg-red-600 hover:bg-red-700 text-white transition delete-btn"
                            data-id="${params.data.id}">
                            <i class="fas fa-trash"></i> Delete
                        </button>
                    </div>
                `;
            }
        }
    ];

    // AG Grid options
    var gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        pagination: true,
        paginationPageSize: 10,
        defaultColDef: {
            minWidth: 100,
            sortable: true,
            filter: true,
            resizable: true
        },
        rowSelection: 'single',
        animateRows: true,
        rowHeight: 50,
        onGridReady: function (params) {
            params.api.sizeColumnsToFit();
            loadDepartmentsGrid();
        },
        onGridSizeChanged: function (params) {
            params.api.sizeColumnsToFit();
        },
        getRowNodeId: data => data.id,
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
                console.log('Departments response:', response);
                if (response.success) {
                    gridOptions.api.setRowData(response.data);
                } else {
                    console.error('Error loading departments:', response.message);
                    toastr.error('Failed to load departments');
                }
            },
            error: function (xhr, status, error) {
                console.error('AJAX error loading departments:', xhr.responseText);
                toastr.error('Error loading departments');
            }
        });
    }

    // Edit department handler
    function editDepartment(data) {
        console.log('Editing department:', data);

        // Populate modal fields with row data
        $('#editDepartmentId').val(data.id);
        $('#editDepartmentName').val(data.name);
        $('#editDepartmentDescription').val(data.description || '');

        // Clear old errors
        $('#editNameError').addClass('hidden').text('');
        $('#editDescriptionError').addClass('hidden').text('');
        $('#editFormError').addClass('hidden').text('');
        $('#editDepartmentName').removeClass('is-invalid');
        $('#editDepartmentDescription').removeClass('is-invalid');

        // Show modal
        window.showModal('editDepartmentModal');
    }

    // Handle Edit Department form submission
    $('#editDepartmentForm').submit(function (e) {
        e.preventDefault();

        var dto = {
            Id: parseInt($('#editDepartmentId').val()),
            Name: $('#editDepartmentName').val(),
            Description: $('#editDepartmentDescription').val()
        };

        console.log('Submitting edit:', dto);

        $.ajax({
            url: '/Department/Edit',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (response) {
                if (response.success) {
                    window.hideModal('editDepartmentModal');
                    loadDepartmentsGrid();
                    toastr.success('Department updated successfully!');
                } else {
                    if (response.errors && response.errors.length > 0) {
                        console.log('Validation errors:', response.errors);

                        response.errors.forEach(function (err) {
                            if (!err.propertyName) return;

                            var prop = err.propertyName.includes('.') ? err.propertyName.split('.').pop() : err.propertyName;

                            if (prop === "Name") {
                                $('#editNameError').removeClass('hidden').text(err.errorMessage);
                                $('#editDepartmentName').addClass('is-invalid');
                            }
                            if (prop === "Description") {
                                $('#editDescriptionError').removeClass('hidden').text(err.errorMessage);
                                $('#editDepartmentDescription').addClass('is-invalid');
                            }
                        });
                    } else {
                        $('#editFormError').removeClass('hidden').text(response.message);
                        console.error('Edit error:', response.message, response.details || '');
                    }
                }
            },
            error: function (xhr, status, error) {
                $('#editFormError').removeClass('hidden').text('An unexpected error occurred: ' + error);
                console.error('AJAX error:', xhr.responseText);
            }
        });
    });

    // Delete department handler
    function deleteDepartment(data) {
        if (!data || !data.id) return;

        if (!confirm(`Are you sure you want to delete department "${data.name}"?`)) return;

        $.ajax({
            url: '/Department/Delete',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data.id),
            success: function (response) {
                if (response.success) {
                    toastr.success('Department deleted successfully!');
                    loadDepartmentsGrid();
                } else {
                    toastr.error(response.message || 'Failed to delete department.');
                    console.error('Delete error:', response.details || '');
                }
            },
            error: function (xhr, status, error) {
                toastr.error('An unexpected error occurred while deleting the department.');
                console.error('AJAX error:', xhr.responseText);
            }
        });
    }

    // Event delegation for Edit/Delete buttons inside grid - FIXED
    $(document).on('click', '.edit-btn', function (e) {
        e.preventDefault();
        const id = parseInt($(this).data('id'));
        console.log('Edit clicked for ID:', id);

        // Find the row data by iterating through all rows
        let foundData = null;
        gridOptions.api.forEachNode(function (node) {
            if (node.data && node.data.id === id) {
                foundData = node.data;
            }
        });

        if (foundData) {
            console.log('Found data:', foundData);
            editDepartment(foundData);
        } else {
            console.error('Could not find department with ID:', id);
        }
    });

    $(document).on('click', '.delete-btn', function (e) {
        e.preventDefault();
        const id = parseInt($(this).data('id'));
        console.log('Delete clicked for ID:', id);

        // Find the row data by iterating through all rows
        let foundData = null;
        gridOptions.api.forEachNode(function (node) {
            if (node.data && node.data.id === id) {
                foundData = node.data;
            }
        });

        if (foundData) {
            console.log('Found data:', foundData);
            deleteDepartment(foundData);
        } else {
            console.error('Could not find department with ID:', id);
        }
    });
});