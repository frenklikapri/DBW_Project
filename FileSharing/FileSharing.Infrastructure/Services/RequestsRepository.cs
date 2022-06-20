using FileSharing.Common.Dtos.Requests;
using FileSharing.Infrastructure.Data;
using FleSharing.Core.Entities;
using FleSharing.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Infrastructure.Services
{
    public class RequestsRepository : IRequestsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RequestsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddRequestAsync(AddBlockRequestDto blockRequestDto)
        {
            var toAdd = new BlockRequest
            {
                FileDocumentId = Guid.Parse(blockRequestDto.FileDocumentId),
                Reason = blockRequestDto.Reason,
                RequestType = blockRequestDto.BlockRequestType,
                UserId = string.IsNullOrWhiteSpace(blockRequestDto.UserId) ? null : blockRequestDto.UserId
            };

            _dbContext.BlockRequests.Add(toAdd);

            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
