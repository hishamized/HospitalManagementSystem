using HMS.Application.Dto;
using HMS.Application.DTO;
using HMS.Application.DTO.Patient;
using HMS.Application.DTOs.PatientVisitDtos;
using HMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IPatientVisitRepository
    {
        Task<int> AddPatientVisitAsync(AddPatientVisitDto dto);
        Task<IEnumerable<PatientVisitDto>> GetAllPatientVisitsAsync();
        Task<bool> DeleteAsync(int id);
        Task<int> UpdateAsync(PatientVisitUpdateDto dto);
    }
}
