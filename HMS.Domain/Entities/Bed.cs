using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class Bed
    {
        public int Id { get; set; }
        public string BedCode { get; set; }
        public string BedNumber { get; set; }
        public int WardId { get; set; }
        public string BedType { get; set; }
        public bool IsOccupied { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Ward Ward { get; set; }
    }
}

