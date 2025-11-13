using HMS.Application.Commands.PatientPortal;
using HMS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers
{
    public class PatientPortalController : Controller
    {
        private readonly IMediator _mediator;

        public PatientPortalController(IMediator mediator) {
            _mediator = mediator;
        }

        [AllowAnonymous]
        public IActionResult PatientPortal()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult SearchPatientByIdentifier(string identifier) {
            try
            {
                if (identifier != null)
                {
                    var command = new SearchPatientByIdentifierQuery(identifier);
                    var result = _mediator.Send(command);
                    if(result != null) {
                        return Json(new { success = true, data = result.Result });
                    }
                    else {
                        return Json(new { success = false, message = "No patient found" });
                    }   
                }
                else {
                    return Json(new { success = false, message = "No patient found" });
                }
          
            }
            catch (Exception ex) {
                return StatusCode(500, new
                {
                    success = false,
                    Error = ex.Message
                });
            }
        }
    }
}
