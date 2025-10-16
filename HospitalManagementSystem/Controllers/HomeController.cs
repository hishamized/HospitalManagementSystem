namespace HMS.Domain.Entities
{
    using HMS.Infrastructure.Data;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;


    namespace HospitalManagementSystem.Controllers
    {
        public class HomeController : Controller
        {
            private readonly ILogger<HomeController> _logger;
            private readonly ApplicationDbContext _context;

            public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
            {
                _logger = logger;
                _context = context;
            }

            public IActionResult Index()
            {
                return View();
            }

            public IActionResult Privacy()
            {
                return View();
            }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            //public IActionResult Error()
            //{
            //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            //}
            // GET: /Home/CheckDb
            public IActionResult CheckDb()
            {
                try
                {
                    // Try opening the database connection
                    var canConnect = _context.Database.CanConnect();

                    if (canConnect)
                        return Content("Database connection successful!");
                    else
                        return Content("Database exists but cannot connect.");
                }
                catch (SqlException ex)
                {
                    return Content($"Database connection failed: {ex.Message}");
                }
            }
        }
    }

}