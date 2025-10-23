using HMS.Application.Commands;
using HMS.Application.Commands.Insurance;
using HMS.Application.Commands.PatientVisit;
using HMS.Application.Commands.PatientVisits;
using HMS.Application.Dto;
using HMS.Application.DTO;
using HMS.Application.DTO.Allergy;
using HMS.Application.DTO.Insurance;
using HMS.Application.DTO.MedicalHistory;
using HMS.Application.DTO.Patient;
using HMS.Application.DTOs.PatientVisitDtos;
using HMS.Application.Features.PatientVisits.Commands;
using HMS.Application.Features.PatientVisits.Queries;
using HMS.Application.Queries;
using HMS.Application.Queries.Insurance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;


namespace HMS.Web.Controllers
{
    [Authorize]
    public class PatientVisitController : Controller

    {
        private readonly IMediator _mediator;
        public PatientVisitController(IMediator Mediator)
        {
            _mediator = Mediator;
        }

        [HttpGet]
        public IActionResult PatientVisits()
        {
            return View("~/Views/Patient/PatientVisits.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] AddPatientVisitDto dto)
        {
            try
            {
                var command = new AddPatientVisitCommand(dto);
                var newVisitId = await _mediator.Send(command);

                if (newVisitId > 0)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Patient visit added successfully.",
                        visitId = newVisitId
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "Failed to add patient visit."
                });
            }
            catch (FluentValidation.ValidationException ex)
            {
                // Return validation errors to the frontend
                var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed.",
                    errors
                });
            }
            catch (Exception ex)
            {
                // Log the error (Serilog, NLog, etc.)
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while adding the patient visit.",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var query = new GetAllPatientVisitsQuery();
                var visits = await _mediator.Send(query);
                return Ok(visits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving patient visits",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return Json(new { success = false, message = "Invalid ID" });

            var result = await _mediator.Send(new DeletePatientVisitCommand(id));

            if (result)
                return Json(new { success = true });
            else
                return Json(new { success = false, message = "Failed to delete patient visit" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePatientVisit([FromBody] PatientVisitUpdateDto visitDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new UpdatePatientVisitCommand(visitDto);
                bool updated = await _mediator.Send(command); // this returns true/false

                if (!updated)
                    return NotFound(new { Message = "No rows were updated. Visit may not exist." });

                return Ok(new { Success = true, Message = "Patient visit updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the patient visit.", Details = ex.Message });
            }
        }

    }
}
