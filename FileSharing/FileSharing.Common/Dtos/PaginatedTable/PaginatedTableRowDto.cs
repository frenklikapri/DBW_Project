using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Common.Dtos.PaginatedTable
{
    public class PaginatedTableRowDto<T>
    {
        public T Item { get; set; }
        public bool Show { get; set; } = true;
        public bool Selected { get; set; }
    }
}
