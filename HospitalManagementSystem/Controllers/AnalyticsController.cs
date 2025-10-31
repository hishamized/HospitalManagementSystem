using HMS.Application.Queries.Analytics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers
{
    [Authorize]
    public class AnalyticsController : Controller
    {
        private readonly IMediator _mediator;

        public AnalyticsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [AllowAnonymous]
        public IActionResult Analytics()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetDashboardAnalytics()
        {
            try
            {
                var result = await _mediator.Send(new GetDashboardAnalyticsQuery());

                if (result == null)
                    return NotFound(new { message = "No analytics data found." });

                return Ok(result);
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching analytics data.", details = ex.Message });
            }
        }
    }
}
