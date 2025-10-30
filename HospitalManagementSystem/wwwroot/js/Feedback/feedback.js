// feedback.js
$(document).ready(function () {

    // ==============================
    // 1️⃣ Define AG Grid Columns
    // ==============================
    const columnDefs = [
        { headerName: "ID", field: "id", width: 80 },
        { headerName: "Doctor", field: "doctorName", flex: 1 },
        { headerName: "Department", field: "departmentName", flex: 1 },
        { headerName: "Rating", field: "rating", width: 100 },
        { headerName: "Comments", field: "comments", flex: 1 },
        { headerName: "IP", field: "submittedFromIP", flex: 1 },
        { headerName: "Device", field: "submittedFromDevice", flex: 1 },
        { headerName: "Submitted", field: "submittedAt", width: 180 },
        {
            headerName: "Actions",
            field: "actions",
            width: 130,
            cellRenderer: (params) => {
                return `
                    <button class="btn btn-sm btn-danger delete-btn" data-id="${params.data.id}">
                        <i class="fa fa-trash"></i> Delete
                    </button>
                `;
            },
        },
    ];

    // ==============================
    // 2️⃣ Initialize Grid
    // ==============================
    const gridOptions = {
        columnDefs,
        rowData: [],
        defaultColDef: {
            resizable: true,
            sortable: true,
            filter: true,
        },
        // Right-click context menu that reuses the same delete button logic
        getContextMenuItems: (params) => {
            return [
                {
                    name: '🗑️ Delete Feedback',
                    action: () => {
                        handleDelete(params.node.data.id);
                    }
                }
            ];
        },
    };

    const eGridDiv = document.querySelector("#feedbackGrid");
    new agGrid.Grid(eGridDiv, gridOptions);

    // ==============================
    // 3️⃣ Load Doctors for Dropdown
    // ==============================
    function loadDoctors() {
        $.ajax({
            url: "/Doctor/GetAllDoctors",
            type: "GET",
            dataType: "json",
            success: function (response) {
                const select = $("#doctorId");
                select.empty().append(`<option value="">-- Select Doctor --</option>`);

                if (response && response.success && Array.isArray(response.data)) {
                    response.data.forEach(d => {
                        select.append(`<option value="${d.id}">${d.fullName}</option>`);
                    });
                } else {
                    console.warn("Doctor data not in expected format:", response);
                }
            },
            error: function (xhr) {
                console.error("Error loading doctors:", xhr.responseText);
            }
        });
    }

    // ==============================
    // 4️⃣ Detect Device Info
    // ==============================
    function detectDeviceInfo() {
        let os = "Unknown OS", deviceType = "Unknown device", browser = "UnknownBrowser";

        try {
            const ua = navigator.userAgent || "";
            if (/Android/i.test(ua)) os = "Android";
            else if (/iPhone|iPad|iPod/i.test(ua)) os = "iOS";
            else if (/Win/i.test(ua)) os = "Windows";
            else if (/Mac/i.test(ua)) os = "macOS";
            else if (/Linux/i.test(ua)) os = "Linux";

            if (/Mobi|Android|iPhone/i.test(ua)) deviceType = "Mobile";
            else if (/Tablet|iPad/i.test(ua)) deviceType = "Tablet";
            else deviceType = "Desktop";

            if (/Chrome/i.test(ua) && !/Edge/i.test(ua)) browser = "Chrome";
            else if (/Firefox/i.test(ua)) browser = "Firefox";
            else if (/Safari/i.test(ua) && !/Chrome/i.test(ua)) browser = "Safari";
            else if (/Edg/i.test(ua) || /Edge/i.test(ua)) browser = "Edge";
        } catch { }

        return `${os} | ${deviceType} | ${browser}`;
    }

    // ==============================
    // 5️⃣ Fetch Public IP (Multiple Fallbacks)
    // ==============================
    async function fetchPublicIP() {
        const endpoints = [
            "/Feedback/GetClientIP",
            "https://api.ipify.org?format=json",
            "https://ipapi.co/json/"
        ];

        for (const url of endpoints) {
            try {
                const res = await fetch(url, { cache: "no-store" });
                if (!res.ok) continue;
                const text = await res.text();
                try {
                    const json = JSON.parse(text);
                    if (json.ip || json.ip_address || json.client_ip)
                        return json.ip || json.ip_address || json.client_ip;
                } catch {
                    if (/^\d+\.\d+\.\d+\.\d+$/.test(text.trim()))
                        return text.trim();
                }
            } catch { /* continue */ }
        }
        return "Unknown";
    }

    // ==============================
    // 6️⃣ Hidden Field Setter
    // ==============================
    function setHiddenFields(ip, deviceInfo) {
        $("#submittedFromIP").val(ip);
        $("#submittedFromDevice").val(deviceInfo);
    }

    // ==============================
    // 7️⃣ Handle Feedback Delete
    // ==============================
    function handleDelete(id) {
        if (!id) return;
        if (!confirm("Are you sure you want to delete this feedback?")) return;

        $.ajax({
            url: `/Feedback/Delete/${id}`,
            type: "DELETE",
            success: function (response) {
                alert("✅ Feedback deleted successfully!");
                // Remove from grid visually
                const allData = [];
                gridOptions.api.forEachNode(node => {
                    if (node.data.id !== id) allData.push(node.data);
                });
                gridOptions.api.setRowData(allData);
            },
            error: function (xhr) {
                alert("❌ Error deleting feedback: " + xhr.responseText);
            },
        });
    }

    // Attach click event (only one handler — not duplicated)
    $(document).on("click", ".delete-btn", function () {
        const id = $(this).data("id");
        handleDelete(id);
    });

    // ==============================
    // 8️⃣ Submit Feedback
    // ==============================
    $("#addFeedbackForm").on("submit", async function (e) {
        e.preventDefault();

        const ip = window.__feedback_client_ip || await fetchPublicIP();
        const device = window.__feedback_device_info || detectDeviceInfo();
        setHiddenFields(ip, device);

        const formData = {
            DoctorId: $("#doctorId").val(),
            Rating: $("#rating").val(),
            Comments: $("#comments").val(),
            SubmittedFromIP: ip,
            SubmittedFromDevice: device
        };

        $.ajax({
            url: "/Feedback/Add",
            type: "POST",
            data: JSON.stringify(formData),
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response && response.success) {
                    alert("✅ Thank you! Your feedback has been submitted.");
                    $("#addFeedbackModal").modal("hide");
                    $("#addFeedbackForm")[0].reset();
                    $(".modal-backdrop").remove();
                    $("body").removeClass("modal-open").css("padding-right", "");
                    reloadFeedbackGrid();
                } else {
                    alert("⚠️ Something went wrong. Please try again.");
                }
            },
            error: function (xhr) {
                console.error("Error submitting feedback:", xhr.responseText);
                alert("Error submitting feedback. Please try again later.");
            }
        });
    });

    // ==============================
    // 9️⃣ Load Feedback Data
    // ==============================
    function reloadFeedbackGrid() {
        $.ajax({
            url: "/Feedback/GetAllFeedbacks",
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response && response.success && Array.isArray(response.data)) {
                    gridOptions.api.setRowData(response.data);
                } else {
                    console.warn("Unexpected feedback response:", response);
                    gridOptions.api.setRowData([]);
                }
            },
            error: function (xhr) {
                console.error("Error loading feedbacks:", xhr.responseText);
                gridOptions.api.setRowData([]);
            }
        });
    }

    // ==============================
    // 🔟 Initialize Page
    // ==============================
    (async function init() {
        loadDoctors();
        window.__feedback_device_info = detectDeviceInfo();
        window.__feedback_client_ip = await fetchPublicIP();
        setHiddenFields(window.__feedback_client_ip, window.__feedback_device_info);
        reloadFeedbackGrid();

        $("#addFeedbackModal").on("shown.bs.modal", function () {
            setHiddenFields(window.__feedback_client_ip, window.__feedback_device_info);
        });
    })();

});
