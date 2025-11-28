document.addEventListener('DOMContentLoaded', function () {
    // === AG Grid Column Definitions ===
    const columnDefs = [
        { headerName: "Ward Code", field: "wardCode", width: 120, sortable: true, filter: true },
        { headerName: "Ward Name", field: "wardName", flex: 1, sortable: true, filter: true },
        { headerName: "Ward Type", field: "wardType", width: 130, sortable: true, filter: true },
        { headerName: "Capacity", field: "capacity", width: 110, sortable: true, filter: true },
        { headerName: "Occupied", field: "occupiedBeds", width: 110, sortable: true, filter: true },
        {
            headerName: "Available",
            width: 110,
            valueGetter: params => {
                return params.data.capacity - params.data.occupiedBeds;
            },
            cellStyle: params => {
                const available = params.data.capacity - params.data.occupiedBeds;
                return available === 0 ? { color: '#ef4444', fontWeight: 'bold' } :
                    available < 5 ? { color: '#f59e0b', fontWeight: 'bold' } :
                        { color: '#10b981', fontWeight: 'bold' };
            }
        },
        { headerName: "Location", field: "location", flex: 1, sortable: true, filter: true },
        { headerName: "Description", field: "description", flex: 1, sortable: true, filter: true },
        {
            headerName: "Actions",
            width: 180,
            cellRenderer: params => {
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
            },
            sortable: false,
            filter: false
        }
    ];

    // === AG Grid Options ===
    const gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        pagination: true,
        paginationPageSize: 20,
        animateRows: true,
        rowHeight: 50,
        defaultColDef: {
            resizable: true,
            sortable: true,
            filter: true,
            minWidth: 100
        },
        onGridReady: function (params) {
            params.api.sizeColumnsToFit();
            loadWards();
        },
        onGridSizeChanged: function (params) {
            params.api.sizeColumnsToFit();
        },
        getRowNodeId: data => data.id,
        getContextMenuItems: function (params) {
            return [
                {
                    name: 'Edit Ward',
                    action: function () {
                        openEditModal(params.node.data);
                    },
                    icon: '<i class="fas fa-edit"></i>'
                },
                {
                    name: 'Delete Ward',
                    action: function () {
                        deleteWard(params.node.data.id, params.node.data.wardName);
                    },
                    icon: '<i class="fas fa-trash"></i>'
                },
                'separator',
                'copy',
                'export'
            ];
        }
    };

    // === Initialize AG Grid ===
    const gridDiv = document.querySelector('#wardGrid');
    new agGrid.Grid(gridDiv, gridOptions);

    // === Load Wards from Server ===
    function loadWards() {
        $.ajax({
            url: '/Ward/GetAll',
            type: 'GET',
            success: function (response) {
                console.log('Wards response:', response);
                if (response.success) {
                    gridOptions.api.setRowData(response.data);
                } else {
                    showToast(response.message || "Failed to load wards", 'error');
                }
            },
            error: function (xhr) {
                console.error('Error loading wards:', xhr);
                showToast("Error loading ward data", 'error');
            }
        });
    }

    // === Show Add Ward Modal ===
    $('#btnAddWard').on('click', function () {
        $('#wardForm')[0].reset();
        window.showModal('addWardModal');
    });

    // === Save New Ward ===
    $('#btnSaveWard').on('click', function () {
        const formData = {
            WardCode: $('input[name="WardCode"]').val(),
            WardName: $('input[name="WardName"]').val(),
            WardType: $('input[name="WardType"]').val(),
            Capacity: parseInt($('input[name="Capacity"]').val()),
            OccupiedBeds: parseInt($('input[name="OccupiedBeds"]').val()),
            Location: $('input[name="Location"]').val(),
            Description: $('textarea[name="Description"]').val(),
            IsActive: true,
            CreatedAt: new Date().toISOString()
        };

        $.ajax({
            url: '/Ward/Create',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (response) {
                if (response.success) {
                    window.hideModal('addWardModal');
                    loadWards();
                    showToast(response.message || 'Ward created successfully!', 'success');
                } else {
                    showToast('Error: ' + response.message, 'error');
                }
            },
            error: function (xhr) {
                console.error('Error saving ward:', xhr);
                showToast('An error occurred while saving the ward.', 'error');
            }
        });
    });

    // === Handle Edit Button Click ===
    $(document).on('click', '.edit-btn', function (e) {
        e.preventDefault();
        const wardId = parseInt($(this).data('id'));
        console.log('Edit clicked for ward ID:', wardId);

        let foundData = null;
        gridOptions.api.forEachNode(function (node) {
            if (node.data && node.data.id === wardId) {
                foundData = node.data;
            }
        });

        if (foundData) {
            openEditModal(foundData);
        } else {
            console.error('Could not find ward with ID:', wardId);
            showToast('Error: Ward not found', 'error');
        }
    });

    // === Handle Delete Button Click ===
    $(document).on('click', '.delete-btn', function (e) {
        e.preventDefault();
        const wardId = parseInt($(this).data('id'));
        console.log('Delete clicked for ward ID:', wardId);

        let foundData = null;
        gridOptions.api.forEachNode(function (node) {
            if (node.data && node.data.id === wardId) {
                foundData = node.data;
            }
        });

        if (foundData) {
            deleteWard(foundData.id, foundData.wardName);
        } else {
            console.error('Could not find ward with ID:', wardId);
            showToast('Error: Ward not found', 'error');
        }
    });

    // === Open Edit Modal and Prefill Fields ===
    function openEditModal(data) {
        console.log('Opening edit modal for:', data);

        $('#editWardId').val(data.id);
        $('#editWardCode').val(data.wardCode);
        $('#editWardName').val(data.wardName);
        $('#editWardType').val(data.wardType);
        $('#editCapacity').val(data.capacity);
        $('#editOccupiedBeds').val(data.occupiedBeds);
        $('#editLocation').val(data.location);
        $('#editDescription').val(data.description || '');

        window.showModal('editWardModal');
    }

    // === Handle Edit Form Submission ===
    $('#editWardForm').on('submit', function (e) {
        e.preventDefault();

        const ward = {
            Id: parseInt($('#editWardId').val()),
            WardCode: $('#editWardCode').val().trim(),
            WardName: $('#editWardName').val().trim(),
            WardType: $('#editWardType').val().trim(),
            Capacity: parseInt($('#editCapacity').val()),
            OccupiedBeds: parseInt($('#editOccupiedBeds').val()),
            Location: $('#editLocation').val().trim(),
            Description: $('#editDescription').val().trim()
        };

        console.log('Updating ward:', ward);

        const command = { Ward: ward };

        $.ajax({
            url: '/Ward/UpdateWard',
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(command),
            success: function (response) {
                if (response.success) {
                    window.hideModal('editWardModal');
                    loadWards();
                    showToast(response.message || 'Ward updated successfully!', 'success');
                } else {
                    showToast(response.message || 'Failed to update ward.', 'error');
                }
            },
            error: function (xhr) {
                console.error('Error updating ward:', xhr);
                if (xhr.status === 400 && xhr.responseJSON?.errors) {
                    let errorMsg = 'Validation Errors:\n';
                    xhr.responseJSON.errors.forEach(err => {
                        errorMsg += `- ${err.PropertyName}: ${err.ErrorMessage}\n`;
                    });
                    showToast(errorMsg, 'error');
                } else {
                    showToast('An unexpected error occurred while updating the ward.', 'error');
                }
            }
        });
    });

    // === Delete Ward Function ===
    function deleteWard(id, wardName) {
        if (!id || id <= 0) {
            showToast("Invalid Ward ID.", 'error');
            return;
        }

        if (!confirm(`Are you sure you want to delete "${wardName}"?`)) {
            return;
        }

        $.ajax({
            url: '/Ward/DeleteWard',
            type: 'DELETE',
            contentType: 'application/json',
            data: JSON.stringify({ Id: id }),
            success: function (response) {
                if (response.success) {
                    loadWards();
                    showToast(response.message || 'Ward deleted successfully!', 'success');
                } else {
                    showToast(response.message || 'Failed to delete the ward.', 'error');
                }
            },
            error: function (xhr) {
                console.error('Error deleting ward:', xhr);
                if (xhr.status === 400 && xhr.responseJSON?.errors) {
                    let errorMsg = 'Validation Errors:\n';
                    xhr.responseJSON.errors.forEach(err => {
                        errorMsg += `- ${err.PropertyName}: ${err.ErrorMessage}\n`;
                    });
                    showToast(errorMsg, 'error');
                } else {
                    showToast('An unexpected error occurred while deleting the ward.', 'error');
                }
            }
        });
    }

    // === Toast Notification Helper ===
    function showToast(message, type) {
        // Using toastr if available, otherwise fallback to alert
        if (typeof toastr !== 'undefined') {
            toastr[type](message);
        } else {
            alert(message);
        }
    }
});