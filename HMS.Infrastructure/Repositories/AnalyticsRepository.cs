using AutoMapper;
using Dapper;
using HMS.Application.DTO.Analytics;
using HMS.Application.Interfaces;
using HMS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;
        private readonly IMapper _mapper;

        public AnalyticsRepository(IUnitOfWork unitOfWork, DapperContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<DashboardAnalyticsDto> GetDashboardAnalyticsAsync()
        {
            using var connection = _context.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "sp_GetDashboardAnalytics",
                commandType: CommandType.StoredProcedure
            );

            var dto = new DashboardAnalyticsDto();

            // 1️⃣  HIGH-LEVEL COUNTS
            var counts = await multi.ReadFirstOrDefaultAsync<dynamic>();
            if (counts != null)
            {
                dto.TotalPatients = counts.TotalPatients;
                dto.TotalDoctors = counts.TotalDoctors;
                dto.TotalAppointments = counts.TotalAppointments;
                dto.TotalDepartments = counts.TotalDepartments;
                dto.TotalInsuranceRecords = counts.TotalInsuranceRecords;
                dto.TotalFeedback = counts.TotalFeedback;
                dto.TotalWards = counts.TotalWards;
                dto.TotalUsers = counts.TotalUsers;
            }

            // 2️⃣  APPOINTMENTS OVER TIME
            dto.AppointmentTrends = (await multi.ReadAsync<AppointmentTrendItem>()).AsList();

            // 3️⃣  PATIENT REGISTRATIONS OVER TIME
            dto.PatientTrends = (await multi.ReadAsync<PatientTrendItem>()).AsList();

            // 4️⃣  FEEDBACK ANALYTICS
            dto.DoctorFeedbacks = (await multi.ReadAsync<DoctorFeedbackItem>()).AsList();

            // 5️⃣  DEPARTMENT-WISE DOCTOR DISTRIBUTION
            dto.DepartmentDoctorDistribution = (await multi.ReadAsync<DepartmentDoctorItem>()).AsList();

            // 6️⃣  WARD UTILIZATION
            dto.WardUtilization = (await multi.ReadAsync<WardUtilizationItem>()).AsList();

            // 7️⃣  INSURANCE EXPIRY ANALYTICS
            var expiringCount = await multi.ReadFirstOrDefaultAsync<dynamic>();
            if (expiringCount != null)
                dto.ExpiringSoonCount = expiringCount.ExpiringSoonCount;

            dto.ExpiringInsuranceList = (await multi.ReadAsync<ExpiringInsuranceItem>()).AsList();

            // 8️⃣  GENDER DISTRIBUTION
            dto.GenderDistribution = (await multi.ReadAsync<GenderDistributionItem>()).AsList();

            // 9️⃣  TOP DOCTORS BY APPOINTMENTS
            dto.TopDoctors = (await multi.ReadAsync<TopDoctorItem>()).AsList();

            return dto;
        }
    }
}
