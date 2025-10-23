$(document).ready(function () {

    // Helper: Convert bitmask to day names
    function getDaysOfWeekText(bitmask) {
        const days = [
            { value: 1, name: 'Sunday' },
            { value: 2, name: 'Monday' },
            { value: 4, name: 'Tuesday' },
            { value: 8, name: 'Wednesday' },
            { value: 16, name: 'Thursday' },
            { value: 32, name: 'Friday' },
            { value: 64, name: 'Saturday' }
        ];
        return days.filter(d => (bitmask & d.value) !== 0).map(d => d.name).join(', ');
    }

    // AG Grid column definitions
    var columnDefs = [
        { headerName: "ID", field: "id", checkboxSelection: true },
        { headerName: "Reporting Time", field: "reportingTime" },
        { headerName: "Leaving Time", field: "leavingTime" },
        {
            headerName: "Days Of Week",
            field: "daysOfWeek",
            valueFormatter: function (params) {
                return getDaysOfWeekText(params.value);
            }
        },
        {
            headerName: "Actions",
            field: "actions",
            cellRenderer: function (params) {
                // Add data-row-id to buttons
                return `
                    <button class="btn btn-sm btn-primary edit-btn" data-row-id="${params.data.id}">Edit</button>
                    <button class="btn btn-sm btn-danger delete-btn" data-row-id="${params.data.id}">Delete</button>
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
            loadSlotsGrid();
        },
        getContextMenuItems: function (params) {
            return [
                {
                    name: 'Edit Slot',
                    action: function () {
                        openEditModal(params.node.data);
                    }
                },
                {
                    name: 'Delete Slot',
                    action: function () {
                        deleteSlotById(params.node.data.id); 
                    }
                },
                'separator', 'copy', 'copyWithHeaders', 'paste'
            ];
        }
    };

    // Initialize AG Grid
    var eGridDiv = document.querySelector('#slotsGrid');
    new agGrid.Grid(eGridDiv, gridOptions);

    // Load data via AJAX
    function loadSlotsGrid() {
        $.ajax({
            url: '/Slot/GetAllSlots',
            type: 'GET',
            success: function (res) {
                if (res.success) {
                    gridOptions.api.setRowData(res.data);
                } else {
                    alert(res.message);
                }
            },
            error: function (xhr) {
                alert('Failed to load slots.');
                console.error(xhr.responseText);
            }
        });
    }

    $('#btnAddSlot').on('click', function () {
        var addModalEl = document.getElementById('addSlotModal');
        var modal = new bootstrap.Modal(addModalEl);
        modal.show();
    });

    // Submit form via AJAX
    $('#addSlotForm').submit(function (e) {
        e.preventDefault();

        // Compute DaysOfWeek bitmask
        let daysOfWeek = 0;
        $('.day-checkbox:checked').each(function () {
            daysOfWeek += parseInt($(this).val());
        });

        // Prepare DTO
        const dto = {
            ReportingTime: $('#ReportingTime').val(),
            LeavingTime: $('#LeavingTime').val(),
            DaysOfWeek: daysOfWeek
        };

        $.ajax({
            url: '/Slot/Add',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (res) {
                if (res.success) {
                    $('#addSlotModal').modal('hide');
                    alert(res.message);
                    loadSlotsGrid();
                } else {
                    // Clear previous errors
                    $('.text-danger').text('');

                    if (res.errors) {
                        // Display validation errors inline
                        if (res.errors.ReportingTime) {
                            $('#ReportingTime_error').text(res.errors.ReportingTime);
                        }
                        if (res.errors.LeavingTime) {
                            $('#LeavingTime_error').text(res.errors.LeavingTime);
                        }
                        if (res.errors.DaysOfWeek) {
                            $('#DaysOfWeek_error').text(res.errors.DaysOfWeek);
                        }
                    } else {
                        alert(res.message);
                    }
                }
            },
            error: function (xhr) {
                alert('Something went wrong. Please try again.');
                console.error(xhr.responseText);
            }
        });
    });
    // Open Edit modal
    function openEditModal(rowData) {
        if (!rowData) return;

        $('#editSlotId').val(rowData.id);
        $('#editReportingTime').val(rowData.reportingTime);
        $('#editLeavingTime').val(rowData.leavingTime);

        // Clear checkboxes
        $('.edit-day-checkbox').prop('checked', false);

        // Check checkboxes according to bitmask
        $('.edit-day-checkbox').each(function () {
            var value = parseInt($(this).val());
            if ((rowData.daysOfWeek & value) !== 0) $(this).prop('checked', true);
        });

        // Clear error messages
        $('#editReportingTime_error').text('');
        $('#editLeavingTime_error').text('');
        $('#editDaysOfWeek_error').text('');

        // Show Bootstrap 4 modal
        $('#editSlotModal').modal('show');
    }

    // Edit button handler
    $('#slotsGrid').on('click', '.edit-btn', function () {
        var rowId = $(this).data('row-id');
        var rowData = gridOptions.api.getRowNode(rowId).data;
        openEditModal(rowData);
    });

 

    // Handle Edit Slot form submission
    $('#editSlotForm').on('submit', function (e) {
        e.preventDefault();

        // Collect selected days into bitmask
        let daysOfWeek = 0;
        $('.edit-day-checkbox:checked').each(function () {
            daysOfWeek |= parseInt($(this).val());
        });

        // Prepare DTO
        const dto = {
            id: parseInt($('#editSlotId').val()),
            reportingTime: $('#editReportingTime').val(),
            leavingTime: $('#editLeavingTime').val(),
            daysOfWeek: daysOfWeek
        };

        // Clear previous errors
        $('#editReportingTime_error').text('');
        $('#editLeavingTime_error').text('');
        $('#editDaysOfWeek_error').text('');

        $.ajax({
            url: '/Slot/EditSlot',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (response) {
                if (response.success) {
                    // Close modal
                    var editModalEl = document.getElementById('editSlotModal');
                    var modal = bootstrap.Modal.getInstance(editModalEl);
                    modal.hide();

                    // Update row in AG Grid
                    const rowNode = gridOptions.api.getRowNode(dto.id);
                    if (rowNode) {
                        rowNode.setData({
                            id: dto.id,
                            reportingTime: dto.reportingTime,
                            leavingTime: dto.leavingTime,
                            daysOfWeek: dto.daysOfWeek
                        });
                    }

                    alert(response.message);
                } else {
                    // Show validation errors
                    if (response.errors && response.errors.length > 0) {
                        response.errors.forEach(function (err) {
                            if (err.toLowerCase().includes('reporting')) {
                                $('#editReportingTime_error').text(err);
                            } else if (err.toLowerCase().includes('leaving')) {
                                $('#editLeavingTime_error').text(err);
                            } else if (err.toLowerCase().includes('day')) {
                                $('#editDaysOfWeek_error').text(err);
                            }
                        });
                    } else {
                        alert(response.message);
                    }
                }
            },
            error: function (xhr, status, error) {
                console.error(error);
                alert('An unexpected error occurred.');
            }
        });
    });


    function deleteSlotById(rowId) {
        if (!rowId) return;

        swal({
            title: "Are you sure?",
            text: "Once deleted, you will not be able to recover this slot!",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: '/Slot/Delete',
                    type: 'POST',
                    data: { id: rowId },
                    success: function (res) {
                        if (res.success) {
                            // Remove from AG Grid
                            var node = gridOptions.api.getRowNode(rowId);
                            if (node) gridOptions.api.applyTransaction({ remove: [node.data] });

                            swal("Deleted!", res.message, "success");
                        } else {
                            // Handle server-side validation or unexpected errors
                            let msg = res.message || "Failed to delete slot.";
                            if (res.detail) msg += "\n" + res.detail;
                            swal("Error!", msg, "error");
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error("AJAX Error:");
                        console.error("Status:", status);
                        console.error("Error:", error);
                        console.error("Response Text:", xhr.responseText);
                        swal("Error!", "Something went wrong while deleting. Please check console for details.", "error");
                    }

                });
            }
        });
    }

    $('#slotsGrid').on('click', '.delete-btn', function () {
        var rowId = $(this).data('row-id');
        deleteSlotById(rowId);  // call the reusable function
    });

});
