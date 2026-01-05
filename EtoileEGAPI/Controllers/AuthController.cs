using Application.Common;
using Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EtoileEGAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<Results<Ok<APIResponse>, BadRequest<APIResponse>>> Register([FromBody] RegisterCommand command)
        {
            if (!ModelState.IsValid)
            {
                return TypedResults.BadRequest(
                    APIResponse.Failure(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    "Invalid model state.")
                    );
            }
            var response = await _mediator.Send(command);
            if (response.IsSuccess)
            {
                return TypedResults.Ok(response);
            }
            else
            {
                return TypedResults.BadRequest(response);
            }
        }
    }
}
