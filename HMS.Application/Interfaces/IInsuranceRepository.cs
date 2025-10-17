using HMS.Application.DTO.Insurance;
using HMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IInsuranceRepository
    {
        Task<int> AddAsync(AddInsuranceDto dto);
        Task<IEnumerable<Insurance>> GetByPatientIdAsync(int patientId);

    }
}
