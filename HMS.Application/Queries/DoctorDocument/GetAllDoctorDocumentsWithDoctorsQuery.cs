using HMS.Application.ViewModel;
using HMS.Application.ViewModel.DoctorDocument;
using MediatR;
using System.Collections.Generic;

namespace HMS.Application.Queries.DoctorDocument
{
    // This Query will return a list of DoctorDocumentWithDoctorInfoVm
    public class GetAllDoctorDocumentsWithDoctorsQuery : IRequest<IEnumerable<DoctorDocumentWithDoctorViewModel>>
    {
    }
}
