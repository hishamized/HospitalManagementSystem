$(document).ready(function () {

    // -------------------------------------------------------
    // STEP 2: VERIFY EMAIL + PHONE
    // -------------------------------------------------------
    $("#forgotPasswordForm").on("submit", function (e) {
        e.preventDefault();

        const email = $("#fpEmail").val().trim();
        const phone = $("#fpPhone").val().trim();

        $("#otpEmail").val(email);
        $("#fpError").addClass("hidden");

        $.ajax({
            url: "/User/VerifyEmailPhone",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                email: email,
                phoneNumber: phone
            }),

            success: function (json) {
                if (json.success) {

                    // Close forgot password modal and open OTP modal
                    window.dispatchEvent(new CustomEvent('close-forgot-modal'));
                    window.dispatchEvent(new CustomEvent('open-otp-modal'));

                    toastr.success("Verification successful! OTP sent to your email.");

                } else {
                    $("#fpError").removeClass("hidden").text(json.message || "Verification failed.");
                }
            },

            error: function () {
                $("#fpError").removeClass("hidden").text("Server error. Please try again.");
            }
        });
    });

    // -------------------------------------------------------
    // STEP 3: VERIFY OTP
    // -------------------------------------------------------
    $("#otpForm").on("submit", function (e) {
        e.preventDefault();

        const otpCode = $("#otpCode").val().trim();
        const email = $("#otpEmail").val().trim();

        $("#otpError").addClass("hidden");

        if (!otpCode) {
            $("#otpError").removeClass("hidden").text("Please enter your OTP.");
            return;
        }

        $.ajax({
            url: "/User/VerifyOtp",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                otpCode: otpCode,
                email: email
            }),

            success: function (json) {

                if (json.success) {

                    // Close OTP modal
                    window.dispatchEvent(new CustomEvent('close-otp-modal'));

                    toastr.success("OTP verified! New password sent to your email.");

                    alert("Your new password: " + json.newPassword);

                } else {
                    $("#otpError")
                        .removeClass("hidden")
                        .text(json.message || "Invalid or expired OTP.");
                }
            },

            error: function () {
                $("#otpError")
                    .removeClass("hidden")
                    .text("Server error. Please try again.");
            }
        });
    });

});