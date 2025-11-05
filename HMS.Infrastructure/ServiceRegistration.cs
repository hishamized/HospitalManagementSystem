using FluentValidation;
using HMS.Application.Commands.Bed;
using HMS.Application.Commands.Doctor;
using HMS.Application.Interfaces;
using HMS.Application.Validators.Bed;
using HMS.Application.Validators.Doctor;
using HMS.Domain.Interfaces;
using HMS.Infrastructure.Data;
using HMS.Infrastructure.Repositories;
using HMS.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<DapperContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IMedicalHistoryRepository, MedicalHistoryRepository>();
            services.AddScoped<IAllergyRepository, AllergyRepository>();
            services.AddScoped<IInsuranceRepository, InsuranceRepository>();
            services.AddScoped<IPatientVisitRepository, PatientVisitRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ISlotRepository, SlotRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IDoctorDocumentRepository, DoctorDocumentRepository>();
            services.AddScoped<IWardRepository, WardRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IBillRepository, BillRepository>();
            services.AddScoped<IBedRepository, BedRepository>();
            services.AddTransient<IValidator<EditBedCommand>, EditBedValidator>();
            services.AddTransient<IValidator<AddDoctorRoundCommand>, AddDoctorRoundCommandValidator>();
            services.AddTransient<IValidator<EditDoctorRoundCommand>, EditDoctorRoundCommandValidator>();

        }
    }
}
