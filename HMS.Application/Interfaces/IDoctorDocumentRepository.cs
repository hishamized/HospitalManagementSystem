using HMS.Application.DTO.DoctorDocument;
using HMS.Application.ViewModel.DoctorDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IDoctorDocumentRepository
    {
        Task<int> AddDoctorDocumentAsync(DoctorDocumentDto documentDto);
        Task<IEnumerable<DoctorDocumentWithDoctorViewModel>> GetAllDoctorDocumentsWithDoctorsAsync();
        Task<bool> UpdateDoctorDocumentAsync(EditDoctorDocumentDto dto);

        Task<bool> DeleteDoctorDocumentAsync(int documentId);

    }
}
