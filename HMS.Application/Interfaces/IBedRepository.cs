using HMS.Application.DTO.Bed;
using HMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IBedRepository
    {
        Task<int> AddBedAsync(AddBedDto dto);
        Task<int> CheckBedUniquenessAsync(string bedCode, string bedNumber);
        Task<(IEnumerable<BedListDto> Beds, int TotalCount)> GetPagedBedsAsync(int pageNumber, int pageSize);

        Task<int> UpdateBedAsync(EditBedDto dto);
        Task<int> DeleteBedAsync(int id);

        Task<IEnumerable<BedDropdownDto>> GetBedsByWardIdAsync(int wardId);
        Task<bool> AllotBedAsync(AllotBedDto entity);
        Task<CheckBedDto> CheckBedStatusAsync(int patientId);
        Task<int> RemovePatientFromBedAsync(int patientBedWardId);
    }
}
