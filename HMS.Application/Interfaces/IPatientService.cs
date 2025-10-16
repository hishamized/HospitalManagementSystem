using HMS.Application.DTO.Patient;
using HMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Domain.Interfaces
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<int> AddAsync(CreatePatientDto patient);
        Task<int> UpdateAsync(UpdatePatientDto patient);
        Task<int> DeleteAsync(int id);
        Task<string> GeneratePatientCodeAsync();

    }
}
