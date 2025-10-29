using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Ward
{
    public class CreateWardDto
    {
        public string WardCode { get; set; }
        public string WardName { get; set; }
        public string WardType { get; set; }
        public int Capacity { get; set; }
        public int OccupiedBeds { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
