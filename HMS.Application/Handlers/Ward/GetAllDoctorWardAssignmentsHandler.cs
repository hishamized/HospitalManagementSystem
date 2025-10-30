using AutoMapper;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Ward;
using HMS.Application.ViewModels.Ward;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Ward
{
    public class GetAllDoctorWardAssignmentsHandler
      : IRequestHandler<GetAllDoctorWardAssignmentsQuery, IEnumerable<DoctorWardAssignmentViewModel>>
    {
        private readonly IWardRepository _wardRepository;
        private readonly IMapper _mapper;

        public GetAllDoctorWardAssignmentsHandler(IWardRepository wardRepository, IMapper mapper)
        {
            _wardRepository = wardRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DoctorWardAssignmentViewModel>> Handle(
            GetAllDoctorWardAssignmentsQuery request,
            CancellationToken cancellationToken)
        {
            // Fetch from DB (stored procedure)
            var assignmentDtos = await _wardRepository.GetAllDoctorWardAssignmentsAsync();

            // Map to ViewModel
            var mappedResult = _mapper.Map<IEnumerable<DoctorWardAssignmentViewModel>>(assignmentDtos);

            return mappedResult;
        }
    }
}
