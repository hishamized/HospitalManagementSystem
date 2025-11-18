using HMS.Application.Commands.LabTest;
using HMS.Application.DTO.LabTest;
using HMS.Application.Queries.LabTest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
