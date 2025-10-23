using System;

namespace HMS.Application.DTOs.Slot
{
    public class SlotDto
    {
        public int Id { get; set; }

        public TimeSpan ReportingTime { get; set; }

        public TimeSpan LeavingTime { get; set; }

        public int DaysOfWeek { get; set; }

    }
}
