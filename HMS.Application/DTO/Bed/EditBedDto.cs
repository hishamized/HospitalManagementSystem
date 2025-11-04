using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Bed
{
    public class EditBedDto
    {
        public int Id { get; set; }
        public string BedCode { get; set; } = string.Empty;
        public string BedNumber { get; set; } = string.Empty;
        public string BedType { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
