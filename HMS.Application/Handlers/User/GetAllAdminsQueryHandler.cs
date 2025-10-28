using AutoMapper;
using HMS.Application.Interfaces;
using HMS.Application.ViewModel.User;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Features.Users.Queries.GetAllAdmins
{
    public class GetAllAdminsQueryHandler : IRequestHandler<GetAllAdminsQuery, List<AdminListVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetAllAdminsQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<AdminListVm>> Handle(GetAllAdminsQuery request, CancellationToken cancellationToken)
        {
            // Fetch data from repository (SP will handle join logic)
            var admins = await _userRepository.GetAllAdminsAsync();

            // Map to ViewModel
            var adminListVm = _mapper.Map<List<AdminListVm>>(admins);

            return adminListVm;
        }
    }
}
