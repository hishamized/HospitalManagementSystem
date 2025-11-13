$('#patientSearchForm').on('submit', function (e) {
    e.preventDefault();

    const form = $(this);
    $.ajax({
        url: '/PatientPortal/SearchPatientByIdentifier',
        method: 'POST',
        data: form.serialize(),
        success: function (response) {

            // Assuming your controller returns JSON like:
            // { success: true, data: { fullName: "...", patientCode: "...", ... } }

            const modalBody = $('#patientModalBody');
            const modalFooter = $('#patientModalFooter');
            modalBody.empty();
            modalFooter.empty();

            if (response && response.success && response.data) {
                const p = response.data;

                modalBody.html(`
                    <div class="mb-3">
                        <strong>Full Name:</strong> ${p.fullName}<br/>
                        <strong>Patient Code:</strong> ${p.patientCode}<br/>
                        <strong>Date of Birth:</strong> ${new Date(p.dateOfBirth).toLocaleDateString()}<br/>
                        <strong>Email:</strong> ${p.email}<br/>
                        <strong>Contact:</strong> ${p.contactNumber}<br/>
                        <strong>Created At:</strong> ${new Date(p.createdAt).toLocaleString()}
                    </div>
                `);

                modalFooter.html(`
                    <button type="button" class="btn btn-success">
                        <i class="bi bi-person-plus"></i> Create Account
                    </button>
                `);

                $('#patientModalLabel').text('Patient Found');
            }
            else {
                modalBody.html(`<p class="text-danger text-center mb-0">No such patient registered at the hospital.</p>`);
                modalFooter.html('');
                $('#patientModalLabel').text('No Record Found');
            }

            const modal = new bootstrap.Modal(document.getElementById('patientModal'));
            modal.show();
        },
        error: function (xhr) {
            console.error(xhr.responseText);
        }
    });
});
