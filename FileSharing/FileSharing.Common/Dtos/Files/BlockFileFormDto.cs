using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Common.Dtos.Files
{
    public class BlockFileFormDto
    {
        [Required(ErrorMessage = "Please enter a reason for the request")]
        public string Reason { get; set; }
    }
}
