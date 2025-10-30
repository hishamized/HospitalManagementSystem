$(document).ready(function () {
    // 🔹 Define AG Grid columns
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
                        <button class="btn btn-sm btn-danger unassign-btn" data-id="${params.data.id}">
                            <i class="bi bi-x-circle"></i> Unassign
                        </button>`
        }
    ];
    // 🔹 AG Grid Options
    const gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        pagination: true,
        paginationPageSize: 10,
        animateRows: true,
        rowSelection: "single",
        getRowId: params => params.data.id,

        // ✅ Right-Click Context Menu for Unassign
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
                    alert(response.message || "No data found");
                }
            },
            error: function () {
                alert("Error fetching assignment data.");
            }
        });
    }

    // 🔹 Load data on page load
    loadAssignments();
    // Load dropdowns on page load
    loadDoctors();
    loadWards();

    // Refresh lists
    $("#refreshBtn").on("click", function () {
        loadDoctors();
        loadWards();
        loadAssignments();
    });

    // 🔹 Date Formatter for "Assigned At"
    function dateFormatter(params) {
        if (!params.value) return "";
        return new Date(params.value).toLocaleString();
    }

    // 🔹 Handle Unassign button click
    $(document).on('click', '.unassign-btn', function () {
        const id = $(this).data('id');
        unassignDoctor(id);
    });
    // 🔹 Context Menu Creation
    function showContextMenu(x, y, rowData) {
        const menu = $(`
                    <div id="contextMenu" class="dropdown-menu show shadow">
                        <a class="dropdown-item text-danger fw-bold" href="#" data-id="${rowData.id}">
                            <i class="bi bi-x-circle"></i> Unassign
                        </a>
                    </div>
                `);

        $('body').append(menu);
        menu.css({ position: 'absolute', top: y, left: x, zIndex: 10000 });

        // Handle click
        menu.find('a').on('click', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            unassignDoctor(id);
            menu.remove();
        });

        // Remove on click outside
        $(document).on('click.contextMenu', function () {
            menu.remove();
            $(document).off('click.contextMenu');
        });
    }

    // Submit form
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
    // 🧭 Function to Unassign a Doctor from a Ward
    function unassignDoctor(assignmentId) {
        if (!assignmentId || assignmentId <= 0) {
            alert("Invalid assignment ID.");
            return;
        }

        // Confirm before proceeding
        if (!confirm("Are you sure you want to unassign this doctor from the ward?")) {
            return;
        }

        $.ajax({
            url: `/Ward/UnassignDoctor?id=${assignmentId}`, // Adjust path if needed
            type: "DELETE",
            beforeSend: function () {
                console.log("Unassigning doctor... please wait.");
            },
            success: function (response) {
                if (response.success) {
                    loadAssignments
                    alert(response.message || "Doctor unassigned successfully.");
                } else {
                    // ⚠️ Show warning/error from backend
                    alert(response.message || "Failed to unassign doctor.");
                }
            },
            error: function (xhr, status, error) {
                // ❌ Handle unexpected errors
                console.error("Error during unassignment:", error);
                const msg = xhr.responseJSON?.message || "An unexpected error occurred.";
                alert(msg);
            }
        });
    }

    function showToast(message, isError = false) {
        const toast = $("#toast");
        const msg = $("#toastMsg");
        msg.text(message);
        toast.removeClass("bg-success bg-danger").addClass(isError ? "bg-danger" : "bg-success");
        const bsToast = new bootstrap.Toast(toast[0]);
        bsToast.show();
    }


});
