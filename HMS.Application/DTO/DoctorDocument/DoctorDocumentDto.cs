using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.DoctorDocument
{
    public class DoctorDocumentDto
    {
        public int Id { get; set; }                // For update/view purposes
        public int DoctorId { get; set; }          // FK to Doctors table
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public string UploadedBy { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
