using HMS.Application.DTO.Ward;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Ward
{
    public class CreateWardCommand : IRequest<int>
    {
        public CreateWardDto Ward { get; set; }

        public CreateWardCommand(CreateWardDto ward)
        {
            Ward = ward;
        }
    }
}
