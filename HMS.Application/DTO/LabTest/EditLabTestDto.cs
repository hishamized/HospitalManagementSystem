using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.LabTest
{
    public class EditLabTestDto
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string? Description { get; set; }
        public string? SampleType { get; set; }
        public string NormalRange { get; set; }
        public decimal? Price { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
