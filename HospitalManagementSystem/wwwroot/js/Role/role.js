$(document).ready(function () {

    // Column definitions
    const columnDefs = [
        { headerName: "ID", field: "id", sortable: true, filter: true, width: 90 },
        { headerName: "Name", field: "name", sortable: true, filter: true, flex: 1 },
        { headerName: "Description", field: "description", sortable: true, filter: true, flex: 2 },
        {
            headerName: "Actions",
            cellRenderer: function (params) {
                return `
                            <button class="btn btn-sm btn-primary edit-btn" data-id="${params.data.id}" data-name="${params.data.name}">Edit</button>
                            <button class="btn btn-sm btn-danger delete-btn" data-id="${params.data.id}" data-name="${params.data.name}">Delete</button>
                        `;
            },
            width: 200
        }
    ];

    // Grid options
    const gridOptions = {
        columnDefs: columnDefs,
        rowSelection: "single",
        animateRows: true,
        pagination: true,
        paginationPageSize: 10,
        getContextMenuItems: function (params) {
            const defaultItems = params.defaultItems || [];

            const customItems = [
                {
                    name: 'Edit',
                    icon: '<i class="bi bi-pencil-square text-primary"></i>',
                    action: () => openEditModal(params.node.data)
                },
                {
                    name: 'Delete',
                    icon: '<i class="bi bi-trash text-danger"></i>',
                    action: () => deleteRole(params.node.data.id, params.node.data.name)
                },
                'separator',
            ];

            // Return custom items followed by AG Grid defaults
            return [...customItems, ...defaultItems];
        }
    };


    // Initialize AG Grid
    const gridDiv = document.querySelector("#roleGrid");
    new agGrid.Grid(gridDiv, gridOptions);

    // Load data
    loadRoles();

    // AJAX call to fetch roles
    function loadRoles() {
        $.ajax({
            url: '/Role/GetRoles',
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    gridOptions.api.setRowData(response.data);
                } else {
                    console.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching roles:", error);
            }
        });
    }

    // Context menu logic
    function showContextMenu(event) {
        const contextMenu = $("#contextMenu");
        contextMenu.css({
            display: "block",
            left: event.event.pageX,
            top: event.event.pageY
        }).data("rowData", event.data);
    }

    // Hide context menu when clicking elsewhere
    $(document).click(function () {
        $("#contextMenu").hide();
    });

    // Context menu Edit/Delete actions
    $("#contextMenu").on("click", ".context-edit", function (e) {
        e.preventDefault();
        const rowData = $("#contextMenu").data("rowData");
        $("#contextMenu").hide();
        openEditModal(rowData);
    });

    $("#contextMenu").on("click", ".context-delete", function (e) {
        e.preventDefault();
        const rowData = $("#contextMenu").data("rowData");
        console.log("Delete via context menu:", rowData);
        $("#contextMenu").hide();

        deleteRole(rowData.id, rowData.name);
    });


    // Button-based Edit/Delete actions
    $(document).on("click", ".edit-btn", function () {
        const id = $(this).data("id");
        const name = $(this).data("name");

        // Get row data from AG Grid API
        const allRows = [];
        gridOptions.api.forEachNode(node => allRows.push(node.data));
        const selectedRow = allRows.find(row => row.id === id);

        if (selectedRow) {
            openEditModal(selectedRow);
        } else {
            Swal.fire({
                icon: "error",
                title: "Error",
                text: "Unable to fetch row data for editing."
            });
        }
    });


    $(document).on("click", ".delete-btn", function () {
        const id = $(this).data("id");
        const name = $(this).data("name");
        console.log("Delete button clicked for ID:", id, name);
        deleteRole(id, name);
    });


    // Open modal when "Add Role" button is clicked
    $(document).on("click", "#btnAddRole", function () {
        $("#addRoleForm")[0].reset();
        $(".invalid-feedback").text("");
        $(".form-control").removeClass("is-invalid");
        $("#addRoleModal").modal("show");
    });

    // Handle form submission
    $("#btnSaveRole").click(function (e) {
        e.preventDefault();

        // Collect form data
        const roleData = {
            name: $("#roleName").val(),
            description: $("#roleDescription").val()
        };

        // AJAX call to controller
        $.ajax({
            url: "/Role/Add",
            type: "POST",
            data: JSON.stringify(roleData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            success: function (response) {
                if (response.success) {
                    // Close modal and refresh AG Grid
                    $("#addRoleModal").modal("hide");
                    Swal.fire({
                        icon: "success",
                        title: "Success",
                        text: response.message,
                        timer: 2000,
                        showConfirmButton: false
           
                    });
                    loadRoles();

                    // Reload AG Grid data (assuming gridOptions.api is globally accessible)
                    if (window.roleGridOptions) {
                        window.roleGridOptions.api.refreshServerSideStore({ purge: true });
                    }
                } else {
                    // Handle validation or general errors
                    $(".invalid-feedback").text("");
                    $(".form-control").removeClass("is-invalid");

                    if (response.errors && response.errors.length > 0) {
                        response.errors.forEach(err => {
                            if (err.field.includes("Name")) {
                                $("#roleName").addClass("is-invalid");
                                $("#error-roleName").text(err.error);
                            }
                            if (err.field.includes("Description")) {
                                $("#roleDescription").addClass("is-invalid");
                                $("#error-roleDescription").text(err.error);
                            }
                        });
                    }

                    Swal.fire({
                        icon: "error",
                        title: "Error",
                        text: response.message
                    });
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", error);
                Swal.fire({
                    icon: "error",
                    title: "Unexpected Error",
                    text: "An unexpected error occurred while adding the role."
                });
            }
        });
    });
    function deleteRole(id, name) {
        Swal.fire({
            title: "Are you sure?",
            text: `You are about to delete the role "${name}". This action cannot be undone.`,
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes, delete it!",
            cancelButtonText: "Cancel",
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: "/Role/Delete",
                    type: "POST",
                    data: { id: id }, // Controller expects 'id'
                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                icon: "success",
                                title: "Deleted!",
                                text: response.message,
                                timer: 2000,
                                showConfirmButton: false
                            });

                            // Refresh grid
                            loadRoles();
                        } else {
                            Swal.fire({
                                icon: "error",
                                title: "Cannot Delete",
                                text: response.message
                            });
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error("Delete AJAX error:", error);
                        Swal.fire({
                            icon: "error",
                            title: "Unexpected Error",
                            text: "Something went wrong while deleting the role."
                        });
                    }
                });
            }
        });
    }

    function openEditModal(rowData) {
        // Reset previous validation errors
        $(".invalid-feedback").text("");
        $(".form-control").removeClass("is-invalid");

        // Prefill form
        $("#editRoleId").val(rowData.id);
        $("#editRoleName").val(rowData.name);
        $("#editRoleDescription").val(rowData.description);

        // Show modal
        $("#editRoleModal").modal("show");
    }

    $("#btnUpdateRole").click(function (e) {
        e.preventDefault();

        const roleData = {
            id: $("#editRoleId").val(),
            name: $("#editRoleName").val(),
            description: $("#editRoleDescription").val()
        };

        $.ajax({
            url: "/Role/Edit",
            type: "POST",
            data: JSON.stringify(roleData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            success: function (response) {
                if (response.success) {
                    $("#editRoleModal").modal("hide");
                    Swal.fire({
                        icon: "success",
                        title: "Updated!",
                        text: response.message,
                        timer: 2000,
                        showConfirmButton: false
                    });
                    loadRoles(); // refresh grid data
                } else {
                    $(".invalid-feedback").text("");
                    $(".form-control").removeClass("is-invalid");

                    if (response.errors && response.errors.length > 0) {
                        response.errors.forEach(err => {
                            if (err.field.includes("Name")) {
                                $("#editRoleName").addClass("is-invalid");
                                $("#error-editRoleName").text(err.error);
                            }
                            if (err.field.includes("Description")) {
                                $("#editRoleDescription").addClass("is-invalid");
                                $("#error-editRoleDescription").text(err.error);
                            }
                        });
                    }

                    Swal.fire({
                        icon: "error",
                        title: "Error",
                        text: response.message
                    });
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", error);
                Swal.fire({
                    icon: "error",
                    title: "Unexpected Error",
                    text: "Something went wrong while updating the role."
                });
            }
        });
    });

});