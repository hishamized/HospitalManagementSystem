using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class Sample
    {
        public int Id { get; set; }
        public int? LabRequestItemId { get; set; }
        public DateTime CollectionDate { get; set; } = DateTime.Now;
        public int? CollectedByUserId { get; set; }
        public string Status { get; set; } = "Collected";
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public LabRequestItem? LabRequestItem { get; set; } = null!;
        public User? User { get; set; } = null!;
    }
}
