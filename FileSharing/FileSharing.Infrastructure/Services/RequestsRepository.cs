using FileSharing.Common.Dtos.PaginatedTable;
using FileSharing.Common.Dtos.Requests;
using FileSharing.Infrastructure.Data;
using FleSharing.Core.Entities;
using FleSharing.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> AproveRequestAsync(Guid requestId)
        {
            var request = await _dbContext
                .BlockRequests
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request is null)
                return false;

            var document = await _dbContext
                .FileDocuments
                .FirstOrDefaultAsync(d => d.Id == request.FileDocumentId);

            if (document is null)
                return false;

            document.IsBlocked = request.RequestType == Common.Enums.BlockRequestType.Block;

            _dbContext.BlockRequests.Remove(request);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<PaginatedListResult<BlockRequestDto>> GetBlockRequestsAsync(PaginationParameters paginationParameters)
        {
            if (string.IsNullOrEmpty(paginationParameters.Search))
                paginationParameters.Search = string.Empty;

            var query = _dbContext
                .BlockRequests
                .AsQueryable();

            var countAll = await query.CountAsync();
            var list = await query
                .Skip((paginationParameters.Page - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .Include(b => b.User)
                .Include(b => b.FileDocument)
                .Select(b => new BlockRequestDto
                {
                    FileDocumentId = b.FileDocumentId,
                    Id = b.Id,
                    Reason = b.Reason,
                    RequestType = b.RequestType,
                    UserId = b.UserId,
                    FileName = b.FileDocument.Filename,
                    UserName = b.User.UserName
                })
                .ToListAsync();

            return new PaginatedListResult<BlockRequestDto>
            {
                CountAll = countAll,
                Items = list
            };
        }
    }
}
