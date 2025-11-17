using HMS.Application.Commands;
using HMS.Application.DTO.Patient;
using HMS.Application.Queries;
using HMS.Application.Queries.Patient;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace HMS.Web.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class PatientController : Controller
    {
        private readonly IMediator _mediator;
        public PatientController(IMediator mediator)
        {
            _mediator = mediator;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Patients() {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _mediator.Send(new GetAllPatientsQuery());
            return Ok(patients);
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var patient = await _mediator.Send(new GetPatientByIdQuery(id));
            return Ok(patient);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto dto)
        {
            var id = await _mediator.Send(new CreatePatientCommand(dto));
            return Ok(id);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePatient([FromBody] UpdatePatientDto dto)
        {
            var result = await _mediator.Send(new UpdatePatientCommand(dto));
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var result = await _mediator.Send(new DeletePatientCommand(id));
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> ViewPatient(int id)
        {
            var query = new GetPatientByIdQuery(id);
            PatientDto patient = await _mediator.Send(query);

            if (patient == null)
                return NotFound();

            return View("ViewPatient", patient);
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
            catch (Exception ex) {
                return StatusCode(500, new
                 {
                    success = false,
                    message = $"Server error: {ex.Message}"
                });
            }
        }

    }
}
