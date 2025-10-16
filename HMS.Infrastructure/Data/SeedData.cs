using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace HMS.Infrastructure.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Apply migrations if not applied yet
            context.Database.Migrate();

            // List of default roles
            var defaultRoles = new[]
            {
                "Admin",
                "Doctor",
                "Nurse",
                "Receptionist",
                "Pharmacist"
                // Add more roles here if needed
            };

            // Seed roles
            foreach (var roleName in defaultRoles)
            {
                if (!context.Roles.Any(r => r.Name == roleName))
                {
                    var role = new Role
                    {
                        Name = roleName,
                        Description = $"{roleName} role for HMS system"
                    };
                    context.Roles.Add(role);
                }
            }
            context.SaveChanges();

            // Seed default admin user
            if (!context.Users.Any(u => u.Username == "admin"))
            {
                // Hash the password "qwertyuiop" using BCrypt
                string passwordHash = BCrypt.Net.BCrypt.HashPassword("qwertyuiop");

                var adminUser = new User
                {
                    FullName = "System Administrator",
                    Email = "admin@hospital.com",
                    Username = "admin",
                    ContactNumber = "0000000000",
                    PasswordHash = passwordHash,
                    IsActive = true,
                    Gender = "Other"
                };
                context.Users.Add(adminUser);
                context.SaveChanges();

                // Assign admin role
                var adminRole = context.Roles.First(r => r.Name == "Admin");
                context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                });
                context.SaveChanges();
            }
        }
    }
}
