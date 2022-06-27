using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Common.Dtos.PaginatedTable
{
    public class PaginationParameters
    {
        public string Search { get; set; } = string.Empty;
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
