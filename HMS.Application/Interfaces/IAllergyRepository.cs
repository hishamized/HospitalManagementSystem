using HMS.Application.DTO.Allergy;
using HMS.Domain.Entities;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IAllergyRepository
    {
        Task<int> AddAsync(AddAllergyDto allergy);
        Task<IEnumerable<Allergy>> GetPatientAllergiesAsync(int patientId);
        Task<bool> UpdateAllergyAsync(EditAllergyDto allergy);

        Task<int> DeleteAllergyAsync(int id);

  
    }
}
