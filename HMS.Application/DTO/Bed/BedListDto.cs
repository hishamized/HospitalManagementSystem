using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Bed
{
    public class BedListDto
    {
        public int Id { get; set; }
        public string BedCode { get; set; }
        public string BedNumber { get; set; }
        public string BedType { get; set; }
        public bool IsOccupied { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string WardName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
