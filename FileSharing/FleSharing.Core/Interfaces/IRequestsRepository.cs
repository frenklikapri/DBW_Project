using FileSharing.Common.Dtos.PaginatedTable;
using FileSharing.Common.Dtos.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleSharing.Core.Interfaces
{
    public interface IRequestsRepository
    {
        Task<bool> AddRequestAsync(AddBlockRequestDto blockRequestDto);
        Task<PaginatedListResult<BlockRequestDto>> GetBlockRequestsAsync(PaginationParameters paginationParameters);
        Task<bool> AproveRequestAsync(Guid requestId);
    }
}
