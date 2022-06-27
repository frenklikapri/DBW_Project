using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Common.Store.PaginatedTable
{
    public class PaginatedTableReducer : Reducer<PaginatedTableState, PaginatedTableAction>
    {
        public override PaginatedTableState Reduce(PaginatedTableState state, PaginatedTableAction action)
        {
            return new PaginatedTableState();
        }
    }
}
