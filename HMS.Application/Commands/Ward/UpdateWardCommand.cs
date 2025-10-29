using HMS.Application.DTO.Ward;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Ward
{
    public class UpdateWardCommand : IRequest<int>
    {
        public UpdateWardDto Ward { get; set; }

        public UpdateWardCommand(UpdateWardDto ward)
        {
            Ward = ward;
        }
    }
}
