document.addEventListener('DOMContentLoaded', function () {
    const columnDefs = [
        { headerName: "Ward Code", field: "wardCode", flex: 1 },
        { headerName: "Ward Name", field: "wardName", flex: 1 },
        { headerName: "Ward Type", field: "wardType", flex: 1 },
        { headerName: "Capacity", field: "capacity", width: 120 },
        { headerName: "Occupied Beds", field: "occupiedBeds", width: 150 },
        { headerName: "Location", field: "location", flex: 1 },
        { headerName: "Description", field: "description", flex: 1 },
        {
            headerName: "Actions",
            cellRenderer: params => {
                return `
                            <button class="btn btn-sm btn-warning me-2 edit-btn">Edit</button>
                            <button class="btn btn-sm btn-danger delete-btn">Delete</button>
                        `;
            },
            width: 160,
            cellStyle: { textAlign: "center" }
        }
    ];

    // ✅ Initialize AG Grid
    const gridOptions = {
        columnDefs,
        rowData: [],
        pagination: true,
        paginationPageSize: 10,
        defaultColDef: { resizable: true, sortable: true, filter: true },
        onGridReady: loadWards,
        onCellClicked: onCellClicked,
        onRowContextMenu: onRowContextMenu,
    };

    const gridDiv = document.querySelector('#wardGrid');
    new agGrid.Grid(gridDiv, gridOptions);

    // ✅ Handle edit/delete buttons in the grid
    function onCellClicked(params) {
        const rowData = params.data;

        if ($(params.event.target).hasClass('edit-btn')) {
            openEditModal(rowData);
        }

        if ($(params.event.target).hasClass('delete-btn')) {
            deleteWard(params.data.id, params.data.wardName);
        }
    }
    // ✅ Right-click context menu
    function onRowContextMenu(event) {
        event.event.preventDefault();
        const rowData = event.node.data;

        // Remove any existing menu
        $("#contextMenu").remove();

        // Build context menu
        const contextMenu = $(`
        <div id="contextMenu" class="card shadow-sm"
             style="position:fixed; top:${event.event.clientY}px; left:${event.event.clientX}px; z-index:99999; width:160px;">
            <div class="list-group list-group-flush">
                <button class="list-group-item list-group-item-action edit-context">✏️ Edit</button>
                <button class="list-group-item list-group-item-action text-danger delete-context">🗑️ Delete</button>
            </div>
        </div>
    `);

        $("body").append(contextMenu);

        // Remove menu when clicking outside
        $(document).one("click.context", function (e) {
            if (!$(e.target).closest("#contextMenu").length) {
                $("#contextMenu").remove();
            }
        });

        // Reuse same functions used by the column buttons
        $(".edit-context").on("click", function () {
            $("#contextMenu").remove();
            openEditModal(rowData); // same as edit button
        });

        $(".delete-context").on("click", function () {
            $("#contextMenu").remove();
            deleteWard(rowData.id, rowData.wardName);
        });

    }

    // 📥 Load wards (placeholder for backend)
    function loadWards() {
        $.ajax({
            url: '/Ward/GetAll',
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    gridOptions.api.setRowData(response.data);
                } else {
                    alert(response.message || "Failed to load data");
                }
            },
            error: function () {
                alert("Error loading ward data");
            }
        });
    }

    // Initialize data (you can connect this when GetAll is implemented)
    loadWards();

    // 🟢 SHOW MODAL
    $('#btnAddWard').on('click', function () {
        $('#wardForm')[0].reset();
        $('#addWardModal').modal('show');
    });

    // 🟣 SAVE WARD
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
                    alert(response.message);
                    $('#addWardModal').modal('hide');
                    loadWards();
                } else {
                    alert('Error: ' + response.message);
                }
            },
            error: function (xhr) {
                alert('An error occurred while saving the ward.');
            }
        });
    });

    // ✅ Open Edit Modal and prefill fields (no server call)
    function openEditModal(data) {
        $('#editWardId').val(data.id);
        $('#editWardCode').val(data.wardCode);
        $('#editWardName').val(data.wardName);
        $('#editWardType').val(data.wardType);
        $('#editCapacity').val(data.capacity);
        $('#editOccupiedBeds').val(data.occupiedBeds);
        $('#editLocation').val(data.location);
        $('#editDescription').val(data.description);
        $('#editWardModal').modal('show');
    }


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

        const command = { Ward: ward }; // ✅ wrap in Ward property

        $.ajax({
            url: '/Ward/UpdateWard',
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(command),
            success: function (response) {
                if (response.success) {
                    $('#editWardModal').modal('hide');
                    alert(response.message);
                    loadWards();
                } else {
                    alert(response.message || 'Failed to update ward.');
                }
            },
            error: function (xhr) {
                if (xhr.status === 400 && xhr.responseJSON?.errors) {
                    let errorMsg = 'Validation Errors:\n';
                    xhr.responseJSON.errors.forEach(err => {
                        errorMsg += `- ${err.PropertyName}: ${err.ErrorMessage}\n`;
                    });
                    alert(errorMsg);
                } else {
                    alert('An unexpected error occurred while updating the ward.');
                }
            }
        });
    });
    // ✅ Reusable delete function
    function deleteWard(id, wardName) {
        if (!id || id <= 0) {
            alert("Invalid Ward ID.");
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
                    alert(response.message);
                    loadWards(); // 🔄 refresh AG Grid data
                } else {
                    alert(response.message || 'Failed to delete the ward.');
                }
            },
            error: function (xhr) {
                if (xhr.status === 400 && xhr.responseJSON?.errors) {
                    let errorMsg = 'Validation Errors:\n';
                    xhr.responseJSON.errors.forEach(err => {
                        errorMsg += `- ${err.PropertyName}: ${err.ErrorMessage}\n`;
                    });
                    alert(errorMsg);
                } else {
                    alert('An unexpected error occurred while deleting the ward.');
                }
            }
        });
    }

});