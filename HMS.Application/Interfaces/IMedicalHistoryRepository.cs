using HMS.Application.DTO.MedicalHistory;
using HMS.Domain.Entities;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IMedicalHistoryRepository
    {
        Task<int> AddAsync(CreateMedicalHistoryDto dto);
        Task<IEnumerable<MedicalHistory>> GetPatientMedicalHistoryAsync(int patientId);
        Task<int> UpdateMedicalHistoryAsync(EditMedicalHistoryDto medicalHistory);
        Task<int> DeleteMedicalHistoryAsync(int id);

    }
}
