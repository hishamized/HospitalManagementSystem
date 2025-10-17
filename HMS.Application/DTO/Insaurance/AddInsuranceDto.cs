using System;
using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTO.Insurance
{
    public class AddInsuranceDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProviderName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string PolicyNumber { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
