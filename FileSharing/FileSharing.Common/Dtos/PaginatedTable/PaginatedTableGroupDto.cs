using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Common.Dtos.PaginatedTable
{
    public class PaginatedTableGroupDto<T>
    {
        public string Name { get; set; }
        public List<PaginatedTableRowDto<T>> Items { get; set; }
        public bool Show { get; set; } = true;
    }
}
