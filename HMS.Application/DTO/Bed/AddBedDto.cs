using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Bed
{
    public class AddBedDto
    {
        public string BedCode { get; set; }
        public string BedNumber { get; set; }
        public int WardId { get; set; }
        public string BedType { get; set; }
        public bool IsOccupied { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
