using HMS.Application.Dto;
using HMS.Application.Dto.Doctor;
using HMS.Application.DTO.Doctor;
using HMS.Application.DTO.DoctorPortal;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IDoctorRepository
    {
        Task<int> AddDoctorAsync(AddDoctorDto doctor);
        Task<IEnumerable<GetDoctorsDto>> GetAllDoctorsAsync();
        Task<bool> UpdateDoctorAsync(EditDoctorDto dto);
        Task<bool> DeleteDoctorAsync(int doctorId);
        Task<IEnumerable<DoctorSelectDto>> GetDoctorsByPatientWardAsync(int patientId);
        Task<int> AddDoctorRoundAsync(AddDoctorRoundDto dto);
        Task<IEnumerable<DoctorRoundHistoryDto>> GetRoundsByPatientAsync(int patientId);

        Task<UpdateDoctorRoundDto?> UpdateDoctorRoundAsync(UpdateDoctorRoundDto doctorRound);
        Task<bool> DeleteDoctorRoundAsync(int id);
    }
}
