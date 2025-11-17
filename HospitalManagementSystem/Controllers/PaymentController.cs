using AutoMapper;
using HMS.Application.Commands.Payment;
using HMS.Application.DTO.Payment;
using HMS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public PaymentController(IMediator mediator, IMapper mapper) { 
            _mediator = mediator;
            _mapper = mapper;
        }
        [HttpPost]
        [Authorize(Roles = "Patient")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment([FromBody] AddPaymentDto Dto) {
            var patientIdClaimValue = User.FindFirst("PatientId")?.Value;
            if (!long.TryParse(patientIdClaimValue, out var _PatiendId))
            {
                return BadRequest(new { success = false, message = "Invalid patient ID in claim." });
            }
            if (Dto == null)
                return BadRequest("Invalid bill data.");
            try
            {
                Dto.PatientId = Convert.ToInt32(_PatiendId);
                Dto.PaymentDate = DateTime.UtcNow;
                var command = new AddPaymentCommand(Dto);
                var _result = await _mediator.Send(command);

                return Ok(new
                {
                    success = true,
                    result = _result
                });
            } catch (Exception ex) {
                return StatusCode(500, new{ 
                 success = false,
                 message = ex.Message
                });
            }
        }
    }
}
