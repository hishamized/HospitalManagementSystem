// availability.js

// make allDoctors global so all functions can access it
var allDoctors = [];

$(function () {
    // Initialize Select2 (searchable dropdown)
    $("#doctorSelect").select2({
        placeholder: "Search and select a doctor",
        allowClear: true,
        width: "100%"
    });

    // Load all doctors when the page loads
    loadDoctors();

    // When doctor is selected
    $("#doctorSelect").on("change", function () {
        const val = $(this).val();
        if (!val) {
            $("#availabilityContainer").hide();
            return;
        }

        const doctorId = parseInt(val, 10);
        if (isNaN(doctorId)) {
            console.warn("Selected doctorId is not a number:", val);
            $("#availabilityContainer").hide();
            return;
        }

        const selectedDoctor = allDoctors.find(d => d.id === doctorId);
        if (selectedDoctor) {
            renderAvailabilityGrid(selectedDoctor);
        } else {
            console.warn("Selected doctor not found in allDoctors:", doctorId);
            $("#availabilityContainer").hide();
        }
    });
});

// 🩺 Load all doctors from backend
function loadDoctors() {
    $.ajax({
        url: "/Doctor/GetAllDoctors",
        type: "GET",
        dataType: "json",
        success: function (response) {
            //console.log("Doctors Response:", response);

            const doctors = (response && response.data) ? response.data : [];
            allDoctors = doctors; // store globally

            const $select = $("#doctorSelect");
            $select.empty().append('<option value="">-- Select a Doctor --</option>');

            if (doctors.length > 0) {
                $.each(doctors, function (i, doc) {
                    // ensure id exists and is numeric
                    const id = doc.id ?? doc.Id ?? doc.ID;
                    const name = doc.fullName ?? doc.FullName ?? `${doc.firstName ?? ''} ${doc.lastName ?? ''}`.trim();
                    const spec = doc.specialization ?? doc.Specialization ?? 'N/A';

                    $select.append(
                        `<option value="${id}">${escapeHtml(name)} (${escapeHtml(spec)})</option>`
                    );
                });

                // If Select2 is already initialized, refresh it so it knows about new options
                if ($select.hasClass("select2-hidden-accessible")) {
                    $select.trigger("change.select2");
                }
            } else {
                console.warn("No doctors found");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error loading doctors:", error, xhr.responseText);
        }
    });
}

// 🧱 Render the availability grid for selected doctor
function renderAvailabilityGrid(doctor) {
    // debug
    console.log("Rendering availability for doctor:", doctor);
    // optional alert for debugging (remove in production)
    // alert("renderAvailabilityGrid called for " + (doctor.fullName || doctor.FullName || doctor.name));

    $("#availabilityContainer").show();
    const $tableBody = $("#availabilityTableBody");
    $tableBody.empty();

    // Build slot info safely from doctor data
    // support different property name casings and formats
    const slotName = doctor.slotName ?? doctor.SlotName ?? "";
    const daysStr = doctor.daysOfWeek ?? doctor.DaysOfWeek ?? "0";

    // reportingTime / leavingTime — parse from slotName "09:00 - 17:00"
    const parts = slotName.split('-').map(p => p.trim()).filter(p => p.length);
    let reportingTime = parts[0] ?? "09:00";
    let leavingTime = parts[1] ?? "17:00";

    // if slotName is reversed like "15:00 - 12:00" we'll handle below
    const slot = {
        reportingTime: reportingTime,
        leavingTime: leavingTime,
        daysOfWeek: parseInt(daysStr, 10) || 0
    };

    const dayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

    // Convert bitmask to activeDays array
    const activeDays = [];
    for (let i = 0; i < dayNames.length; i++) {
        if ((slot.daysOfWeek & (1 << i)) !== 0) {
            activeDays.push(dayNames[i]);
        }
    }

    // Parse hours (hour numbers)
    let startHour = parseInt(slot.reportingTime.split(':')[0], 10);
    let endHour = parseInt(slot.leavingTime.split(':')[0], 10);

    // sanity defaults
    if (isNaN(startHour)) startHour = 9;
    if (isNaN(endHour)) endHour = 17;

    // handle reversed ranges (e.g., "15:00 - 12:00")
    if (endHour <= startHour) {
        // assume the shift crosses midnight or was reversed — try to interpret as start earlier than end
        // Common case in your data might be "15:00 - 12:00" meaning 12:00 to 15:00? unclear.
        // We'll assume they meant "12:00 - 15:00" if leaving < reporting.
        const tmp = startHour;
        startHour = endHour;
        endHour = tmp;
        // if still invalid, fallback to defaults
        if (endHour <= startHour) {
            startHour = 9;
            endHour = 17;
        }
    }

    // Build grid rows: Sunday ... Saturday, columns 8..20
    for (let d = 0; d < dayNames.length; d++) {
        const day = dayNames[d];
        const $tr = $("<tr>");
        const $dayCell = $("<td>").text(day).addClass("fw-bold bg-light");
        $tr.append($dayCell);

        for (let hour = 0; hour < 24; hour++) {

            const $td = $("<td>");
            // available if this day is active AND hour is inside [startHour, endHour)
            if (activeDays.indexOf(day) !== -1 && hour >= startHour && hour < endHour) {
                $td.addClass("available-box").text("✓");
            } else {
                $td.addClass("unavailable-box").text("—");
            }
            $tr.append($td);
        }

        $tableBody.append($tr);
    }
}

// small helper to escape html in option text
function escapeHtml(text) {
    if (!text && text !== 0) return "";
    return text.toString()
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}
