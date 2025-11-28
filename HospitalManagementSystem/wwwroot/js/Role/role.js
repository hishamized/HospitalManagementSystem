$(document).ready(function () {

    /* ============================================================
       AG GRID CONFIG
    ============================================================ */

    const columnDefs = [
        { headerName: "ID", field: "id", sortable: true, filter: true, width: 90 },
        { headerName: "Name", field: "name", sortable: true, filter: true, flex: 1 },
        { headerName: "Description", field: "description", sortable: true, filter: true, flex: 2 },

        {
            headerName: "Actions",
            field: "id",
            cellRenderer: function (params) {
                const name = params.data.name.replace(/'/g, "\\'");
                const desc = (params.data.description || "").replace(/'/g, "\\'");
                return `
                    <button class="bg-purple-600 hover:bg-purple-700 text-white px-3 py-1 rounded-lg"
                        onclick="openEditModal(${params.data.id}, '${name}', '${desc}')">
                        <i class="fa-solid fa-pen"></i>
                    </button>

                    <button class="bg-red-600 hover:bg-red-700 text-white px-3 py-1 rounded-lg ml-2"
                        onclick="deleteRole(${params.data.id}, '${name}')">
                        <i class="fa-solid fa-trash"></i>
                    </button>
                `;
            },
            width: 150,
            sortable: false,
            filter: false
        }
    ];

    const gridOptions = {
        columnDefs: columnDefs,
        rowSelection: "single",
        animateRows: true,
        pagination: true,
        paginationPageSize: 10,
        rowHeight: 50,

        getContextMenuItems: function (params) {
            return [
                {
                    name: "Edit",
                    icon: '<i class="bi bi-pencil-square text-purple-400"></i>',
                    action: () =>
                        openEditModal(params.node.data.id, params.node.data.name, params.node.data.description)
                },
                {
                    name: "Delete",
                    icon: '<i class="bi bi-trash text-red-500"></i>',
                    action: () =>
                        deleteRole(params.node.data.id, params.node.data.name)
                },
                "separator",
                ...params.defaultItems
            ];
        }
    };

    const gridDiv = document.querySelector("#roleGrid");
    new agGrid.Grid(gridDiv, gridOptions);
    // After creating the grid
    gridOptions.api.sizeColumnsToFit();


    loadRoles();


    /* ============================================================
       LOAD ROLES
    ============================================================ */
    function loadRoles() {
        $.ajax({
            url: '/Role/GetRoles',
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    gridOptions.api.setRowData(response.data);
                }
            },
            error: () => console.error("Error fetching roles")
        });
    }


    /* ============================================================
       TAILWIND: OPEN ADD ROLE MODAL
    ============================================================ */
    $("#btnAddRole").click(function () {
        $("#addRoleForm")[0].reset();
        $(".invalid-feedback").text("").addClass("hidden");
        $(".form-control").removeClass("is-invalid");

        window.showModal("addRoleModal");
    });


    /* ============================================================
       ADD ROLE
    ============================================================ */
    $("#btnSaveRole").click(function (e) {
        e.preventDefault();

        const roleData = {
            name: $("#roleName").val(),
            description: $("#roleDescription").val()
        };

        $.ajax({
            url: "/Role/Add",
            type: "POST",
            data: JSON.stringify(roleData),
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response.success) {
                    window.hideModal("addRoleModal");

                    Swal.fire({
                        icon: "success",
                        title: "Success",
                        text: response.message,
                        timer: 2000,
                        showConfirmButton: false
                    });

                    loadRoles();
                } else {
                    showValidationErrors(response.errors);
                }
            }
        });
    });


    /* ============================================================
       🔥 CLEAN TAILWIND-ONLY EDIT MODAL FUNCTION
    ============================================================ */
    window.openEditModal = function (roleId, roleName, roleDescription) {

        $("#editRoleId").val(roleId);
        $("#editRoleName").val(roleName);
        $("#editRoleDescription").val(roleDescription || "");

        $("#error-editRoleName").text("").addClass("hidden");
        $("#error-editRoleDescription").text("").addClass("hidden");

        window.showModal("editRoleModal");
    };


    /* ============================================================
       UPDATE ROLE
    ============================================================ */
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

            success: function (response) {
                if (response.success) {
                    window.hideModal("editRoleModal");

                    Swal.fire({
                        icon: "success",
                        title: "Updated!",
                        text: response.message,
                        timer: 2000,
                        showConfirmButton: false
                    });

                    loadRoles();
                } else {
                    showValidationErrors(response.errors);
                }
            }
        });
    });


    /* ============================================================
       DELETE ROLE
    ============================================================ */
    window.deleteRole = function (id, name) {
        Swal.fire({
            title: "Delete this role?",
            text: `Role: "${name}"`,
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Delete"
        }).then(result => {
            if (result.isConfirmed) {
                $.ajax({
                    url: "/Role/Delete",
                    type: "POST",
                    data: { id },

                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                icon: "success",
                                title: "Deleted!",
                                text: response.message,
                                timer: 2000,
                                showConfirmButton: false
                            });

                            loadRoles();
                        }
                    }
                });
            }
        });
    };


    /* ============================================================
       FORM VALIDATION HELPER
    ============================================================ */
    function showValidationErrors(errors) {
        $(".invalid-feedback").text("").addClass("hidden");
        $(".form-control").removeClass("is-invalid");

        if (!errors) return;

        errors.forEach(err => {
            if (err.field.includes("Name")) {
                $("#editRoleName, #roleName").addClass("is-invalid");
                $("#error-editRoleName, #error-roleName").text(err.error).removeClass("hidden");
            }
            if (err.field.includes("Description")) {
                $("#editRoleDescription, #roleDescription").addClass("is-invalid");
                $("#error-editRoleDescription, #error-roleDescription").text(err.error).removeClass("hidden");
            }
        });
    }

});
