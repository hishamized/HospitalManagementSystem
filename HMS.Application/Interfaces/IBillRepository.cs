using HMS.Application.DTO.Bill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
   public interface IBillRepository
    {
        Task<int> AddBillAsync(AddBillDto billDto);
        Task<IEnumerable<BillListDto>> GetBillsByPatientIdAsync(int patientId);
        Task<int> UpdateBillAsync(EditBillDto dto);
        Task<int> DeleteBillAsync(int id);

        Task<GetFinalBillDto> GetFinalBillAsync(int PatientId, int Visitid);
    }
}
