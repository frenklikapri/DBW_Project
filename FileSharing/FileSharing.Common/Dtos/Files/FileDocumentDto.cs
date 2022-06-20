using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Common.Dtos.Files
{
    public class FileDocumentDto
    {
        public Guid Id { get; set; }
        public string Filename { get; set; }
        public int Size { get; set; }
        public bool IsBlocked { get; set; }
        public string FileUrl { get; set; }
        public string UserId { get; set; }
    }
}
