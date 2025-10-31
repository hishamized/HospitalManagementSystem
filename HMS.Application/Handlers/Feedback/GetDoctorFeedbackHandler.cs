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
    public class GetDoctorFeedbackHandler : IRequestHandler<GetDoctorFeedbackQuery, IEnumerable<DoctorFeedbackDto>>
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMapper _mapper;

        public GetDoctorFeedbackHandler(IFeedbackRepository feedbackRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DoctorFeedbackDto>> Handle(GetDoctorFeedbackQuery request, CancellationToken cancellationToken)
        {
            // Fetch feedback data for the given doctor
            var feedbackList = await _feedbackRepository.GetFeedbackByDoctorIdAsync(request.DoctorId);

            // AutoMapper mapping (in case repository returns entities)
            // If repository already returns DTOs from Dapper, this step may be skipped.
            var feedbackDtos = _mapper.Map<IEnumerable<DoctorFeedbackDto>>(feedbackList);

            return feedbackDtos;
        }
    }
}
