$(document).ready(function () {

    // ==============================
    // 1️⃣ Load Doctors Dropdown
    // ==============================
    function loadDoctors() {
        $.ajax({
            url: "/Doctor/GetAllDoctors",
            type: "GET",
            dataType: "json",
            success: function (response) {
                const select = $("#doctorSelect");
                select.empty().append(`<option value="">-- Select a Doctor --</option>`);

                if (response && response.success && Array.isArray(response.data)) {
                    response.data.forEach(d => {
                        const specialization = d.specialization ? ` - ${d.specialization}` : '';
                        select.append(`<option value="${d.id}">${d.fullName}${specialization}</option>`);
                    });
                } else {
                    showNotification("No doctors found", "info");
                }
            },
            error: function () {
                showNotification("Failed to load doctors", "error");
            }
        });
    }

    // ==============================
    // 2️⃣ Check Feedback Button
    // ==============================
    $("#checkFeedbackBtn").on("click", function () {
        const doctorId = $("#doctorSelect").val();
        if (!doctorId) {
            showNotification("Please select a doctor first", "warning");
            $("#doctorSelect").addClass("animate-shake");
            setTimeout(() => $("#doctorSelect").removeClass("animate-shake"), 500);
            return;
        }

        loadFeedbackForDoctor(doctorId);
    });

    // ==============================
    // 3️⃣ Load Feedback for Doctor
    // ==============================
    function loadFeedbackForDoctor(doctorId) {
        const btn = $("#checkFeedbackBtn");
        const originalText = btn.html();

        btn.prop('disabled', true).html(`<span class="loader mr-2"></span>Loading...`);

        $.ajax({
            url: `/Feedback/GetDoctorFeedback?doctorId=${doctorId}`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response && response.success && Array.isArray(response.data) && response.data.length) {
                    displayFeedbackCards(response.data);
                    showNotification(`Found ${response.data.length} feedback(s)`, "success");
                } else {
                    displayNoFeedback();
                }
                btn.prop('disabled', false).html(originalText);
            },
            error: function () {
                displayNoFeedback();
                btn.prop('disabled', false).html(originalText);
                showNotification("Error loading feedback", "error");
            }
        });
    }

    // ==============================
    // 4️⃣ Display Feedback Cards
    // ==============================
    function displayFeedbackCards(feedbacks) {
        const container = $("#feedbackContainer");
        container.empty();
        feedbacks.forEach(fb => container.append(createFeedbackCard(fb)));
    }

    // ==============================
    // 5️⃣ Create Feedback Card
    // ==============================
    function createFeedbackCard(fb) {
        const rating = fb.rating || 0;
        const stars = generateStars(rating);
        const submittedDate = fb.submittedAt
            ? new Date(fb.submittedAt).toLocaleDateString('en-US', {
                year: 'numeric',
                month: 'short',
                day: 'numeric'
            })
            : 'Unknown';

        return `
<div class="feedback-card bg-gray-900 border border-gray-700 rounded-xl shadow-lg p-6 transition-transform transform hover:scale-105 hover:shadow-2xl">
    <!-- Header -->
    <div class="flex justify-between items-start mb-4">
        <div>
            <h5 class="text-white font-semibold text-lg leading-snug">${fb.patientName || "Anonymous Patient"}</h5>
            <p class="text-gray-400 text-sm mt-1">${fb.departmentName || "General"}</p>
        </div>
        <div class="flex items-center gap-2">
            ${stars}
            <span class="text-gray-300 text-sm ml-1 font-medium">${rating.toFixed(1)}/5</span>
        </div>
    </div>

    <!-- Feedback -->
    <div class="text-gray-300 text-base leading-relaxed mb-4">
        ${fb.comments ? escapeHtml(fb.comments) : '<em>No comments provided</em>'}
    </div>

    <!-- Footer -->
    <div class="flex justify-between text-gray-400 text-xs border-t border-gray-700 pt-3 mt-3">
        <div class="flex items-center gap-2">
            <i class="fas fa-calendar-alt"></i> ${submittedDate}
        </div>
        <div class="flex items-center gap-2">
            <i class="fas fa-laptop"></i> ${fb.submittedFromDevice || "Unknown"}
        </div>
    </div>
</div>
`;

    }

    // ==============================
    // 6️⃣ Generate Stars
    // ==============================
    function generateStars(rating) {
        let stars = '';
        for (let i = 1; i <= 5; i++) {
            stars += i <= rating
                ? `<i class="fas fa-star text-yellow-400"></i>`
                : `<i class="far fa-star text-gray-500"></i>`;
        }
        return stars;
    }

    // ==============================
    // 7️⃣ No Feedback
    // ==============================
    function displayNoFeedback() {
        $("#feedbackContainer").html(`
            <div class="col-span-full text-center py-12">
                <i class="fas fa-inbox text-5xl text-gray-600 mb-4"></i>
                <h4 class="text-gray-400 text-xl font-semibold mb-1">No Feedback Found</h4>
                <p class="text-gray-500">This doctor hasn't received any feedback yet.</p>
            </div>
        `);
    }

    // ==============================
    // 8️⃣ Notification
    // ==============================
    function showNotification(message, type) {
        const colors = {
            success: 'green',
            error: 'red',
            warning: 'yellow',
            info: 'blue'
        };
        const color = colors[type] || 'blue';

        const notification = $(`
            <div class="fixed top-4 right-4 z-50 bg-${color}-600 text-white px-6 py-4 rounded-lg shadow-lg flex items-center gap-3">
                <i class="fas fa-info-circle text-xl"></i>
                <span class="flex-1">${message}</span>
                <button onclick="$(this).parent().remove()" class="text-white hover:text-gray-200">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        `);
        $("body").append(notification);
        setTimeout(() => notification.fadeOut(300, () => notification.remove()), 5000);
    }

    // ==============================
    // 9️⃣ Escape HTML
    // ==============================
    function escapeHtml(text) {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return text.replace(/[&<>"']/g, m => map[m]);
    }

    // ==============================
    // 🔟 Initialize
    // ==============================
    loadDoctors();

    // Shake animation
    if (!$("#shake-animation").length) {
        $("head").append(`
            <style id="shake-animation">
                @keyframes shake {
                    0%, 100% { transform: translateX(0); }
                    10%, 30%, 50%, 70%, 90% { transform: translateX(-5px); }
                    20%, 40%, 60%, 80% { transform: translateX(5px); }
                }
                .animate-shake { animation: shake 0.5s ease-in-out; }
            </style>
        `);
    }
});
