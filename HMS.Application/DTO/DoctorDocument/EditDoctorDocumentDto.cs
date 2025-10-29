using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.DoctorDocument
{
    public class EditDoctorDocumentDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }

        // New file info (if updated)
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }

        public string UploadedBy { get; set; } = string.Empty;
    }
}
