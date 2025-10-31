$(document).ready(function () {
    loadDashboardAnalytics();
});

function loadDashboardAnalytics() {
    $.ajax({
        url: '/Analytics/GetDashboardAnalytics',
        type: 'GET',
        success: function (data) {
            if (!data) return;

            // 1️⃣ Cards
            $('#totalPatients').text(data.totalPatients);
            $('#totalDoctors').text(data.totalDoctors);
            $('#totalAppointments').text(data.totalAppointments);
            $('#totalDepartments').text(data.totalDepartments);

            // 2️⃣ Charts
            renderLineChart(
                'appointmentsChart',
                data.appointmentTrends.map(x => x.month),
                data.appointmentTrends.map(x => x.appointmentCount),
                'Appointments',
                '#727cf5'
            );

            renderLineChart(
                'patientsChart',
                data.patientTrends.map(x => x.month),
                data.patientTrends.map(x => x.patientCount),
                'Patients',
                '#39afd1'
            );

            renderBarChart(
                'departmentChart',
                data.departmentDoctorDistribution.map(x => x.departmentName),
                data.departmentDoctorDistribution.map(x => x.doctorCount),
                '#6c5ce7'
            );

            renderPieChart(
                'genderChart',
                data.genderDistribution.map(x => x.gender),
                data.genderDistribution.map(x => x.patientCount)
            );

            // 3️⃣ Top Doctors
            $('#topDoctorsList').empty();
            data.topDoctors.forEach(doc => {
                $('#topDoctorsList').append(`
                    <li class="list-group-item d-flex justify-content-between align-items-center">
                        <span><i class="uil uil-user-md text-primary me-2"></i>${doc.doctorName}</span>
                        <span class="badge bg-primary rounded-pill">${doc.appointmentCount}</span>
                    </li>
                `);
            });

            // 4️⃣ Expiring Insurance
            $('#expiringSoonCount').text(data.expiringSoonCount);
            $('#expiringInsuranceTable').empty();
            data.expiringInsuranceList.forEach(item => {
                $('#expiringInsuranceTable').append(`
                    <tr>
                        <td>${item.patientName}</td>
                        <td>${item.providerName}</td>
                        <td>${item.policyNumber}</td>
                        <td>${new Date(item.endDate).toLocaleDateString()}</td>
                    </tr>
                `);
            });

            // 5️⃣ Doctor Feedback Ratings
            if (data.doctorFeedbacks && data.doctorFeedbacks.length > 0) {
                renderBarChart(
                    'feedbackChart',
                    data.doctorFeedbacks.map(x => x.doctorName),
                    data.doctorFeedbacks.map(x => x.averageRating),
                    '#0acf97'
                );
            }

            // 6️⃣ Ward Utilization (Occupancy Rate)
            if (data.wardUtilization && data.wardUtilization.length > 0) {
                renderBarChart(
                    'wardChart',
                    data.wardUtilization.map(x => x.wardName),
                    data.wardUtilization.map(x => x.occupancyRate),
                    '#fa5c7c'
                );
            }

        },
        error: function (xhr) {
            console.error('Failed to load analytics data:', xhr.responseText);
        }
    });
}

// ===== Chart.js Enhanced Rendering =====
function renderLineChart(elementId, labels, data, label, color) {
    const ctx = document.getElementById(elementId).getContext('2d');
    const gradient = ctx.createLinearGradient(0, 0, 0, 400);
    gradient.addColorStop(0, color + '55');
    gradient.addColorStop(1, 'transparent');

    new Chart(ctx, {
        type: 'line',
        data: {
            labels,
            datasets: [{
                label,
                data,
                borderColor: color,
                backgroundColor: gradient,
                tension: 0.4,
                fill: true,
                pointBackgroundColor: color,
                borderWidth: 2
            }]
        },
        options: {
            plugins: { legend: { display: false } },
            scales: {
                y: { beginAtZero: true, grid: { color: 'rgba(0,0,0,0.05)' } },
                x: { grid: { display: false } }
            }
        }
    });
}

function renderBarChart(elementId, labels, data, color) {
    const ctx = document.getElementById(elementId).getContext('2d');
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels,
            datasets: [{
                data,
                backgroundColor: color + 'cc',
                borderRadius: 8,
            }]
        },
        options: {
            plugins: { legend: { display: false } },
            scales: { y: { beginAtZero: true } }
        }
    });
}

function renderPieChart(elementId, labels, data) {
    const ctx = document.getElementById(elementId).getContext('2d');
    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels,
            datasets: [{
                data,
                backgroundColor: ['#727cf5', '#39afd1', '#fa5c7c', '#ffbc00', '#0acf97']
            }]
        },
        options: {
            cutout: '70%',
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: { boxWidth: 12, padding: 15 }
                }
            }
        }
    });
}

