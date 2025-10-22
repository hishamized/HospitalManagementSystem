$(document).ready(function () {
    // Initialize Bootstrap modal
    var addDoctorModal = new bootstrap.Modal(document.getElementById('addDoctorModal'));

    // Show modal on Add button click
    $('#btnAddDoctor').click(function () {
        $('#addDoctorForm')[0].reset();      // Reset form
        $('#doctorFormErrors').addClass('d-none').text(''); // Clear previous errors
        addDoctorModal.show();
    });

    // Handle Save button click
    $('#saveDoctorBtn').click(function () {
        var formData = $('#addDoctorForm').serialize(); // Serialize form data

        $.ajax({
            url: '/Doctor/AddDoctor', // Controller method
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    addDoctorModal.hide();   // Hide modal
                    // Optional: refresh AG Grid or table here
                    alert('Doctor added successfully!');
                } else {
                    // Show errors in modal
                    $('#doctorFormErrors').removeClass('d-none').text(response.message || 'An error occurred.');
                }
            },
            error: function (xhr, status, error) {
                $('#doctorFormErrors').removeClass('d-none').text('Server error: ' + error);
            }
        });
    });
});
