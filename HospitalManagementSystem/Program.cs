using AutoMapper;
using FluentValidation;
using HMS.Application.Behaviors;
using HMS.Application.Features.Appointments.Validators;
using HMS.Application.Handlers;
using HMS.Application.Mappings;
using HMS.Application.Validators.Appointment;
using HMS.Application.Validators.Department;
using HMS.Application.Validators.Doctor;
using HMS.Application.Validators.PatientVisit;
using HMS.Application.Validators.Slot;
using HMS.Infrastructure;
using HMS.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// MVC + FluentValidation
builder.Services.AddControllersWithViews();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddValidatorsFromAssemblyContaining<AddPatientVisitCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdatePatientVisitCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddDoctorCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EditDoctorCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddDepartmentCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EditDepartmentCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddSlotCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EditSlotCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddAppointmentCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RescheduleAppointmentCommandValidator>();


// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(LoginQueryHandler).Assembly, typeof(GetAllPatientsQueryHandler).Assembly)
);

// Validation Pipeline
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Application-specific infrastructure
builder.Services.AddInfrastructure();

// AutoMapper
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

// EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login"; // redirect if not authenticated
        options.AccessDeniedPath = "/Account/AccessDenied"; // redirect if unauthorized
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization();

// Build app
var app = builder.Build();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    SeedData.Initialize(dbContext);
}

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
