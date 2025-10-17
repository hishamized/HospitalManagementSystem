using HMS.Application.Handlers;
using HMS.Application.Interfaces;
using HMS.Domain.Interfaces;
using HMS.Infrastructure.Data;
using HMS.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using HMS.Application.Mappings;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(LoginQueryHandler).Assembly));
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<GetAllPatientsQueryHandler>());

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IMedicalHistoryRepository, MedicalHistoryRepository>();
builder.Services.AddScoped<IAllergyRepository, AllergyRepository>();
builder.Services.AddScoped<IInsuranceRepository, InsuranceRepository>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDistributedMemoryCache(); // Required for in-memory session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // necessary for GDPR compliance
});


builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    SeedData.Initialize(dbContext);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
