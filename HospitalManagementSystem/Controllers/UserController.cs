using HMS.Application.DTOs;
using HMS.Application.DTOs.Users;
using HMS.Application.Features.Users.Commands.CreateAdmin;
using HMS.Application.Features.Users.Queries.GetAllAdmins;
using HMS.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create() => View();

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login() => View();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            // Validate user
            var query = new LoginQuery(dto);
            var user = await _mediator.Send(query);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username/email or password");
                return View(dto);
            }

            // ----------------------------
            // 1️⃣ Store info in session
            // ----------------------------
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Roles", string.Join(",", user.Roles));

            // ----------------------------
            // 2️⃣ Create claims for cookie
            // ----------------------------
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = dto.RememberMe, // cookie persists if RememberMe checked
                ExpiresUtc = dto.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : null
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            // ----------------------------
            // 3️⃣ Redirect based on role
            // ----------------------------
            if (user.Roles.Contains("Admin")) return RedirectToAction("AdminDashboard");
            if (user.Roles.Contains("Doctor")) return RedirectToAction("DoctorDashboard");
            if (user.Roles.Contains("Nurse")) return RedirectToAction("NurseDashboard");
            if (user.Roles.Contains("Receptionist")) return RedirectToAction("ReceptionistDashboard");
            if (user.Roles.Contains("Pharmacist")) return RedirectToAction("PharmacistDashboard");

            return RedirectToAction("Login"); // fallback
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Sign out the cookie-based authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDto model)
        {
            try
            {
                // 🧩 Construct the command
                var command = new CreateAdminCommand(model);

                // ✅ Send the command to MediatR pipeline (validators will run automatically)
                var result = await _mediator.Send(command);

                // If handler returned a success message
                if (result.Success)
                {
                    return Json(new
                    {
                        success = true,
                        message = result.Message
                    });
                }

                // If operation failed but not due to validation
                return Json(new
                {
                    success = false,
                    message = result.Message
                });
            }
            catch (FluentValidation.ValidationException ex)
            {
                // 🧾 Handle FluentValidation errors specifically
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                return Json(new
                {
                    success = false,
                    message = "Validation failed.",
                    errors
                });
            }
            catch (Exception ex)
            {
                // ⚠️ General exception handler
                return Json(new
                {
                    success = false,
                    message = $"An unexpected error occurred: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAdmins()
        {
            try
            {
                var admins = await _mediator.Send(new GetAllAdminsQuery());

                if (admins == null || !admins.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "No admins found."
                    });
                }

                return Json(new
                {
                    success = true,
                    data = admins
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error retrieving admins: {ex.Message}"
                });
            }
        }
        // Role-based dashboard actions
        public IActionResult AdminDashboard()
        {
            return View(); // Views/User/AdminDashboard.cshtml
        }

        public IActionResult DoctorDashboard()
        {
            return View(); // Views/User/DoctorDashboard.cshtml
        }

        public IActionResult NurseDashboard()
        {
            return View(); // Views/User/NurseDashboard.cshtml
        }

        public IActionResult ReceptionistDashboard()
        {
            return View(); // Views/User/ReceptionistDashboard.cshtml
        }

        public IActionResult PharmacistDashboard()
        {
            return View(); // Views/User/PharmacistDashboard.cshtml
        }

        public IActionResult Index()
        {
            return View(); // Default landing page
        }
    }
}
