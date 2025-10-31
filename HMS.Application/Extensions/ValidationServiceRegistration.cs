using FluentValidation;
using HMS.Application.Commands.User;
using HMS.Application.Features.Appointments.Validators;
using HMS.Application.Features.Users.Commands.CreateAdmin;
using HMS.Application.Validators.Appointment;
using HMS.Application.Validators.Department;
using HMS.Application.Validators.Doctor;
using HMS.Application.Validators.DoctorDocument;
using HMS.Application.Validators.Feedback;
using HMS.Application.Validators.PatientVisit;
using HMS.Application.Validators.Role;
using HMS.Application.Validators.Slot;
using HMS.Application.Validators.User;
using HMS.Application.Validators.Ward;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Application.Extensions
{
    public static class ValidationServiceRegistration
    {
        public static IServiceCollection AddApplicationValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<AddPatientVisitCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdatePatientVisitCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<AddDoctorCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<EditDoctorCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<AddDepartmentCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<EditDepartmentCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<AddSlotCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<EditSlotCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<AddAppointmentCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<RescheduleAppointmentCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<AddRoleCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<EditRoleCommandValidator>();
            services.AddValidatorsFromAssembly(typeof(CreateAdminCommandValidator).Assembly);
            services.AddValidatorsFromAssemblyContaining<AddDoctorDocumentCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateWardValidator>();
            services.AddValidatorsFromAssembly(typeof(UpdateWardCommandValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(CreateFeedbackCommandValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(VerifyEmailPhoneCommandValidator).Assembly);
            services.AddTransient<IValidator<VerifyOtpCommand>, VerifyOtpCommandValidator>();


            return services;
        }
    }
}
