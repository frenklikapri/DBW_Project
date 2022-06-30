using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Common.Dtos.Files
{
    public class FileToDownloadDto
    {
        public bool DownloadLimitPassed { get; set; }
        public DateTime LastDownloadedAt { get; set; }
        public string FileName { get; set; }
        public byte[] Bytes { get; set; }
    }
}
