using HMS.Application.Commands;
using HMS.Application.DTO.MedicalHistory;
using HMS.Application.DTO.Patient;
using HMS.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HMS.Application.Commands.MedicalHistory;
using HMS.Application.Queries.MedicalHistory;

namespace HMS.Web.Controllers
{
    public class MedicalHistoryController : Controller
    {
        private readonly IMediator _mediator;

        public MedicalHistoryController(IMediator Mediator)
        {
            _mediator = Mediator;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMedicalHistory([FromForm] CreateMedicalHistoryDto dto)
        {
            var command = new AddMedicalHistoryCommand(dto);
            var result = await _mediator.Send(command);

            if (result > 0)
                return Json(new { success = true, id = result });

            return BadRequest("Failed to add medical history.");
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicalHistoryByPatient(int id)
        {
            var query = new GetPatientMedicalHistoryQuery(id);
            var result = await _mediator.Send(query);

            if (result == null || !result.Any())
                return NotFound(new { message = "No medical history records found for this patient." });

            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateMedicalHistory([FromBody] EditMedicalHistoryDto dto)
        {
            if (dto == null || dto.Id <= 0)
                return BadRequest(new { message = "Invalid medical history data." });
            try
            {
                var command = new UpdateMedicalHistoryCommand(dto);
                var result = await _mediator.Send(command);
                // Always return Ok if updatedRows > 0
                return Ok(new { message = "Medical history updated successfully." });
            }
            catch (Exception ex)
            {
                // log exception here
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMedicalHistory([FromBody] int id)
        {
            if (id <= 0) return BadRequest(new { message = "Invalid id." });

            try
            {
                var result = await _mediator.Send(new DeleteMedicalHistoryCommand(id));

                if (!result)
                    return StatusCode(500, new { message = "No row was deleted (id might not exist)." });

                return Ok(new { message = "Medical history deleted successfully." });
            }
            catch (Exception ex)
            {
                // return the exception message to frontend (for debugging)
                return StatusCode(500, new { message = ex.Message });
            }
        }



    }
}
