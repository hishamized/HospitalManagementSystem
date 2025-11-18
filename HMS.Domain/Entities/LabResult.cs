using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class LabResult
    {
        public int Id { get; set; }
        public int? LabRquestItemId { get; set; }
        public string ResultValue { get; set; } = string.Empty;
        public string? ResultNotes { get; set; }
        public DateTime ResultDate { get; set; } = DateTime.UtcNow;
        public int? DoctorId {get; set;}
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public LabRequestItem? LabRequestItem { get; set; } = null!;
        public Doctor? Doctor { get; set; } = null!;
    }
}
