$(document).ready(function () {

    // Bootstrap modal instance
    const editBedModal = new bootstrap.Modal(document.getElementById('editBedModal'));

    $(document).on("click", ".edit-btn", function () {
        const id = $(this).data("id");

        // ✅ Safely find row data
        let rowData = [];
        gridOptions.api.forEachNode(node => rowData.push(node.data));
        const data = rowData.find(r => r.id == id);

        if (!data) {
            console.error("Row data not found for Bed ID:", id);
            return;
        }

        // Prefill modal fields
        $("#editBedId").val(data.id);
        $("#editBedCode").val(data.bedCode);
        $("#editBedNumber").val(data.bedNumber);
        $("#editWardName").val(data.wardName);
        $("#editBedType").val(data.bedType);
        $("#editIsOccupied").val(data.isOccupied.toString());
        $("#editStatus").val(data.status);
        $("#editDescription").val(data.description);

        // Show modal
        editBedModal.show();
    });


    // Just log form submit for now
    $("#editBedForm").on("submit", function (e) {
        e.preventDefault();

        const updatedData = {
            id: $("#editBedId").val(),
            bedCode: $("#editBedCode").val(),
            bedNumber: $("#editBedNumber").val(),
            wardName: $("#editWardName").val(),
            bedType: $("#editBedType").val(),
            isOccupied: $("#editIsOccupied").val() === "true",
            status: $("#editStatus").val(),
            description: $("#editDescription").val()
        };

        editBedModal.hide();

        const token = $('input[name="__RequestVerificationToken"]').val(); // ✅ include this

        $.ajax({
            url: '/Bed/EditBed',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(updatedData), // ✅ use correct variable
            headers: { 'RequestVerificationToken': token },
            success: function (res) {
                if (res.success) {
                    alert(res.message);
                    loadBeds(currentPage, pageSize); // ✅ refresh data
                } else {
                    let msg = res.message || 'Failed to update bed.';
                    if (res.errors) {
                        msg += '\n' + res.errors.join('\n');
                    }
                    alert(msg);
                }
            },
            error: function (xhr) {
                let errMsg = 'Error updating bed.';
                if (xhr.responseJSON && xhr.responseJSON.message)
                    errMsg = xhr.responseJSON.message;
                alert(errMsg);
            }
        });
    });

    let currentPage = 1;
    const pageSize = 10;
    let totalCount = 0;
    let gridOptions;

    const columnDefs = [
        { headerName: "ID", field: "id", width: 80 },
        { headerName: "Bed Code", field: "bedCode" },
        { headerName: "Bed Number", field: "bedNumber" },
        { headerName: "Ward Name", field: "wardName" },
        { headerName: "Bed Type", field: "bedType" },
        { headerName: "Occupied", field: "isOccupied", width: 120 },
        { headerName: "Status", field: "status" },
        { headerName: "Description", field: "description" },
        {
            headerName: "Actions",
            field: "id",
            cellRenderer: function (params) {
                return `
            <button class="btn btn-sm btn-primary btn-edit" data-id="${params.data.id}">Edit</button>
            <button class="btn btn-sm btn-danger btn-delete" data-id="${params.data.id}">Delete</button>
        `;
            },
            width: 180,
            suppressSizeToFit: true
        }

    ];

    gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        getRowId: params => params.data.id, // ✅ Add this line
        defaultColDef: {
            sortable: true,
            filter: true,
            resizable: true,
        },
        pagination: false,
        onGridReady: function () {
            loadBeds(currentPage);
        }
    };


    const gridDiv = document.querySelector("#bedGrid");
    new agGrid.Grid(gridDiv, gridOptions);

    $('#btnShowAll').on('click', function () {
        loadBeds(1,0); // 0 => fetch all
    });

    function loadBeds(page, ps) {
        $.ajax({
            url: `/Bed/GetPagedBeds?pageNumber=${page}&pageSize=${ps}`,
            type: 'GET',
            success: function (res) {
                if (res.success) {
                    gridOptions.api.setRowData(res.data);
                    totalCount = res.totalCount;
                    $("#pageInfo").text(`Page ${currentPage} of ${Math.ceil(totalCount / ps)}`);
                } else {
                    alert("Failed to fetch data.");
                }
            },
            error: function (xhr) {
                console.error(xhr.responseText);
                alert("Error fetching beds.");
            }
        });
    }

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

 
    // Handle Delete Button Click
    $(document).on("click", ".btn-delete", function () {
        const bedId = $(this).data("id");

        if (!bedId) {
            console.error("No Bed ID found for delete action.");
            return;
        }

        // Confirm deletion with user
        if (!confirm(`Are you sure you want to delete bed with ID ${bedId}?`)) {
            return;
        }

        // Disable button to prevent double clicks
        const $button = $(this);
        $button.prop("disabled", true).text("Deleting...");

        $.ajax({
            url: `/Bed/DeleteBed/${bedId}`,
            type: "DELETE",
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    console.log("Delete success:", response);

                    // Refresh grid after delete
                    loadBeds(currentPage, pageSize);
                } else {
                    alert(response.message || "Failed to delete bed.");
                    console.warn("Delete response:", response);
                }
            },
            error: function (xhr, status, error) {
                console.error("Delete error:", xhr.responseText);
                alert(`Error deleting bed: ${xhr.responseText || error}`);
            },
            complete: function () {
                $button.prop("disabled", false).text("Delete");
            }
        });
    });


    $("#addBedBtn").click(function () {
        console.log("Add Bed button clicked");
    });
    const bedModal = new bootstrap.Modal(document.getElementById('bedModal'));

    $('#btnAddBed').click(function () {
        $('#bedForm')[0].reset();
        $('#bedModalLabel').text('Add Bed');
        loadWards();
        bedModal.show();
    });

    function loadWards() {
        $.ajax({
            url: '/Ward/GetAll',
            type: 'GET',
            success: function (res) {
                const beds = res.data
                const wardSelect = $('#WardId');
                wardSelect.empty();
                wardSelect.append('<option value="">Select Ward</option>');
                $.each(beds, function (i, ward) {
                    wardSelect.append(`<option value="${ward.id}">${ward.wardName}</option>`);
                });
            }
        });
    }

    $('#bedForm').on('submit', function (e) {
        e.preventDefault();
        const formData = {
            bedCode: $('#BedCode').val(),
            bedNumber: $('#BedNumber').val(),
            wardId: $('#WardId').val(),
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
                    bedModal.hide();
                    alert(res.message);
                    loadBeds(currentPage, pageSize);
                } else {
                    let msg = res.message || 'Failed to save bed';
                    if (res.errors) {
                        const errorList = Object.values(res.errors).flat().join('\n');
                        msg += '\n' + errorList;
                    }
                    alert(msg);
                }
            },
            error: function (xhr) {
                let errMsg = 'Error saving bed';
                if (xhr.responseJSON && xhr.responseJSON.message)
                    errMsg = xhr.responseJSON.message;
                alert(errMsg);
            }
        });
    });
});
