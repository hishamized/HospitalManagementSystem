$(document).ready(function () {
    // 🔹 Define AG Grid columns with dark theme styling
    const columnDefs = [
        { headerName: "Doctor Name", field: "doctorName", flex: 2, sortable: true, filter: true },
        { headerName: "Specialization", field: "specialization", flex: 1, sortable: true, filter: true },
        { headerName: "Ward Name", field: "wardName", flex: 2, sortable: true, filter: true },
        { headerName: "Ward Type", field: "wardType", flex: 1, sortable: true, filter: true },
        { headerName: "Assigned At", field: "assignedAt", flex: 1.5, valueFormatter: dateFormatter },
        { headerName: "Remarks", field: "remarks", flex: 1 },
        {
            headerName: "Actions",
            field: "id",
            flex: 1,
            cellRenderer: params => `
                <button class="unassign-btn bg-red-600 hover:bg-red-700 text-white text-sm font-semibold py-2 px-4 rounded-lg transition-all shadow-md hover:shadow-lg flex items-center gap-2"
                data-id="${params.data.id}">
                    <i class="fa-solid fa-xmark-circle"></i> Unassign
                </button>

            `
        }
    ];

    // 🔹 AG Grid Options with dark theme
    const gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        pagination: true,
        paginationPageSize: 10,
        animateRows: true,
        rowSelection: "single",
        getRowId: params => params.data.id,
        // Dark theme configuration
        theme: 'ag-theme-alpine-dark',
        onCellContextMenu: function (params) {
            params.event.preventDefault();
            if (params.node && params.data) {
                showContextMenu(params.event.clientX, params.event.clientY, params.data);
            }
        }
    };

    // 🔹 Initialize Grid
    const gridDiv = document.querySelector('#assignmentGrid');
    new agGrid.Grid(gridDiv, gridOptions);

    // 🔹 Fetch Assignments from Controller
    function loadAssignments() {
        $.ajax({
            url: '/Ward/GetAllDoctorWardAssignments',
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    gridOptions.api.setRowData(response.data);
                } else {
                    gridOptions.api.setRowData([]);
                    showToast("No assignment data found", true);
                }
            },
            error: function () {
                showToast("Error fetching assignment data", true);
            }
        });
    }

    // 🔹 Load dropdowns and data on page load
    loadAssignments();
    loadDoctors();
    loadWards();

    // 🔹 Refresh button for grid
    $("#refreshBtn").on("click", function () {
        loadAssignments();
        showToast("Grid refreshed successfully!");
    });

    // 🔹 Refresh button for lists
    $("#refreshListsBtn").on("click", function () {
        loadDoctors();
        loadWards();
        showToast("Lists refreshed successfully!");
    });

    // 🔹 Date Formatter
    function dateFormatter(params) {
        if (!params.value) return "";
        return new Date(params.value).toLocaleString();
    }

    // 🔹 Handle Unassign button click
    $(document).on('click', '.unassign-btn', function () {
        const id = $(this).data('id');
        unassignDoctor(id);
    });

    // 🔹 Context Menu Creation (Dark Theme with Tailwind)
    function showContextMenu(x, y, rowData) {
        // Remove any existing context menu
        $('#contextMenu').remove();

        const menu = $(`
            <div id="contextMenu" class="absolute z-50 min-w-[180px] rounded-lg border border-gray-700 bg-gray-800 shadow-2xl">
                <a href="#" data-id="${rowData.id}" class="block px-4 py-2.5 text-red-400 font-semibold hover:bg-gray-700 hover:text-red-300 rounded-lg transition-colors flex items-center gap-2">
                    <i class="fa-solid fa-xmark-circle"></i> Unassign Doctor
                </a>
            </div>
        `);

        $('body').append(menu);
        menu.css({ top: y, left: x });

        menu.find('a').on('click', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            unassignDoctor(id);
            menu.remove();
        });

        $(document).on('click.contextMenu', function () {
            menu.remove();
            $(document).off('click.contextMenu');
        });
    }

    // 🔹 Form Submission
    $("#assignForm").on("submit", function (e) {
        e.preventDefault();

        const doctorId = $("#doctorSelect").val();
        const wardId = $("#wardSelect").val();
        const remarks = $("#remarks").val();

        if (!doctorId || !wardId) {
            showToast("Please select both Doctor and Ward!", true);
            return;
        }

        $.ajax({
            url: "/Ward/AssignDoctorToWard",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ doctorId, wardId, remarks }),
            success: function (res) {
                if (res.success) {
                    loadAssignments();
                    showToast("Doctor assigned successfully!");
                    $("#assignForm")[0].reset();
                } else {
                    showToast(res.message || "Assignment failed!", true);
                }
            },
            error: function () {
                showToast("Server error! Try again later.", true);
            }
        });
    });

    // ---------------- Helper Functions ----------------
    function loadDoctors() {
        $.getJSON("/Doctor/GetAllDoctors", function (res) {
            const select = $("#doctorSelect");
            select.empty().append('<option value="">-- Choose Doctor --</option>');
            if (res.success && res.data) {
                res.data.forEach(d => {
                    select.append(`<option value="${d.id}">${d.fullName} (${d.specialization})</option>`);
                });
            }
        });
    }

    function loadWards() {
        $.getJSON("/Ward/GetAll", function (res) {
            const select = $("#wardSelect");
            select.empty().append('<option value="">-- Choose Ward --</option>');
            if (res.success && res.data) {
                res.data.forEach(w => {
                    select.append(`<option value="${w.id}">${w.wardName} (${w.wardType})</option>`);
                });
            }
        });
    }

    // 🔹 Unassign Doctor
    function unassignDoctor(assignmentId) {
        if (!assignmentId || assignmentId <= 0) {
            showToast("Invalid assignment ID", true);
            return;
        }

        if (!confirm("Are you sure you want to unassign this doctor from the ward?")) {
            return;
        }

        $.ajax({
            url: `/Ward/UnassignDoctor?id=${assignmentId}`,
            type: "DELETE",
            beforeSend: function () {
                console.log("Unassigning doctor... please wait.");
            },
            success: function (response) {
                if (response.success) {
                    loadAssignments();
                    showToast(response.message || "Doctor unassigned successfully");
                } else {
                    showToast(response.message || "Failed to unassign doctor", true);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error during unassignment:", error);
                const msg = xhr.responseJSON?.message || "An unexpected error occurred";
                showToast(msg, true);
            }
        });
    }

    // 🔹 Toast Notifications (Dark Theme with Tailwind)
    function showToast(message, isError = false) {
        const toast = $("#toast");
        const msg = $("#toastMsg");
        const icon = $("#toastIcon");

        msg.text(message);

        // Update icon based on message type
        if (isError) {
            icon.removeClass("fa-check-circle").addClass("fa-exclamation-circle");
            toast.removeClass("bg-purple-600").addClass("bg-red-600");
        } else {
            icon.removeClass("fa-exclamation-circle").addClass("fa-check-circle");
            toast.removeClass("bg-red-600").addClass("bg-purple-600");
        }

        toast.removeClass("hidden").addClass("flex");

        setTimeout(() => {
            toast.removeClass("flex").addClass("hidden");
        }, 4000);
    }

    // Close toast manually
    $("#toastClose").on("click", function () {
        $("#toast").removeClass("flex").addClass("hidden");
    });

});