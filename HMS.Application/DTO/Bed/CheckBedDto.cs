using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Bed
{
    public class CheckBedDto
    {
        public int? PatientBedWardId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public bool HasBedAllotted { get; set; }

        public string WardName { get; set; }
        public string WardCode { get; set; }
        public int WardCapacity { get; set; }
        public int OccupiedBeds { get; set; }

        public string BedCode { get; set; }
        public string BedNumber { get; set; }
        public string BedType { get; set; }
        public int BedPosition { get; set; }  // Example: 3rd bed out of N
        public bool IsBedOccupied { get; set; }

        public string Status { get; set; }
        public string Description { get; set; }
    }
}
