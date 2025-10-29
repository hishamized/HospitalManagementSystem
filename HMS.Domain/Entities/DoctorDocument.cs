using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities
{
    public class DoctorDocument
    {
        public int Id { get; set; }

        // Foreign key column
        public int DoctorId { get; set; }

        // Navigation property — one document belongs to one doctor
        [ForeignKey(nameof(DoctorId))]
        public Doctor Doctor { get; set; } = null!;

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty; // e.g., PDF, JPG, DOCX

        public long FileSize { get; set; } // in bytes

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public string UploadedBy { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
