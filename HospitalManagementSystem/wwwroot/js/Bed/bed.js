$(document).ready(function () {

    let currentPage = 1;
    const pageSize = 10;
    let totalCount = 0;
    let gridOptions;

    // === AG Grid Column Definitions ===
    const columnDefs = [
        { headerName: "ID", field: "id", width: 80, sortable: true, filter: true },
        { headerName: "Bed Code", field: "bedCode", width: 120, sortable: true, filter: true },
        { headerName: "Bed Number", field: "bedNumber", width: 130, sortable: true, filter: true },
        { headerName: "Ward Name", field: "wardName", flex: 1, sortable: true, filter: true },
        { headerName: "Bed Type", field: "bedType", width: 130, sortable: true, filter: true },
        {
            headerName: "Occupied",
            field: "isOccupied",
            width: 120,
            valueFormatter: params => params.value ? 'Yes' : 'No',
            cellStyle: params => {
                return params.value
                    ? { color: '#ef4444', fontWeight: 'bold' }
                    : { color: '#10b981', fontWeight: 'bold' };
            }
        },
        { headerName: "Status", field: "status", width: 130, sortable: true, filter: true },
        { headerName: "Description", field: "description", flex: 1, sortable: true, filter: true },
        {
            headerName: "Actions",
            width: 180,
            cellRenderer: function (params) {
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
    gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        getRowId: params => params.data.id,
        defaultColDef: {
            sortable: true,
            filter: true,
            resizable: true,
            minWidth: 100
        },
        rowHeight: 50,
        animateRows: true,
        pagination: false,
        onGridReady: function (params) {
            params.api.sizeColumnsToFit();
            loadBeds(currentPage, pageSize);
        },
        onGridSizeChanged: function (params) {
            params.api.sizeColumnsToFit();
        }
    };

    // === Initialize AG Grid ===
    const gridDiv = document.querySelector("#bedGrid");
    new agGrid.Grid(gridDiv, gridOptions);

    // === Load Beds Function ===
    function loadBeds(page, ps) {
        $.ajax({
            url: `/Bed/GetPagedBeds?pageNumber=${page}&pageSize=${ps}`,
            type: 'GET',
            success: function (res) {
                console.log('Beds response:', res);
                if (res.success) {
                    gridOptions.api.setRowData(res.data);
                    totalCount = res.totalCount;
                    const totalPages = ps === 0 ? 1 : Math.ceil(totalCount / ps);
                    $("#pageInfo").text(`Page ${page} of ${totalPages} (Total: ${totalCount} beds)`);
                } else {
                    showToast("Failed to fetch data.", 'error');
                }
            },
            error: function (xhr) {
                console.error('Error loading beds:', xhr.responseText);
                showToast("Error fetching beds.", 'error');
            }
        });
    }

    // === Show All Beds Button ===
    $('#btnShowAll').on('click', function () {
        currentPage = 1;
        loadBeds(1, 0); // 0 => fetch all
    });

    // === Pagination Controls ===
    $("#nextPage").click(function () {
        const totalPages = Math.ceil(totalCount / pageSize);
        if (currentPage < totalPages) {
            currentPage++;
            loadBeds(currentPage, pageSize);
        }
    });

    $("#prevPage").click(function () {
        if (currentPage > 1) {
            currentPage--;
            loadBeds(currentPage, pageSize);
        }
    });

    // === Add Bed Modal ===
    $('#btnAddBed').click(function () {
        $('#bedForm')[0].reset();
        loadWards();
        window.showModal('bedModal');
    });

    // === Load Wards Dropdown ===
    function loadWards() {
        $.ajax({
            url: '/Ward/GetAll',
            type: 'GET',
            success: function (res) {
                console.log('Wards response:', res);
                const wards = res.data || res;
                const wardSelect = $('#WardId');
                wardSelect.empty();
                wardSelect.append('<option value="">Select Ward</option>');

                if (Array.isArray(wards)) {
                    $.each(wards, function (i, ward) {
                        wardSelect.append(`<option value="${ward.id}">${ward.wardName}</option>`);
                    });
                }
            },
            error: function (xhr) {
                console.error('Error loading wards:', xhr);
                showToast('Failed to load wards', 'error');
            }
        });
    }

    // === Save New Bed ===
    $('#bedForm').on('submit', function (e) {
        e.preventDefault();

        const formData = {
            bedCode: $('#BedCode').val(),
            bedNumber: $('#BedNumber').val(),
            wardId: parseInt($('#WardId').val()),
            bedType: $('#BedType').val(),
            isOccupied: $('#IsOccupied').is(':checked'),
            status: $('#Status').val(),
            description: $('#Description').val(),
            isActive: true
        };

        const token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            url: '/Bed/AddBed',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            headers: {
                'RequestVerificationToken': token
            },
            success: function (res) {
                if (res.success) {
                    window.hideModal('bedModal');
                    loadBeds(currentPage, pageSize);
                    showToast(res.message || 'Bed added successfully!', 'success');
                } else {
                    let msg = res.message || 'Failed to save bed';
                    if (res.errors) {
                        const errorList = Object.values(res.errors).flat().join('\n');
                        msg += '\n' + errorList;
                    }
                    showToast(msg, 'error');
                }
            },
            error: function (xhr) {
                console.error('Error saving bed:', xhr);
                let errMsg = 'Error saving bed';
                if (xhr.responseJSON && xhr.responseJSON.message)
                    errMsg = xhr.responseJSON.message;
                showToast(errMsg, 'error');
            }
        });
    });

    // === Handle Edit Button Click ===
    $(document).on("click", ".edit-btn", function (e) {
        e.preventDefault();
        const bedId = parseInt($(this).data("id"));
        console.log('Edit clicked for bed ID:', bedId);

        let foundData = null;
        gridOptions.api.forEachNode(function (node) {
            if (node.data && node.data.id === bedId) {
                foundData = node.data;
            }
        });

        if (foundData) {
            openEditModal(foundData);
        } else {
            console.error("Row data not found for Bed ID:", bedId);
            showToast('Error: Bed not found', 'error');
        }
    });

    // === Open Edit Modal ===
    function openEditModal(data) {
        console.log('Opening edit modal for:', data);

        $("#editBedId").val(data.id);
        $("#editBedCode").val(data.bedCode);
        $("#editBedNumber").val(data.bedNumber);
        $("#editWardName").val(data.wardName);
        $("#editBedType").val(data.bedType);
        $("#editIsOccupied").val(data.isOccupied.toString());
        $("#editStatus").val(data.status);
        $("#editDescription").val(data.description || '');

        window.showModal('editBedModal');
    }

    // === Handle Edit Form Submission ===
    $("#editBedForm").on("submit", function (e) {
        e.preventDefault();

        const updatedData = {
            id: parseInt($("#editBedId").val()),
            bedCode: $("#editBedCode").val(),
            bedNumber: $("#editBedNumber").val(),
            wardName: $("#editWardName").val(),
            bedType: $("#editBedType").val(),
            isOccupied: $("#editIsOccupied").val() === "true",
            status: $("#editStatus").val(),
            description: $("#editDescription").val()
        };

        console.log('Updating bed:', updatedData);

        const token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            url: '/Bed/EditBed',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(updatedData),
            headers: { 'RequestVerificationToken': token },
            success: function (res) {
                if (res.success) {
                    window.hideModal('editBedModal');
                    loadBeds(currentPage, pageSize);
                    showToast(res.message || 'Bed updated successfully!', 'success');
                } else {
                    let msg = res.message || 'Failed to update bed.';
                    if (res.errors) {
                        msg += '\n' + res.errors.join('\n');
                    }
                    showToast(msg, 'error');
                }
            },
            error: function (xhr) {
                console.error('Error updating bed:', xhr);
                let errMsg = 'Error updating bed.';
                if (xhr.responseJSON && xhr.responseJSON.message)
                    errMsg = xhr.responseJSON.message;
                showToast(errMsg, 'error');
            }
        });
    });

    // === Handle Delete Button Click ===
    $(document).on("click", ".delete-btn", function (e) {
        e.preventDefault();
        const bedId = parseInt($(this).data("id"));
        console.log('Delete clicked for bed ID:', bedId);

        if (!bedId) {
            console.error("No Bed ID found for delete action.");
            showToast('Error: Invalid bed ID', 'error');
            return;
        }

        let foundData = null;
        gridOptions.api.forEachNode(function (node) {
            if (node.data && node.data.id === bedId) {
                foundData = node.data;
            }
        });

        if (!foundData) {
            console.error("Bed not found with ID:", bedId);
            showToast('Error: Bed not found', 'error');
            return;
        }

        if (!confirm(`Are you sure you want to delete bed "${foundData.bedNumber}" (${foundData.bedCode})?`)) {
            return;
        }

        const $button = $(this);
        $button.prop("disabled", true).html('<i class="fas fa-spinner fa-spin"></i> Deleting...');

        $.ajax({
            url: `/Bed/DeleteBed/${bedId}`,
            type: "DELETE",
            success: function (response) {
                if (response.success) {
                    console.log("Delete success:", response);
                    loadBeds(currentPage, pageSize);
                    showToast(response.message || 'Bed deleted successfully!', 'success');
                } else {
                    console.warn("Delete response:", response);
                    showToast(response.message || "Failed to delete bed.", 'error');
                }
            },
            error: function (xhr, status, error) {
                console.error("Delete error:", xhr.responseText);
                showToast(`Error deleting bed: ${xhr.responseText || error}`, 'error');
            },
            complete: function () {
                $button.prop("disabled", false).html('<i class="fas fa-trash"></i> Delete');
            }
        });
    });

    // === Toast Notification Helper ===
    function showToast(message, type) {
        if (typeof toastr !== 'undefined') {
            toastr[type](message);
        } else {
            alert(message);
        }
    }
});