using HMS.Application.Commands.DoctorPortal;
using HMS.Application.DTO.DoctorPortal;
using HMS.Application.DTO.Patient;
using HMS.Application.DTO.PatientPortal;
using HMS.Application.Queries.DoctorPortal;
using HMS.Application.Queries.PatientPortal;
using HMS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Web.Controllers
{
    [Authorize]
    public class DoctorPortalController : Controller
    {
        private readonly IMediator _mediator;
        public DoctorPortalController(IMediator mediator) {
            _mediator = mediator;
        }


        [HttpGet]
        [Authorize(Roles = "Doctor")]
        public IActionResult PatientManager() {
            return View();
        }
        [AllowAnonymous]
        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Doctor")]
        public IActionResult Dashboard()
        {
            // This can be your patient dashboard page
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] DoctorLoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            try
            {
                // Call MediatR handler
                var query = new DoctorLoginQuery(dto);
                var doctor = await _mediator.Send(query);

                if (doctor == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid credentials" });
                }

                // ----------------------------
                // 1️⃣ Store info in session
                // ----------------------------
                HttpContext.Session.SetString("DoctorId", doctor.DoctorId.ToString());
                HttpContext.Session.SetString("DoctorCode", doctor.DoctorCode);
                HttpContext.Session.SetString("FirstName", doctor.FirstName);

                // ----------------------------
                // 2️⃣ Create claims for cookie authentication
                // ----------------------------
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, doctor.DoctorId.ToString()),
                    new Claim(ClaimTypes.Name, doctor.FirstName),
                    new Claim("DoctorId", doctor.DoctorId.ToString()),
                    new Claim(ClaimTypes.Role, "Doctor") // Role-based auth
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
                return Ok(new { success = true, message = "Login successful", redirectUrl = Url.Action("Dashboard", "DoctorPortal") });
            }
            catch (Exception ex)
            {
                // Log exception if you have a logger
                // _logger.LogError(ex, "Error during patient login");

                return StatusCode(500, new { success = false, message = "Login was not successful", details = ex.Message });
            }
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
                return RedirectToAction("LoginPage", "DoctorPortal");
            }
            catch (Exception ex)
            {
                // Log error if logger exists
                // _logger.LogError(ex, "Error during logout");

                return StatusCode(500, new { success = false, message = "Error logging out", details = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> FetchPatientsByDoctor() {
            var doctorIdClaim = User.FindFirst("DoctorId")?.Value;
            if (string.IsNullOrEmpty(doctorIdClaim) || !int.TryParse(doctorIdClaim, out int doctorId)) {
                return Unauthorized(new { success = false, message = "Doctor not authenticated" });
            }
            try
            {
                var command = new FetchPatientsByDoctorCommand(doctorId);
                var result = await _mediator.Send(command);
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex) {
                return StatusCode(500, new { 
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewPatient(int patientId) {
            var doctorIdClaim = User.FindFirst("DoctorId")?.Value;
            if (string.IsNullOrEmpty(doctorIdClaim) || !int.TryParse(doctorIdClaim, out int doctorId)) {
                return Unauthorized(new { success = false, message = "Doctor not authenticated" });
            }
            try
            {
                var query = new ViewPatientQuery(doctorId, patientId);
                var patientData = await _mediator.Send(query);
                if (patientData == null)
                {
                    TempData["Error"] = "Patient not found";
                    return RedirectToAction("PatientManager");
                }
                return View(patientData);
            }
            catch (Exception ex) {
                return StatusCode(500, new {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> FetchValidTests()
        {
            try
            {
                var query = new FetchValidTestsQuery();
                var result = await _mediator.Send(query);
                return Ok(new { 
                    success = true,
                    data = result
                });
            }
            catch (Exception ex) {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> SubmitLabRequest([FromBody] LabRequestDto payload)
        {
            // Get DoctorId from claims
            var doctorIdClaim = User.FindFirst("DoctorId")?.Value;
            if (string.IsNullOrEmpty(doctorIdClaim) || !int.TryParse(doctorIdClaim, out int doctorId))
            {
                return Unauthorized(new { success = false, message = "Doctor not authenticated" });
            }

            // Inject DoctorId into the main LabRequest object
            payload.LabRequest.DoctorId = doctorId;

            // Call CQRS pipeline
            var result = await _mediator.Send(new CreateLabRequestCommand(payload));

            if (result == null || result.LabRequestId <= 0)
            {
                return BadRequest(new { success = false, message = "Failed to create Lab Request" });
            }

            return Ok(new
            {
                success = true,
                message = "Lab request created successfully!",
                labRequestId = result.LabRequestId
            });
        }
    }
}
