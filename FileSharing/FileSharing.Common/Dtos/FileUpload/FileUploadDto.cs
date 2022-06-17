using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Common.Dtos.FileUpload
{
    public class FileUploadDto
    {
        public string Filename { get; set; }
        public int Size { get; set; }
        public byte[] Bytes { get; set; }
        public string UserId { get; set; }
    }
}
