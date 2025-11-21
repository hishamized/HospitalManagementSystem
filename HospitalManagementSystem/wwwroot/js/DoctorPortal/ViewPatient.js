$(document).ready(function () {

    const modal = new bootstrap.Modal(document.getElementById("prescribeTestsModal"));
    const testContainer = $("#testListContainer");

    // OPEN MODAL
    $("#btn-prescribe-tests").on("click", function () {
        modal.show();
        loadValidTests();
    });

    // CLOSE MODAL
    $("#btn-close-tests, #btn-close-tests-2").on("click", function () {
        modal.hide();
    });

    // LOAD TESTS FROM SERVER
    function loadValidTests() {
        testContainer.html(`<div class="p-2 text-muted small">Loading tests...</div>`);

        $.ajax({
            url: "/DoctorPortal/FetchValidTests",
            method: "GET",

            success: function (res) {
                testContainer.html("");

                if (!res || !res.data || res.data.length === 0) {
                    testContainer.html(`<div class="p-2 text-muted">No tests available.</div>`);
                    return;
                }

                res.data.forEach(test => {
                    testContainer.append(`
                        <label class="list-group-item d-flex justify-content-between align-items-center">
                            <div class="d-flex align-items-center gap-2">
                                <input type="checkbox" class="form-check-input test-checkbox"
                                       data-test-id="${test.labTestId}">
                                <span>${test.testName}</span>
                            </div>
                            <span class="badge bg-primary rounded-pill">₹${test.price}</span>
                        </label>
                    `);
                });
            },

            error: function () {
                testContainer.html(`<div class="p-2 text-danger">Failed to load tests.</div>`);
            }
        });
    }

    // SUBMIT LAB REQUEST
    $("#btn-submit-tests").on("click", function () {

        const selectedTests = $(".test-checkbox:checked").map(function () {
            return {
                LabTestId: Number($(this).data("test-id")),
                Status: "Pending",
                CreatedAt: new Date().toISOString(),
                UpdatedAt: null,
                IsActive: true
            };
        }).get();

        if (selectedTests.length === 0) {
            Swal.fire({
                icon: "warning",
                title: "No Test Selected",
                text: "Please select at least one test."
            });
            return;
        }

        const patientId = $("#patientId").val();

        const labRequest = {
            PatientId: Number(patientId),
            DoctorId: null,  // <-- controller will override via claims
            RequestDate: new Date().toISOString(),
            Status: "Pending",
            Notes: $("#labRequestNotes").val() || null,
            CreatedAt: new Date().toISOString(),
            UpdatedAt: null,
            IsActive: true
        };

        const finalPayload = {
            LabRequest: labRequest,
            LabRequestItems: selectedTests
        };

        console.log("🚀 FINAL PAYLOAD:", finalPayload);

        Swal.fire({
            title: "Submitting...",
            text: "Please wait while we submit the lab request.",
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        $.ajax({
            url: "/DoctorPortal/SubmitLabRequest",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(finalPayload),
            success: function (res) {
                Swal.fire({
                    icon: "success",
                    title: "Success",
                    text: res.message || "Lab request submitted successfully!"
                });

                // Optional: refresh grid or clear form
                // reloadLabRequestsGrid();
            },
            error: function (xhr) {
                Swal.fire({
                    icon: "error",
                    title: "Oops!",
                    text: xhr.responseJSON?.message || "Something went wrong while submitting."
                });
            }
        });
    });


});
