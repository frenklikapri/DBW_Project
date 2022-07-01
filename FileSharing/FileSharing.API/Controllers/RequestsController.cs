using FileSharing.Common.Dtos.PaginatedTable;
using FileSharing.Common.Dtos.Requests;
using FleSharing.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileSharing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestsRepository _requestsRepository;

        public RequestsController(IRequestsRepository requestsRepository)
        {
            _requestsRepository = requestsRepository;
        }

        [HttpPost]
        public async Task<ActionResult> AddRequest(AddBlockRequestDto addBlockRequestDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _requestsRepository.AddRequestAsync(addBlockRequestDto);

            if (success)
            {
                var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
                return Created(resourcePath, null);
            }

            return BadRequest("Couldn't create the request");
        }

        [HttpGet("getAllRequests")]
        public async Task<ActionResult<PaginatedListResult<BlockRequestDto>>> GetAllRequests(int page, int pageSize, string? search = "")
        {
            var paginationParams = new PaginationParameters
            {
                Page = page,
                PageSize = pageSize,
                Search = search
            };

            var results = await _requestsRepository.GetBlockRequestsAsync(paginationParams);

            return Ok(results);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("aproveRequest/{id}")]
        public async Task<ActionResult> AproveRequest(string id)
        {
            var guid = Guid.Parse(id);

            var success = await _requestsRepository.AproveRequestAsync(guid);

            if (success)
                return Ok();

            return BadRequest("Couldn't aprove the request, please contact the administrator");
        }
    }
}
