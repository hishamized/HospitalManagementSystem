using HMS.Application.Commands;
using HMS.Application.DTO.MedicalHistory;
using HMS.Application.DTO.Patient;
using HMS.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HMS.Application.Commands.MedicalHistory;

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

    }
}
