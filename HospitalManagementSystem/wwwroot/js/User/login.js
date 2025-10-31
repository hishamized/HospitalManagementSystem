$(document).ready(function () {

    // --------------------------
    // STEP 1: Open Forgot Password Modal
    // --------------------------
    $("#forgotPasswordLink").on("click", function (e) {
        e.preventDefault();
        $("#forgotPasswordModal").modal("show");
    });

    // --------------------------
    // STEP 2: Verify Email + Phone
    // --------------------------
    $("#forgotPasswordForm").on("submit", function (e) {
        e.preventDefault();

        const email = $("#fpEmail").val().trim();
        const phone = $("#fpPhone").val().trim();
        $("#otpEmail").val(email); 

        $("#fpError").addClass("d-none");

        $.ajax({
            url: "/User/VerifyEmailPhone",
            method: "POST",
            contentType: "application/json",      // <-- Tells server it's JSON
            data: JSON.stringify({                // <-- Converts JS object to JSON string
                email: email,
                phoneNumber: phone
            }),
            success: function (json) {
                if (json.success) {
                    $("#forgotPasswordModal").modal("hide");
                    toastr.success("Verification successful! OTP sent to your email.");
                    $("#otpModal").modal("show");
                } else {
                    $("#fpError").removeClass("d-none").text(json.message || "Verification failed.");
                }
            },
            error: function (xhr) {
                console.error(xhr);
                $("#fpError").removeClass("d-none").text("Server error. Please try again.");
            }
        });
    });


    // --------------------------
    // STEP 3: Verify OTP
    // --------------------------
    $("#otpForm").on("submit", function (e) {
        e.preventDefault();

        const otpCode = $("#otpCode").val().trim();
        const email = $("#otpEmail").val().trim();
        $("#otpError").addClass("d-none");

        if (!otpCode) {
            $("#otpError").removeClass("d-none").text("Please enter your OTP.");
            return;
        }

        $.ajax({
            url: "/User/VerifyOtp",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify({ otpCode: otpCode , Email: email}), // ✅ send as object with property name matching backend
            success: function (json) {
                if (json.success) {
                    $("#otpModal").modal("hide");
                    toastr.success("OTP verified! A new password has been generated and sent to your email.");
                    alert("Your new password: " + json.newPassword);
                } else {
                    $("#otpError").removeClass("d-none").text(json.message || "Invalid or expired OTP.");
                }
            },
            error: function () {
                $("#otpError").removeClass("d-none").text("Server error. Please try again.");
            }
        });
    });




});
