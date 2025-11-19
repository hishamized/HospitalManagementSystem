using HMS.Application.Commands.LabTest;
using HMS.Application.DTO.LabTest;
using HMS.Application.Queries.LabTest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace HMS.Web.Controllers
{
    [Authorize]
    public class LabController : Controller
    {
        private readonly IMediator _mediator;
        public LabController(IMediator mediator) {
            _mediator = mediator;
        }

        public IActionResult ManageTestsCore() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddLabTest([FromBody] AddLabTestDto dto) {
            try
            {
                var command = new AddLabTestCommand(dto);

                var _result = await _mediator.Send(command);

                return Ok(new
                {
                    success = true,
                    result = _result
                });
            }
            catch (ValidationException fv)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = fv.Errors.Select(e => e.ErrorMessage)
                });
            }
            catch (Exception ex) {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                });
            }     

        }
        [HttpGet]
        public async Task<IActionResult> FetchLabTests()
        {
            try
            {
                var query = new FetchLabTestsQuery();

                var _result = await _mediator.Send(query);

                return Ok(new
                {
                    success = true,
                    result = _result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLabTest(int TestId) {
            try {
                var query = new DeleteLabTestQuery(TestId);
                var result = await _mediator.Send(query);
                if (result == false)
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        error = "Either No record found for such a test or unknown error occured"
                    });
                }
                return Ok(new
                {
                    success = true,
                    data = result
                });
            } catch (Exception ex)
            {
                return StatusCode(500, new {
                    success = false,
                    error = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditLabTest([FromBody] EditLabTestDto dto)
        {
            try
            {
                var command = new EditLabTestCommand(dto);
                var result = await _mediator.Send(command);
                if (result == false) {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Could not perform updation operation due to an error on the server"
                    });
                }
                return Ok(new { 
                    success = true,
                    message = "The lab test has been updated successfully"
                });
            }
            catch (Exception ex) {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
