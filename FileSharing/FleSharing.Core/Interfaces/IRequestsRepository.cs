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
    }
}
