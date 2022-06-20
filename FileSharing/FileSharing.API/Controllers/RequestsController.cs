using FileSharing.Common.Dtos.Requests;
using FleSharing.Core.Interfaces;
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
    }
}
