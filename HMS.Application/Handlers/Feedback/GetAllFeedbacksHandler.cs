using AutoMapper;
using HMS.Application.DTO.Feedback;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Feedback;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Feedback
{
    public class GetAllFeedbacksHandler : IRequestHandler<GetAllFeedbacksQuery, IEnumerable<FeedbackListDto>>
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMapper _mapper;

        public GetAllFeedbacksHandler(IFeedbackRepository feedbackRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FeedbackListDto>> Handle(GetAllFeedbacksQuery request, CancellationToken cancellationToken)
        {
            // Fetch data from repository (which internally uses the stored procedure)
            var feedbacks = await _feedbackRepository.GetAllFeedbacksAsync();

            // Map to DTOs
            var feedbackDtos = _mapper.Map<IEnumerable<FeedbackListDto>>(feedbacks);

            return feedbackDtos;
        }
    }
}
