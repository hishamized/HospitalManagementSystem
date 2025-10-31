using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Analytics
{
    public class DashboardAnalyticsDto
    {
        // 1️⃣  HIGH-LEVEL COUNTS
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalInsuranceRecords { get; set; }
        public int TotalFeedback { get; set; }
        public int TotalWards { get; set; }
        public int TotalUsers { get; set; }

        // 2️⃣  APPOINTMENTS OVER TIME
        public List<AppointmentTrendItem> AppointmentTrends { get; set; } = new();

        // 3️⃣  PATIENT REGISTRATIONS OVER TIME
        public List<PatientTrendItem> PatientTrends { get; set; } = new();

        // 4️⃣  FEEDBACK ANALYTICS (AVERAGE RATING)
        public List<DoctorFeedbackItem> DoctorFeedbacks { get; set; } = new();

        // 5️⃣  DEPARTMENT-WISE DOCTOR DISTRIBUTION
        public List<DepartmentDoctorItem> DepartmentDoctorDistribution { get; set; } = new();

        // 6️⃣  WARD UTILIZATION
        public List<WardUtilizationItem> WardUtilization { get; set; } = new();

        // 7️⃣  INSURANCE EXPIRY ANALYTICS
        public int ExpiringSoonCount { get; set; }
        public List<ExpiringInsuranceItem> ExpiringInsuranceList { get; set; } = new();

        // 8️⃣  GENDER DISTRIBUTION
        public List<GenderDistributionItem> GenderDistribution { get; set; } = new();

        // 9️⃣  TOP DOCTORS BY APPOINTMENTS
        public List<TopDoctorItem> TopDoctors { get; set; } = new();
    }

    // 🔹 Sub-item definitions for collections

    public class AppointmentTrendItem
    {
        public string Month { get; set; }
        public int AppointmentCount { get; set; }
    }

    public class PatientTrendItem
    {
        public string Month { get; set; }
        public int PatientCount { get; set; }
    }

    public class DoctorFeedbackItem
    {
        public string DoctorName { get; set; }
        public double AverageRating { get; set; }
        public int TotalFeedbacks { get; set; }
    }

    public class DepartmentDoctorItem
    {
        public string DepartmentName { get; set; }
        public int DoctorCount { get; set; }
    }

    public class WardUtilizationItem
    {
        public string WardName { get; set; }
        public int Capacity { get; set; }
        public int OccupiedBeds { get; set; }
        public int AvailableBeds { get; set; }
        public decimal OccupancyRate { get; set; }
    }

    public class ExpiringInsuranceItem
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string ProviderName { get; set; }
        public string PolicyNumber { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GenderDistributionItem
    {
        public string Gender { get; set; }
        public int PatientCount { get; set; }
    }

    public class TopDoctorItem
    {
        public string DoctorName { get; set; }
        public int AppointmentCount { get; set; }
    }
}
