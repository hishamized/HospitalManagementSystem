using AutoMapper;
using HMS.Application.Commands.Feedback;
using MediatR;
using HMS.Application.Interfaces;

namespace HMS.Application.Handlers.Feedback
{
    public class CreateFeedbackHandler : IRequestHandler<CreateFeedbackCommand, int>
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMapper _mapper;

        public CreateFeedbackHandler(IFeedbackRepository feedbackRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
        {
            // Map DTO to entity (optional if repository directly accepts DTO)
            var dto = request.Dto;

            // Call repository method that executes stored procedure
            var newFeedbackId = await _feedbackRepository.AddFeedbackAsync(dto);

            // Return inserted record ID (or 0 if failed)
            return newFeedbackId;
        }
    }
}
