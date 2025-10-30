using HMS.Application.DTO.Ward;
using HMS.Application.ViewModels.Ward;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IWardRepository
    {
        Task<int> AddWardAsync(CreateWardDto dto);
        Task<IEnumerable<WardDto>> GetAllWardsAsync();
        Task<int> UpdateWardAsync(UpdateWardDto ward);
        Task<int> DeleteWardAsync(int wardId);
        Task<(bool IsSuccess, string Message)> AssignDoctorToWardAsync(AssignDoctorWardDto dto);
        Task<IEnumerable<DoctorWardAssignmentViewModel>> GetAllDoctorWardAssignmentsAsync();
        Task<int> DeleteDoctorWardAssignmentAsync(int id);


    }
}
