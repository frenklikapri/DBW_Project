using FileSharing.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Common.Dtos.Requests
{
    public class AddBlockRequestDto
    {
        [Required(ErrorMessage = "Please enter a reason for the request")]
        public string Reason { get; set; }

        public string UserId { get; set; }

        [Required]
        public string FileDocumentId { get; set; }

        public BlockRequestType BlockRequestType { get; set; }
    }
}
