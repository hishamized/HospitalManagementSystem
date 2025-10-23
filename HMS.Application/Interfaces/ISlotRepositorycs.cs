using HMS.Application.DTO.Slot;
using HMS.Application.DTOs.Slot;
using HMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
   public interface ISlotRepository
    {
        Task<int> AddAsync(AddSlotDto dto);
        Task<List<SlotDto>> GetAllAsync();
        Task<int> EditAsync(EditSlotDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
