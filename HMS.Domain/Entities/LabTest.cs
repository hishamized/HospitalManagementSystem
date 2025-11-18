using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class LabTest
    {
        public int Id { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string? Description {get; set;}
        public string? SampleType { get; set; }
        public string NormalRange { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
