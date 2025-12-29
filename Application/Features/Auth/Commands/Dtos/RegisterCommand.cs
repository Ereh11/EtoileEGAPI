using Application.Common;
using MediatR;

namespace Application.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<APIResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
