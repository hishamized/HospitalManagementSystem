using HMS.Application.Dto;
using HMS.Application.DTO.Doctor;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IDoctorRepository
    {
        Task<int> AddDoctorAsync(AddDoctorDto doctor);
        // Other CRUD methods: Update, Delete, GetAll, GetById
    }
}
