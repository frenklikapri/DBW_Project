using FileSharing.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Common.Dtos.Requests
{
    public class BlockRequestDto
    {
        public Guid Id { get; set; }
        public Guid FileDocumentId { get; set; }
        public string FileName { get; set; }
        public BlockRequestType RequestType { get; set; }
        public string Reason { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
