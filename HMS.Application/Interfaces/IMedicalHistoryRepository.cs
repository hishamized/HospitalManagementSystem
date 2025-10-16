using HMS.Application.DTO.MedicalHistory;
using HMS.Domain.Entities;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IMedicalHistoryRepository
    {
        Task<int> AddAsync(CreateMedicalHistoryDto dto);
    }
}
