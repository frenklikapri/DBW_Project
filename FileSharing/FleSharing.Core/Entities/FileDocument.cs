using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Core.Entities
{
    public class FileDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Filename { get; set; }
        public int Size { get; set; }
        public byte[] Bytes { get; set; }
        public bool IsBlocked { get; set; }
        public string FileUrl { get; set; }
        public string UserId { get; set; }
        public DateTime UploadedAt { get; set; }

        public IEnumerable<DownloadLog> DownloadLogs { get; set; }
    }
}
