using HMS.Application.DTO.Feedback;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Feedback
{
    // Command for adding new feedback
    public class CreateFeedbackCommand : IRequest<int>
    {
        public CreateFeedbackDto Dto { get; }

        public CreateFeedbackCommand(CreateFeedbackDto dto)
        {
            Dto = dto;
        }
    }
}
