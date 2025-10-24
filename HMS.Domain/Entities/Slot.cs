using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class Slot
    {
        public int Id { get; set; }

        // Doctor availability times
        public TimeSpan ReportingTime { get; set; }
        public TimeSpan LeavingTime { get; set; }

        // Days of the week as a bitmask (optional) or string/enum
        // Using bitmask allows you to store multiple days in a single integer
        public int DaysOfWeek { get; set; } // Sunday = 1, Monday = 2, ..., Saturday = 64

        public ICollection<Doctor>? Doctors { get; set; }
    }
}
