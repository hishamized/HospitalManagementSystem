using HMS.Application.Commands;
using HMS.Application.Commands.Insurance;
using HMS.Application.DTO.Allergy;
using HMS.Application.DTO.Insurance;
using HMS.Application.DTO.MedicalHistory;
using HMS.Application.DTO.Patient;
using HMS.Application.Queries;
using HMS.Application.Queries.Insurance;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers
{
    public class InsuranceController : Controller
    {
        private readonly IMediator _mediator;

        public InsuranceController(IMediator Mediator)
        {
            _mediator = Mediator;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] AddInsuranceDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid form data" });
                }

                var command = new AddInsuranceCommand(dto);
                var newId = await _mediator.Send(command);

                return Json(new { success = true, insuranceId = newId });
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging
                // _logger.LogError(ex, "Error adding insurance");

                // Return JSON error to AJAX
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while adding insurance",
                    details = ex.Message // optionally include ex.StackTrace for debugging
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            if (patientId <= 0)
            {
                return BadRequest("Invalid patient ID");
            }

            try
            {
                var query = new GetInsurancesByPatientQuery(patientId);
                var insurances = await _mediator.Send(query);

                return Json(insurances); // Returns JSON array for AJAX
            }
            catch (Exception ex)
            {
                // Optional: log the exception
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error fetching insurance data",
                    details = ex.Message
                });
            }
        }

    }
}
