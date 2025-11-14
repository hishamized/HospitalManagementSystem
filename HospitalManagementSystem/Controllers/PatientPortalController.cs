using HMS.Application.Commands.PatientPortal;
using HMS.Application.DTO.PatientPortal;
using HMS.Application.Queries.PatientPortal;
using HMS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace HMS.Web.Controllers
{
    public class PatientPortalController : Controller
    {
        private readonly IMediator _mediator;

        public PatientPortalController(IMediator mediator) {
            _mediator = mediator;
        }

        [AllowAnonymous]
        public IActionResult PatientPortal()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult SearchPatientByIdentifier(string identifier) {
            try
            {
                if (identifier != null)
                {
                    var command = new SearchPatientByIdentifierQuery(identifier);
                    var result = _mediator.Send(command);
                    if(result != null) {
                        return Json(new { success = true, data = result.Result });
                    }
                    else {
                        return Json(new { success = false, message = "No patient found" });
                    }   
                }
                else {
                    return Json(new { success = false, message = "No patient found" });
                }
          
            }
            catch (Exception ex) {
                return StatusCode(500, new
                {
                    success = false,
                    Error = ex.Message
                });
            }
        }

        [AllowAnonymous]
        public IActionResult LoginPage() { 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] PatientLoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            try
            {
                // Call MediatR handler
                var query = new PatientLoginQuery(dto);
                var patient = await _mediator.Send(query);

                if (patient == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid credentials" });
                }

                // ----------------------------
                // 1️⃣ Store info in session
                // ----------------------------
                HttpContext.Session.SetString("PatientId", patient.PatientId.ToString());
                HttpContext.Session.SetString("PatientCode", patient.PatientCode);
                HttpContext.Session.SetString("FullName", patient.FullName);

                // ----------------------------
                // 2️⃣ Create claims for cookie authentication
                // ----------------------------
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, patient.PatientId.ToString()),
                    new Claim(ClaimTypes.Name, patient.FullName),
                    new Claim("PatientId", patient.PatientId.ToString()),
                    new Claim(ClaimTypes.Role, "Patient") // Role-based auth
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = dto.RememberMe,
                    ExpiresUtc = dto.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : null
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                // ----------------------------
                // 3️⃣ Return success JSON for AJAX
                // ----------------------------
                return Ok(new { success = true, message = "Login successful", redirectUrl = Url.Action("Dashboard", "PatientPortal") });
            }
            catch (Exception ex)
            {
                // Log exception if you have a logger
                // _logger.LogError(ex, "Error during patient login");

                return StatusCode(500, new { success = false, message = "Login was not successful", details = ex.Message });
            }
        }


       
        [HttpGet]
        [Authorize(Roles = "Patient")]
        public IActionResult Dashboard()
        {
            // This can be your patient dashboard page
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Clear session
                HttpContext.Session.Clear();

                // Sign out cookie authentication
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Return to Login Page
                return RedirectToAction("LoginPage", "PatientPortal");
            }
            catch (Exception ex)
            {
                // Log error if logger exists
                // _logger.LogError(ex, "Error during logout");

                return StatusCode(500, new { success = false, message = "Error logging out", details = ex.Message });
            }
        }
        [HttpGet]
        [Authorize(Roles = "Patient")]
        public IActionResult ViewManageVisits() { 
            return View();
        }
    }
}
