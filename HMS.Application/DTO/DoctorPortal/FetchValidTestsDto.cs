using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.DoctorPortal
{
    public class FetchValidTestsDto
    {
        public int LabTestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
