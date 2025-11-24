using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.LabTest
{
    public class UpdateLabRequestItemStatusDto
    {
        public int DoctorId { get; set; }
        public int LabRequestItemId { get; set; }
        public int LabRequestId { get; set; }
        public int LabTestId { get; set; }
        public int PatientId { get; set; }
        public string NewStatus { get; set; } = string.Empty;
    }
}
