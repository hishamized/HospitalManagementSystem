using Microsoft.AspNetCore.Mvc;
using MediatR;
using HMS.Application.DTOs;
using HMS.Application.Queries;

namespace HMS.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var query = new LoginQuery(dto);
            var user = await _mediator.Send(query);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username/email or password");
                return View(dto);
            }

            // Set session
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Roles", string.Join(",", user.Roles));

            // Set cookie if RememberMe
            if (dto.RememberMe)
                Response.Cookies.Append("HMSUser", user.Username, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(7) });

            // Role-based redirect to actions within UserController
            if (user.Roles.Contains("Admin"))
                return RedirectToAction("AdminDashboard");
            if (user.Roles.Contains("Doctor"))
                return RedirectToAction("DoctorDashboard");
            if (user.Roles.Contains("Nurse"))
                return RedirectToAction("NurseDashboard");
            if (user.Roles.Contains("Receptionist"))
                return RedirectToAction("ReceptionistDashboard");
            if (user.Roles.Contains("Pharmacist"))
                return RedirectToAction("PharmacistDashboard");

            return RedirectToAction("Index"); // Default landing page
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("HMSUser");
            return RedirectToAction("Login");
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
