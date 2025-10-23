using HMS.Application.Dto;
using HMS.Application.Dto.Doctor;
using HMS.Application.DTO.Doctor;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IDoctorRepository
    {
        Task<int> AddDoctorAsync(AddDoctorDto doctor);
        Task<IEnumerable<GetDoctorsDto>> GetAllDoctorsAsync();
        Task<bool> UpdateDoctorAsync(EditDoctorDto dto);
        Task<bool> DeleteDoctorAsync(int doctorId);
    }
}
