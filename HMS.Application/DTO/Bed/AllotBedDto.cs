using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Bed
{
    public class AllotBedDto
    {
        public int PatientId { get; set; }
        public int WardId { get; set; }
        public int BedId { get; set; }
        public string? Notes { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
