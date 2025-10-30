using AutoMapper;
using HMS.Application.Commands.Feedback;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Feedback
{

    public class DeleteFeedbackCommandHandler : IRequestHandler<DeleteFeedbackCommand, int>
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMapper _mapper;

        public DeleteFeedbackCommandHandler(IFeedbackRepository feedbackRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(DeleteFeedbackCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id <= 0)
                    throw new ArgumentException("Invalid feedback ID.");

                // Repository call (we'll define this next)
                var rowsAffected = await _feedbackRepository.DeleteFeedbackAsync(request.Id);

                if (rowsAffected == 0)
                    throw new Exception("No feedback found with the given ID.");

                return rowsAffected;
            }
            catch (Exception ex)
            {
                // You may log this later
                throw new ApplicationException($"Error deleting feedback: {ex.Message}", ex);
            }
        }
    }
}
