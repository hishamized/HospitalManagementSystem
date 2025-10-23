using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Slot
{
    public class EditSlotDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Reporting time is required.")]
        public TimeSpan ReportingTime { get; set; }

        [Required(ErrorMessage = "Leaving time is required.")]
        public TimeSpan LeavingTime { get; set; }

        [Required(ErrorMessage = "At least one day must be selected.")]
        [Range(1, 127, ErrorMessage = "Invalid days of the week value.")]
        public int DaysOfWeek { get; set; }

        // Optional: link to doctor later
        // public int DoctorId { get; set; }
    }
}
