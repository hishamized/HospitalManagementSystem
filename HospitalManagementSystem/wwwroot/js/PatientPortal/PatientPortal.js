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
                    <div class="space-y-3 text-gray-300">
                        <div class="flex">
                            <span class="font-semibold text-purple-400 w-36">Patient Code:</span>
                            <span>${p.patientCode}</span>
                        </div>
                        <div class="flex">
                            <span class="font-semibold text-purple-400 w-36">Date of Birth:</span>
                            <span>${new Date(p.dateOfBirth).toLocaleDateString()}</span>
                        </div>
                        <div class="flex">
                            <span class="font-semibold text-purple-400 w-36">Email:</span>
                            <span>${p.email}</span>
                        </div>
                        <div class="flex">
                            <span class="font-semibold text-purple-400 w-36">Contact:</span>
                            <span>${p.contactNumber}</span>
                        </div>
                        <div class="flex">
                            <span class="font-semibold text-purple-400 w-36">Gender:</span>
                            <span>${p.gender}</span>
                        </div>
                        <div class="flex">
                            <span class="font-semibold text-purple-400 w-36">Created At:</span>
                            <span>${new Date(p.createdAt).toLocaleString()}</span>
                        </div>
                    </div>
                `);

                modalFooter.html(`
                    <a href="/PatientPortal/LoginPage" 
                       class="w-full sm:w-auto px-6 py-2 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-lg shadow-md transition duration-200 focus:outline-none focus:ring-2 focus:ring-green-500 flex items-center justify-center inline-flex">
                        <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z"></path>
                        </svg>
                        Login
                    </a>
                `);


                $('#patientModalLabel').text('Patient Found').removeClass('text-red-400').addClass('text-purple-400');
            }
            else {
                modalBody.html(`<p class="text-red-400 text-center mb-0">No such patient registered at the hospital.</p>`);
                modalFooter.html('');
                $('#patientModalLabel').text('No Record Found').removeClass('text-purple-400').addClass('text-red-400');
            }

            // Show modal
            showModal();
        },
        error: function (xhr) {
            console.error(xhr.responseText);
        }
    });
});

function showModal() {
    const modal = $('#patientModal');
    const modalContent = modal.find('.inline-block');

    $('body').css('overflow', 'hidden'); // Lock body scroll
    modal.removeClass('hidden');

    // Trigger animation after a small delay to ensure CSS transition works
    setTimeout(() => {
        modalContent.removeClass('scale-95 opacity-0').addClass('scale-100 opacity-100');
    }, 50);
}

function hideModal() {
    const modal = $('#patientModal');
    const modalContent = modal.find('.inline-block');

    modalContent.removeClass('scale-100 opacity-100').addClass('scale-95 opacity-0');

    // Wait for animation to complete before hiding
    setTimeout(() => {
        modal.addClass('hidden');
        $('body').css('overflow', ''); // Restore body scroll
    }, 200);
}

// Modal close functionality
$(document).on('click', '#modalCloseBtn, #modalOverlay', function () {
    hideModal();
});

// Close modal on ESC key
$(document).on('keydown', function (e) {
    if (e.key === 'Escape' && !$('#patientModal').hasClass('hidden')) {
        hideModal();
    }
});

// Prevent Bootstrap from interfering with the custom modal
$(document).ready(function () {
    // Remove any Bootstrap modal attributes that might have been added
    $('#patientModal').removeAttr('data-bs-backdrop data-bs-keyboard');
});