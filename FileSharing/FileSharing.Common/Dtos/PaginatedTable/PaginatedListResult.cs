using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Common.Dtos.PaginatedTable
{
    public class PaginatedListResult<T>
    {
        /// <summary>
        /// Used for CosmosDb
        /// </summary>
        public string ContinuationToken { get; set; }
        public List<T> Items { get; set; }
        public int CountAll { get; set; }
    }
}
