using HMS.Application.Commands.PatientPortal;
using HMS.Application.DTO.PatientPortal;
using HMS.Application.Queries.Bill;
using HMS.Application.Queries.Patient;
using HMS.Application.Queries.PatientPortal;
using HMS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HMS.Web.Controllers
{
    [Authorize]
    public class PatientPortalController : Controller
    {
        private readonly IMediator _mediator;

        public PatientPortalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        public IActionResult PatientPortal()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult SearchPatientByIdentifier(string identifier)
        {
            try
            {
                if (identifier != null)
                {
                    var command = new SearchPatientByIdentifierQuery(identifier);
                    var result = _mediator.Send(command);
                    if (result != null)
                    {
                        return Json(new { success = true, data = result.Result });
                    }
                    else
                    {
                        return Json(new { success = false, message = "No patient found" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "No patient found" });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    Error = ex.Message
                });
            }
        }

        [AllowAnonymous]
        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
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
        public IActionResult ViewManageVisits()
        {
            string patientId = HttpContext.Session.GetString("PatientId")!;
            return View("ViewManageVisits", patientId);
        }
        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> FetchPatientVisits()
        {
            // Fix: Parse the string value to long
            var patientIdClaimValue = User.FindFirst("PatientId")?.Value;
            if (!long.TryParse(patientIdClaimValue, out var patientIdClaim))
            {
                return BadRequest(new { success = false, message = "Invalid patient ID in claim." });
            }

            try
            {
                var query = new GetPatientVisitsPortalQuery(patientIdClaim);

                var _result = await _mediator.Send(query);

                return Ok( new { success = true, result = _result} );
            }
            catch (Exception ex) {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetPatientBills(long visitId) {
            try
            {
                var patientIdClaimValue = User.FindFirst("PatientId")?.Value;
                if (!long.TryParse(patientIdClaimValue, out var PatiendId))
                {
                    return BadRequest(new { success = false, message = "Invalid patient ID in claim." });
                }

                var query = new GetPatientBillsQuery(PatiendId, visitId);

                var _result = await _mediator.Send(query);

                return Ok(new { success = true, result = _result });
            }
            catch (Exception ex) {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
       
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> GenerateDischargeSummary(int visitId)
        {
            try
            {
                var summary = await _mediator.Send(new GetDischargeSummaryQuery(visitId));
                if (summary == null)
                    return NotFound(new
                    {
                        status = 404,
                        success = false,
                        message = $"No discharge summary found for VisitId {visitId}"
                    });

                return Ok(new
                {
                    status = 200,
                    success = true,
                    data = summary
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Server error: {ex.Message}"
                });
            }
        }
    }
}
