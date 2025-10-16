using HMS.Application.DTO.Patient;
using HMS.Application.Queries;
using HMS.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers
{
    public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, IEnumerable<PatientDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllPatientsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PatientDto>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
        {
            var patients = await _unitOfWork.Patients.GetAllAsync();
            var dtos = new List<PatientDto>();

            foreach (var p in patients)
            {
                dtos.Add(new PatientDto
                {
                    Id = p.Id,
                    PatientCode = p.PatientCode,
                    FullName = p.FullName,
                    Gender = p.Gender,
                    DateOfBirth = p.DateOfBirth,
                    Email = p.Email,
                    ContactNumber = p.ContactNumber,
                    AlternateContactNumber = p.AlternateContactNumber,
                    Address = p.Address,
                    City = p.City,
                    State = p.State,
                    ZipCode = p.ZipCode,
                    BloodGroup = p.BloodGroup,
                    EmergencyContactName = p.EmergencyContactName,
                    EmergencyContactNumber = p.EmergencyContactNumber,
                    RelationshipWithEmergencyContact = p.RelationshipWithEmergencyContact,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    IsActive = p.IsActive
                });
            }

            return dtos;
        }
    }
}
