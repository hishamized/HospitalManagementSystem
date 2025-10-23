using HMS.Application.Commands;
using HMS.Application.Commands.Allergy;
using HMS.Application.DTO.Allergy;
using HMS.Application.DTO.MedicalHistory;
using HMS.Application.DTO.Patient;
using HMS.Application.Queries;
using HMS.Application.Queries.Allergy;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HMS.Web.Controllers
{
    [Authorize]
    public class AllergyController : Controller
    {
        private readonly IMediator _mediator;

        public AllergyController(IMediator Mediator)
        {
            _mediator = Mediator;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddAllergy([FromBody] AddAllergyDto dto)
        {
            if (dto == null || dto.PatientId <= 0 || string.IsNullOrEmpty(dto.Allergen))
                return BadRequest(new { message = "Invalid allergy data." });

            var newId = await _mediator.Send(new AddAllergyCommand(dto));

            if (newId <= 0)
                return StatusCode(500, new { message = "Failed to add allergy." });

            return Ok(new { message = "Allergy added successfully.", id = newId });
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientAllergies(int patientId)
        {
            if (patientId <= 0)
                return BadRequest(new { message = "Invalid patient id." });

            var allergies = await _mediator.Send(new GetPatientAllergiesQuery(patientId));
            return Ok(allergies);
        }

        [HttpPost]
        public async Task<IActionResult> EditAllergy([FromBody] EditAllergyDto dto)
        {
            if (dto == null || dto.Id <= 0)
                return BadRequest(new { message = "Invalid allergy data." });

            try
            {
                // create the command with the DTO
                var command = new EditAllergyCommand(dto);

                // send to MediatR handler
                var result = await _mediator.Send(command);

                // return success
                return Ok(new { message = "Allergy updated successfully." });
            }
            catch (Exception ex)
            {
                // log exception if needed
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAllergy([FromBody] int id)
        {
            if (id <= 0) return BadRequest(new { message = "Invalid id." });

            try
            {
                var result = await _mediator.Send(new DeleteAllergyCommand(id));

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
