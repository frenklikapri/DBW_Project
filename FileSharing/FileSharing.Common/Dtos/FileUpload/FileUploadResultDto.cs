using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Common.Dtos.FileUpload
{
    public class FileUploadResultDto
    {
        public bool Success { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
    }
}
