using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Slot
{
    public class AddSlotDto
    {
        [Required(ErrorMessage = "Reporting time is required.")]
        public TimeSpan ReportingTime { get; set; }

        [Required(ErrorMessage = "Leaving time is required.")]
        public TimeSpan LeavingTime { get; set; }

        [Required(ErrorMessage = "At least one day must be selected.")]
        [Range(1, 127, ErrorMessage = "Invalid days of the week value.")]
        public int DaysOfWeek { get; set; }

        // Optional: if you plan to link slot to doctor later
        // [Required(ErrorMessage = "Doctor is required.")]
        // public int DoctorId { get; set; }
    }
}
