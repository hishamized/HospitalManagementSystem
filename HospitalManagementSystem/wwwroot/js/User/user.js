$(document).ready(function () {

    // ========================
    // 1️⃣ Initialize AG Grid
    // ========================
    const gridOptions = {
        rowHeight: 35,
       
        columnDefs: [
            {
                headerName: "UserId",
                field: "userId",
                width: 120,
                cellRenderer: (params) => {
                    // Create a plain div for the cell
                    const eCell = document.createElement('div');
                    eCell.textContent = params.value; // show the value normally
                    eCell.setAttribute('data-user-id', params.value); // attach data attribute
                    return eCell;
                }
            },
            { headerName: "Full Name", field: "fullName", flex: 1, filter: 'agTextColumnFilter' },
            { headerName: "Email", field: "email", flex: 1, filter: 'agTextColumnFilter' },
            { headerName: "Username", field: "username", flex: 1, filter: 'agTextColumnFilter' },
            { headerName: "Role", field: "roleName", flex: 1, filter: 'agTextColumnFilter' },
            { headerName: "Created At", field: "createdAt", flex: 1, filter: 'agDateColumnFilter' },
            /*
            {
                headerName: "Actions",
                cellRenderer: function (params) {
                    return `
                                <button class="btn btn-sm btn-primary edit-btn" data-id="${params.data.id}">Edit</button>
                                <button class="btn btn-sm btn-danger delete-btn" data-id="${params.data.id}">Delete</button>
                            `;
                },
                width: 180,
                sortable: false,
                filter: false
            }
            */
        ],
        defaultColDef: {
            sortable: true,
            filter: true,
            resizable: true,
            floatingFilter: true,
        },
        pagination: true,
        paginationPageSize: 10,
        paginationPageSizeSelector: [10, 20, 50],
        rowData: [],
        rowSelection: 'single',
        animateRows: true,
        /*
        getContextMenuItems: function (params) {
            const result = [
                'copy', 'copyWithHeaders', 'paste', 'separator',
                {
                    name: 'Edit Admin',
                    action: function () { openEditModal(params.node.data); }
                },
                {
                    name: 'Delete Admin',
                    action: function () { deleteAdmin(params.node.data.id); }
                }
            ];
            return result;
        }
        */
    };

    const gridDiv = document.querySelector("#adminGrid");
    new agGrid.Grid(gridDiv, gridOptions);
  

    // Quick filter search
    $('#quickSearch').on('input', function () {
        gridOptions.api.setQuickFilter($(this).val());
    });

    // ========================
    // 2️⃣ Load Admins
    // ========================
    function loadAdmins() {
        $.ajax({
            url: '/User/GetAllAdmins',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                let admins = [];

                // Handle wrapped or direct responses
                if (Array.isArray(response)) {
                    admins = response;
                } else if (response?.data && Array.isArray(response.data)) {
                    admins = response.data;
                }

                gridOptions.api.setRowData(admins);
            },
            error: function () {
                alert("Error loading admins.");
            }
        });
    }

    loadAdmins();

    // ========================
    // 3️⃣ Roles Dropdown
    // ========================
    function loadRoles() {
        $.ajax({
            url: '/Role/GetRoles',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                const select = $('#roleSelect');
                select.empty().append('<option value="">-- Select Role --</option>');
                if (response?.data && Array.isArray(response.data)) {
                    $.each(response.data, function (i, role) {
                        select.append(`<option value="${role.id}">${role.name}</option>`);
                    });
                }
            }
        });
    }

    // ========================
    // 4️⃣ Add Admin Modal
    // ========================
    $('#btnAddAdmin').click(function () {
        $('#addAdminForm')[0].reset();
        $('#formErrors').addClass('d-none').empty();
        loadRoles();
        $('#addAdminModal').modal('show');
    });

    // ========================
    // 5️⃣ Save Admin
    // ========================
    $('#btnSaveAdmin').click(function () {
        const formData = Object.fromEntries(new FormData(document.querySelector('#addAdminForm')).entries());
        $('#formErrors').addClass('d-none').empty();

        $.ajax({
            url: '/User/CreateAdmin',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (response) {
                if (response.success) {
                    $('#addAdminModal').modal('hide');
                    loadAdmins();
                    alert(response.message);
                } else {
                    $('#formErrors').removeClass('d-none').html(response.message || "Failed to create admin.");
                }
            },
            error: function () {
                $('#formErrors').removeClass('d-none').html("Unexpected error occurred.");
            }
        });
    });

    // ========================
    // 6️⃣ Edit / Delete
    // ========================
    /*
    $(document).on('click', '.edit-btn', function () {
        const id = $(this).data('id');
        const admin = gridOptions.api.getDisplayedRowAtIndex(
            gridOptions.api.getRowIndexForNode(gridOptions.api.getRowNode(id))
        )?.data;
        if (admin) openEditModal(admin);
    });

    $(document).on('click', '.delete-btn', function () {
        const id = $(this).data('id');
        deleteAdmin(id);
    });
    */

    function openEditModal(admin) {
        alert("Edit modal logic goes here for admin: " + admin.fullName);
    }

    function deleteAdmin(id) {
        if (confirm('Are you sure you want to delete this admin?')) {
            $.ajax({
                url: `/User/DeleteAdmin/${id}`,
                type: 'DELETE',
                success: function (response) {
                    alert(response.message);
                    loadAdmins();
                },
                error: function () {
                    alert("Error deleting admin.");
                }
            });
        }
    }
    $(window).on('load', function () {
        const loggedId = String(window.loggedInUserId || '').trim();
        //console.log("⏳ Waiting 3 seconds to highlight user ID:", loggedId);

        setTimeout(function () {
            const $cell = $(`[data-user-id="${loggedId}"]`);
            //console.log("🔍 Found cells:", $cell.length, $cell);

            if ($cell.length) {
                $cell.css({
                    'background-color': '#2563eb',
                    'color': 'white',
                    'font-weight': 'bold',
                    'border': '2px solid #1d4ed8',
                    'border-radius': '4px'
                });
                //console.log("✅ Highlighted logged-in user:", loggedId);
            } else {
                //console.warn("⚠️ No element found with data-user-id:", loggedId);
            }
        }, 3000); // 3 seconds delay
    });


});