using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class LabRequestItem
    {
        public int Id { get; set; }
        public int? LabRequestId { get; set; }
        public int? LabTestId { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public LabRequest? LabRequest { get; set; } = null!;
        public LabTest? LabTest { get; set; } = null!;
        public Sample? Sample { get; set; } = null!;
        public LabResult? LabResult { get; set; } = null!;
    }
}
