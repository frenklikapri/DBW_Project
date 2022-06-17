using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Core.Entities
{
    public class DownloadLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime DownloadedAt { get; set; }

        public Guid FileDocumentId { get; set; }
        public FileDocument FileDocument { get; set; }
    }
}
