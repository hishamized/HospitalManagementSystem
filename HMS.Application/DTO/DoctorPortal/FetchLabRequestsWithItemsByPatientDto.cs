using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.DoctorPortal
{
    public class FetchLabRequestsWithItemsByPatientDto
    {
        public int LabRequestId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime RequestDate { get; set; }
        public int LabRequestItemId { get; set; }
        public int LabTestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SampleType { get; set; } = string.Empty;
        public string NormalRange { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string LabRequestStatus { get; set; } = string.Empty;
        public string LabRequestItemStatus { get; set; } = string.Empty;

    }
}
