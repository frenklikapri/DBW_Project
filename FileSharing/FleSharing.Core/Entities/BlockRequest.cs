using FileSharing.Common.Enums;
using FileSharing.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleSharing.Core.Entities
{
    public class BlockRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid FileDocumentId { get; set; }
        public FileDocument FileDocument { get; set; }

        public BlockRequestType RequestType { get; set; }
        public string Reason { get; set; }

        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
    }
}
