$(document).ready(function () {
    const $doctorSelect = $('#doctorSelect');
    const $feedbackContainer = $('#feedbackContainer');

    // 🔹 Load doctor list from GetAllDoctors
    $.ajax({
        url: '/Doctor/GetAllDoctors',
        type: 'GET',
        success: function (response) {
            if (response && response.data) {
                response.data.forEach(doc => {
                    $doctorSelect.append(`<option value="${doc.id}">${doc.fullName} (${doc.specialization})</option>`);
                });
            }
        },
        error: function () {
            alert('Error loading doctors. Please try again later.');
        }
    });

    // 🔹 Handle Check Feedback button
    $('#checkFeedbackBtn').on('click', function () {
        const doctorId = $doctorSelect.val();
        $feedbackContainer.empty();

        if (!doctorId) {
            $feedbackContainer.html('<div class="no-feedback">Please select a doctor first.</div>');
            return;
        }

        $.ajax({
            url: `/Feedback/GetDoctorFeedback?doctorId=${doctorId}`,
            type: 'GET',
            beforeSend: function () {
                $feedbackContainer.html('<div class="text-center text-muted py-4"><i class="fa-solid fa-spinner fa-spin fa-2x"></i><p>Loading feedback...</p></div>');
            },
            success: function (response) {
                $feedbackContainer.empty();

                if (response.success && response.data && response.data.length > 0) {
                    response.data.forEach(item => {
                        const stars = renderStars(item.rating);
                        const comments = item.comments ? item.comments : '<em>No comments provided</em>';
                        const formattedDate = new Date(item.submittedAt).toLocaleString();

                        const cardHtml = `
                            <div class="col-md-6 col-lg-4">
                                <div class="feedback-card">
                                    <div class="feedback-header">
                                        <h5 class="mb-0">${item.doctorFullName}</h5>
                                        <small>${item.specialization}</small>
                                    </div>
                                    <div class="feedback-body">
                                        <div class="feedback-rating mb-2">${stars}</div>
                                        <p class="text-secondary mb-2">${comments}</p>
                                        <small class="text-muted d-block">Submitted on: ${formattedDate}</small>
                                        <small class="text-muted">From: ${item.submittedFromDevice || 'Unknown Device'} (${item.submittedFromIP || 'N/A'})</small>
                                    </div>
                                </div>
                            </div>`;
                        $feedbackContainer.append(cardHtml);
                    });
                } else {
                    $feedbackContainer.html('<div class="no-feedback">No feedback found for this doctor.</div>');
                }
            },
            error: function (xhr) {
                $feedbackContainer.html(`<div class="no-feedback text-danger">Error fetching feedback: ${xhr.responseJSON?.message || 'Server error'}</div>`);
            }
        });
    });

    // ⭐ Helper: Render star icons
    function renderStars(rating) {
        let starsHtml = '';
        for (let i = 1; i <= 5; i++) {
            starsHtml += `<i class="fa-solid fa-star ${i <= rating ? 'star-filled' : 'star-empty'}"></i>`;
        }
        return starsHtml;
    }
});
