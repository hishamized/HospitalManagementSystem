$(document).ready(function () {

    // Open modal on button click
    $("#btnAddAppointment").click(function () {
        $("#addAppointmentModal").modal("show");
        loadPatients();
        loadDoctors();
    });

    // Load Patients via AJAX
    function loadPatients() {
        $.ajax({
            url: '/Patient/GetAllPatients',
            type: 'GET',
            success: function (data) {
                let patientSelect = $("#PatientId");
                patientSelect.empty().append('<option value="">Select Patient</option>');
                data.forEach(function (patient) {
                    patientSelect.append(`<option value="${patient.id}">${patient.fullName}</option>`);
                });
            },
            error: function (err) {
                console.error("Error loading patients:", err);
            }
        });
    }

    // Load Doctors via AJAX
    function getDaysFromBitmap(bitmap) {
        const dayMap = [
            { bit: 1, name: "Sunday" },
            { bit: 2, name: "Monday" },
            { bit: 4, name: "Tuesday" },
            { bit: 8, name: "Wednesday" },
            { bit: 16, name: "Thursday" },
            { bit: 32, name: "Friday" },
            { bit: 64, name: "Saturday" },
        ];

        let days = [];
        dayMap.forEach(d => {
            if ((bitmap & d.bit) === d.bit) {
                days.push(d.name);
            }
        });
        return days;
    }

    let doctorSlots = {};
    function loadDoctors() {
        $.ajax({
            url: '/Doctor/GetAllDoctors',
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    console.log(response.data);

                    let doctorSelect = $("#DoctorId");
                    doctorSelect.empty().append('<option value="">Select Doctor</option>');

                    response.data.forEach(function (doctor) {
                        doctorSelect.append(`<option value="${doctor.id}">${doctor.fullName}</option>`);

                        // Ensure slotName exists
                        if (doctor.slotName) {
                            let times = doctor.slotName.split(" - ");
                            let startHourMin = times[0] > times[1] ? times[1] : times[0]; // earlier time
                            let endHourMin = times[0] > times[1] ? times[0] : times[1];   // later time

                            doctorSlots[doctor.id] = [{
                                startTime: startHourMin,
                                endTime: endHourMin,
                                daysOfWeek: doctor.daysOfWeek ? getDaysFromBitmap(doctor.daysOfWeek) : []

                            }];

                        } else {
                            doctorSlots[doctor.id] = [];
                        }
                    });

                } else {
                    console.error("No doctors found.");
                }
            },
            error: function (err) {
                console.error("Error loading doctors:", err);
            }
        });
    }

    $("#DoctorId").change(function () {
        let doctorId = parseInt($(this).val());
        let container = $("#doctorSlotsContainer");
        let slotsDiv = $("#doctorSlots");

        slotsDiv.empty();

        if (!doctorId) {
            container.hide();
            return;
        }

        let slots = doctorSlots[doctorId] || [];
        if (slots.length === 0) {
            slotsDiv.append('<span class="text-danger">No slots available for this doctor.</span>');
            container.show();
            return;
        }

        // Display each slot as badge
        slots.forEach(function (slot) {
            slot.daysOfWeek.forEach(function (day) {
                slotsDiv.append(`<span class="badge bg-success">${day}: ${slot.startTime} - ${slot.endTime}</span>`);
            });
        });



        container.show();
    });



    // Submit form via AJAX
    $("#appointmentForm").submit(function (e) {
        e.preventDefault();

        let patientId = parseInt($("#PatientId").val());
        let doctorId = parseInt($("#DoctorId").val());
        let appointmentDate = new Date($("#AppointmentDate").val());
        let status = $("#Status").val();
        let remarks = $("#Remarks").val();

        if (!doctorId || !patientId) {
            alert("Please select both patient and doctor.");
            return;
        }

        // Validate against doctor's slots
        let validSlot = false;
        let slots = doctorSlots[doctorId] || [];
        let dayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
        let appointmentDay = dayNames[appointmentDate.getDay()]; // get day string

        for (let slot of slots) {
            if (slot.daysOfWeek.includes(appointmentDay)) {
                let slotStart = new Date(appointmentDate);
                let [startHour, startMin] = slot.startTime.split(":");
                slotStart.setHours(parseInt(startHour), parseInt(startMin), 0, 0);

                let slotEnd = new Date(appointmentDate);
                let [endHour, endMin] = slot.endTime.split(":");
                slotEnd.setHours(parseInt(endHour), parseInt(endMin), 0, 0);

                if (appointmentDate >= slotStart && appointmentDate < slotEnd) {
                    validSlot = true;
                    break;
                }
            }
        }


        if (!validSlot) {
            alert("Selected appointment time is outside the doctor's available slots.");
            return;
        }

        // Submit AJAX if valid
        let appointmentData = {
            PatientId: patientId,
            DoctorId: doctorId,
            AppointmentDate: $("#AppointmentDate").val(),
            Status: status,
            Remarks: remarks
        };

        $.ajax({
            url: '/Appointment/Add',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(appointmentData),
            success: function (res) {
                if (res.success) {
                    alert(res.message);
                    $("#addAppointmentModal").modal("hide");
                    $("#appointmentForm")[0].reset();
                    fetchAppointments();
                } else {
                    alert("Error: " + res.message);
                    console.error(res.errors);
                }
            },
            error: function (err) {
                console.error("AJAX error:", err);
            }
        });
    });
    // AG Grid Options
    let gridOptions = {
        columnDefs: [
             { field: "id", headerName: "ID" },
            { field: "patientFullName", headerName: "Patient" },
            { field: "doctorFullName", headerName: "Doctor" },
            { field: "appointmentDate", headerName: "Date & Time" },
            { field: "status", headerName: "Status" },
            { field: "remarks", headerName: "Remarks" },
            {
                headerName: "Actions",
                cellRenderer: function (params) {
                    return `
                <button class="btn btn-sm btn-warning rescheduleBtn">Reschedule</button>
                <button class="btn btn-sm btn-danger deleteBtn">Delete</button>
            `;
                }
            }
        ]
,
        defaultColDef: { flex: 1, sortable: true, filter: true },
        rowData: [],
        getRowNodeId: data => data.id,
        onGridReady: function () { fetchAppointments(); },
        onCellClicked: function (params) {
            if ($(params.event.target).hasClass("rescheduleBtn")) {
                openRescheduleModal(params.data);
            } else if ($(params.event.target).hasClass("deleteBtn")) {
                if (confirm("Are you sure you want to delete this appointment?")) {
                    deleteAppointment(params.data.id);
                }
            }
        },
        rowContextMenuItems: function (params) {
            return [
                {
                    name: 'Reschedule',
                    action: () => openRescheduleModal(params.node.data)
                },
                {
                    name: 'Delete',
                    action: () => {
                        if (confirm("Are you sure you want to delete this appointment?")) {
                            deleteAppointment(params.node.data.id);
                        }
                    }
                }
            ];
        }
    };

    // Initialize AG Grid
    new agGrid.Grid(document.getElementById("appointmentGrid"), gridOptions);

    // Fetch appointments from server
    function fetchAppointments() {
        $.ajax({
            url: '/Appointment/GetAll',
            type: 'GET',
            success: function (res) {
                if (res.success) {
                    console.log(res.data);
                    gridOptions.api.setRowData(res.data);
                } else {
                    alert("Failed to fetch appointments.");
                }
            }
        });
    }

    // Open Reschedule Modal and populate data
    function openRescheduleModal(data) {
        $("#RescheduleAppointmentId").val(data.id);
        $("#ReschedulePatient").val(data.patientFullName);
        $("#RescheduleRemarks").val(data.remarks);

        // First load doctors
        $.ajax({
            url: '/Doctor/GetAllDoctors',
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    $("#RescheduleDoctor").empty().append('<option value="">Select Doctor</option>');
                    response.data.forEach(d => {
                        $("#RescheduleDoctor").append(`<option value="${d.id}" ${d.id === data.doctorId ? "selected" : ""}>${d.fullName}</option>`);

                        // populate doctorSlots for reschedule
                        if (d.slotName) {
                            let times = d.slotName.split(" - ");
                            let startHourMin = times[0] > times[1] ? times[1] : times[0];
                            let endHourMin = times[0] > times[1] ? times[0] : times[1];
                            doctorSlots[d.id] = [{
                                startTime: startHourMin,
                                endTime: endHourMin,
                                daysOfWeek: d.daysOfWeek ? getDaysFromBitmap(d.daysOfWeek) : []
                            }];
                        } else {
                            doctorSlots[d.id] = [];
                        }
                    });

                    // Now show slots for the selected doctor
                    showDoctorSlots(data.doctorId, "#rescheduleDoctorSlots");
                }
            }
        });

        $("#RescheduleDate").val(data.appointmentDate.split("T")[0] + "T" + data.appointmentDate.split("T")[1].substring(0, 5)); // prefill
        $("#rescheduleModal").modal("show");
    }


    // When doctor changes in reschedule modal, show available slots
    $("#RescheduleDoctor").change(function () {
        let doctorId = $(this).val();
        showDoctorSlots(doctorId, "#rescheduleDoctorSlots");
    });

    function showDoctorSlots(doctorId, containerSelector) {
        let container = $(containerSelector);
        container.empty();
        let slots = doctorSlots[doctorId] || [];
        if (!slots.length) {
            container.append('<span class="text-danger">No slots available for this doctor.</span>');
            return;
        }
        slots.forEach(function (slot) {
            slot.daysOfWeek.forEach(day => {
                container.append(`<span class="badge bg-success me-1">${day}: ${slot.startTime} - ${slot.endTime}</span>`);
            });
        });
    }

    // Submit reschedule form
    $("#rescheduleForm").submit(function (e) {
        e.preventDefault();

        let appointmentData = {
            Id: parseInt($("#RescheduleAppointmentId").val()),
            DoctorId: parseInt($("#RescheduleDoctor").val()),
            AppointmentDate: $("#RescheduleDate").val(),
            Remarks: $("#RescheduleRemarks").val()
        };


        $.ajax({
            url: '/Appointment/Reschedule',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(appointmentData),
            success: function (res) {
                if (res.success) {
                    $("#rescheduleModal").modal("hide");
                    fetchAppointments();
                } else {
                    if (res.errors) {
                        // Validation errors
                        let errorMsg = res.errors.map(e => `${e.propertyName}: ${e.errorMessage}`).join("\n");
                        alert("Validation errors:\n" + errorMsg);
                    } else {
                        alert("Error: " + res.message);
                    }
                }
            },
            error: function (xhr, status, error) {
                let response = xhr.responseJSON;
                if (response && response.errors) {
                    let errorMsg = response.errors.map(e => `${e.propertyName}: ${e.errorMessage}`).join("\n");
                    alert("Validation errors:\n" + errorMsg);
                } else if (response && response.message) {
                    alert("Server error: " + response.message);
                } else {
                    alert("Unexpected error occurred: " + error);
                }
                console.error("AJAX error:", xhr, status, error);
            }
        });
    });


    // Delete appointment
    function deleteAppointment(id) {
        if (!id || id <= 0) {
            alert("Invalid appointment ID.");
            return;
        }

        $.ajax({
            url: `/Appointment/Delete/${id}`,
            type: 'DELETE',
            contentType: 'application/json', // optional here, no body is sent
            success: function (res) {
                if (res.success) {
                    alert(res.message);
                    fetchAppointments(); // refresh AG Grid
                } else {
                    alert(res.message || "Failed to delete appointment.");
                    console.error(res);
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX error:", xhr, status, error);
                alert("Server error occurred while deleting the appointment.");
            }
        });
    }



});

