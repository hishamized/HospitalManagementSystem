// feedback.js
$(document).ready(function () {

    // ==============================
    // 1️⃣ Define AG Grid Columns
    // ==============================
    const columnDefs = [
        { headerName: "ID", field: "id", width: 80, sortable: true, filter: true },
        { headerName: "Doctor", field: "doctorName", flex: 1, sortable: true, filter: true },
        { headerName: "Department", field: "departmentName", width: 150, sortable: true, filter: true },
        {
            headerName: "Rating",
            field: "rating",
            width: 150,
            cellRenderer: (params) => {
                const rating = params.value || 0;
                const stars = '⭐'.repeat(rating);
                return `<span class="text-yellow-400">${stars}</span> <span class="text-gray-400">(${rating})</span>`;
            }
        },
        { headerName: "Comments", field: "comments", flex: 2, sortable: true, filter: true },
        { headerName: "IP", field: "submittedFromIP", width: 140, sortable: true, filter: true },
        { headerName: "Device", field: "submittedFromDevice", flex: 1, sortable: true, filter: true },
        {
            headerName: "Submitted",
            field: "submittedAt",
            width: 180,
            valueFormatter: (params) => {
                if (params.value) {
                    return new Date(params.value).toLocaleString();
                }
                return '';
            }
        },
        {
            headerName: "Actions",
            width: 180,
            cellRenderer: (params) => {
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

    // ==============================
    // 2️⃣ Initialize Grid
    // ==============================
    const gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        defaultColDef: {
            resizable: true,
            sortable: true,
            filter: true,
            minWidth: 100
        },
        rowHeight: 50,
        animateRows: true,
        pagination: true,
        paginationPageSize: 20,
        onGridReady: function (params) {
            params.api.sizeColumnsToFit();
            reloadFeedbackGrid();
        },
        onGridSizeChanged: function (params) {
            params.api.sizeColumnsToFit();
        },
        getRowNodeId: data => data.id,
        getContextMenuItems: (params) => {
            return [
                {
                    name: '✏️ Edit Feedback',
                    action: () => {
                        editFeedback(params.node.data);
                    }
                },
                {
                    name: '🗑️ Delete Feedback',
                    action: () => {
                        handleDelete(params.node.data.id);
                    }
                },
                'separator',
                'copy',
                'export'
            ];
        }
    };

    const eGridDiv = document.querySelector("#feedbackGrid");
    new agGrid.Grid(eGridDiv, gridOptions);

    // ==============================
    // 3️⃣ Load Doctors for Dropdown
    // ==============================
    function loadDoctors(selectElementId = "#doctorId") {
        $.ajax({
            url: "/Doctor/GetAllDoctors",
            type: "GET",
            dataType: "json",
            success: function (response) {
                console.log('Doctors response:', response);
                const select = $(selectElementId);
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
                showMessage("Failed to load doctors", "error");
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
    // 5️⃣ Fetch Public IP
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
    function setHiddenFields(ip, deviceInfo, prefix = '') {
        $(`#${prefix}submittedFromIP`).val(ip);
        $(`#${prefix}submittedFromDevice`).val(deviceInfo);
    }

    // ==============================
    // 7️⃣ Edit Feedback Function
    // ==============================
    function editFeedback(data) {
        console.log('Editing feedback:', data);

        $('#editFeedbackId').val(data.id);
        $('#editRatingSelect').val(data.rating);
        $('#editComments').val(data.comments);
        $('#editSubmittedFromIP').val(data.submittedFromIP);
        $('#editSubmittedFromDevice').val(data.submittedFromDevice);

        // Load doctors for edit dropdown
        loadDoctors('#editDoctorSelect');

        // Set doctor value after doctors are loaded
        setTimeout(() => {
            $('#editDoctorSelect').val(data.doctorId);
        }, 500);

        // Clear errors
        $('#editFeedbackErrors').addClass('hidden').html('');

        window.showModal('editFeedbackModal');
    }

    // ==============================
    // 8️⃣ Handle Edit Button Click
    // ==============================
    $(document).on("click", ".edit-btn", function (e) {
        e.preventDefault();
        const feedbackId = parseInt($(this).data('id'));
        console.log('Edit clicked for feedback ID:', feedbackId);

        let foundData = null;
        gridOptions.api.forEachNode(function (node) {
            if (node.data && node.data.id === feedbackId) {
                foundData = node.data;
            }
        });

        if (foundData) {
            editFeedback(foundData);
        } else {
            console.error('Could not find feedback with ID:', feedbackId);
            showMessage('Error: Feedback not found', 'error');
        }
    });

    // ==============================
    // 9️⃣ Handle Edit Form Submission
    // ==============================
    $("#editFeedbackForm").on("submit", function (e) {
        e.preventDefault();

        const formData = {
            Id: parseInt($('#editFeedbackId').val()),
            DoctorId: parseInt($('#editDoctorSelect').val()),
            Rating: parseInt($('#editRatingSelect').val()),
            Comments: $('#editComments').val(),
            SubmittedFromIP: $('#editSubmittedFromIP').val(),
            SubmittedFromDevice: $('#editSubmittedFromDevice').val()
        };

        console.log('Updating feedback:', formData);

        $.ajax({
            url: "/Feedback/Update",
            type: "PUT",
            data: JSON.stringify(formData),
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response && response.success) {
                    window.hideModal('editFeedbackModal');
                    reloadFeedbackGrid();
                    showMessage("Feedback updated successfully!", "success");
                } else {
                    $('#editFeedbackErrors').removeClass('hidden').html(response.message || 'Update failed');
                }
            },
            error: function (xhr) {
                console.error("Error updating feedback:", xhr.responseText);
                $('#editFeedbackErrors').removeClass('hidden').html('Error updating feedback. Please try again.');
            }
        });
    });

    // ==============================
    // 🔟 Handle Feedback Delete
    // ==============================
    function handleDelete(id) {
        if (!id) return;
        if (!confirm("Are you sure you want to delete this feedback?")) return;

        $.ajax({
            url: `/Feedback/Delete/${id}`,
            type: "DELETE",
            success: function (response) {
                if (response && response.success) {
                    reloadFeedbackGrid();
                    showMessage("Feedback deleted successfully!", "success");
                } else {
                    showMessage("Failed to delete feedback", "error");
                }
            },
            error: function (xhr) {
                console.error("Error deleting feedback:", xhr.responseText);
                showMessage("Error deleting feedback", "error");
            }
        });
    }

    $(document).on("click", ".delete-btn", function (e) {
        e.preventDefault();
        const feedbackId = parseInt($(this).data('id'));

        let foundData = null;
        gridOptions.api.forEachNode(function (node) {
            if (node.data && node.data.id === feedbackId) {
                foundData = node.data;
            }
        });

        if (foundData) {
            handleDelete(foundData.id);
        }
    });

    // ==============================
    // 1️⃣1️⃣ Submit Feedback
    // ==============================
    $("#addFeedbackForm").on("submit", async function (e) {
        e.preventDefault();

        const ip = window.__feedback_client_ip || await fetchPublicIP();
        const device = window.__feedback_device_info || detectDeviceInfo();
        setHiddenFields(ip, device);

        const formData = {
            DoctorId: parseInt($("#doctorId").val()),
            Rating: parseInt($("#rating").val()),
            Comments: $("#comments").val(),
            SubmittedFromIP: ip,
            SubmittedFromDevice: device
        };

        console.log('Submitting feedback:', formData);

        $.ajax({
            url: "/Feedback/Add",
            type: "POST",
            data: JSON.stringify(formData),
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response && response.success) {
                    window.hideModal('addFeedbackModal');
                    $("#addFeedbackForm")[0].reset();
                    reloadFeedbackGrid();
                    showMessage("Thank you! Your feedback has been submitted.", "success");
                } else {
                    showMessage("Something went wrong. Please try again.", "error");
                }
            },
            error: function (xhr) {
                console.error("Error submitting feedback:", xhr.responseText);
                showMessage("Error submitting feedback. Please try again later.", "error");
            }
        });
    });

    // ==============================
    // 1️⃣2️⃣ Load Feedback Data
    // ==============================
    function reloadFeedbackGrid() {
        $.ajax({
            url: "/Feedback/GetAllFeedbacks",
            type: "GET",
            dataType: "json",
            success: function (response) {
                console.log('Feedback response:', response);
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
                showMessage("Error loading feedback data", "error");
            }
        });
    }

    // ==============================
    // 1️⃣3️⃣ Message Display Function
    // ==============================
    function showMessage(message, type) {
        const messageContainer = $('#messageContainer');
        const alertClass = type === 'success' ? 'green' : type === 'error' ? 'red' : 'blue';
        const icon = type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-triangle' : 'info-circle';

        const alertHtml = `
            <div class="bg-${alertClass}-600/20 border border-${alertClass}-500 text-${alertClass}-300 p-4 rounded-lg mb-4 flex items-center animate-fade-in">
                <i class="fas fa-${icon} mr-3 text-xl"></i>
                <span>${message}</span>
                <button onclick="this.parentElement.remove()" class="ml-auto text-${alertClass}-300 hover:text-white">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        `;

        messageContainer.html(alertHtml);

        // Auto-hide after 5 seconds
        setTimeout(() => messageContainer.html(''), 5000);
    }

    // ==============================
    // 1️⃣4️⃣ Initialize Page
    // ==============================
    (async function init() {
        loadDoctors();
        window.__feedback_device_info = detectDeviceInfo();
        window.__feedback_client_ip = await fetchPublicIP();
        setHiddenFields(window.__feedback_client_ip, window.__feedback_device_info);
    })();

});