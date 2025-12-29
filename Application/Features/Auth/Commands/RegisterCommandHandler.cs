using Application.Common;
using Application.Features.Auth.Commands.Dtos;
using Domain.Entities;
using Domain.Interfaces.JwtTokenGenerator;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Application.Features.Auth.Commands
{
    internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, APIResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger _logger;
        private readonly IValidator<RegisterCommand> _validator;
        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            IJwtTokenGenerator jwtTokenGenerator,
            ILogger logger,
            IValidator<RegisterCommand> validator
            )
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
            _validator = validator;
        }

        public async Task<APIResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if(!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return APIResponse.Failure(errors, "Validation failed.");
            }
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if(existingUser != null)
            {
                return APIResponse.Failure(["User with given email already exists."], "Registration failed.");
            }
            var newUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedOnUtc = DateTime.UtcNow,
            };

            var createUserResult = await _userManager.CreateAsync(newUser, request.Password);
            if(!createUserResult.Succeeded)
            {
                var errors = createUserResult.Errors.Select(e => e.Description).ToList();
                return APIResponse.Failure(errors, "User creation failed.");
            }

            await _userManager.AddToRoleAsync(newUser, Roles.User);
            var roles = await _userManager.GetRolesAsync(newUser);

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(newUser, roles);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();


            newUser.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(5));
            newUser.UpdateLastLogin();

            await _userManager.UpdateAsync(newUser);

            _logger.Information($"User registered: {newUser.Email}");
            var authResponse = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = _jwtTokenGenerator.GetAccessTokenExpiry()
            };

            return APIResponse<AuthResponseDto>.Success(authResponse, "User registered successfully.");
        }
    }       
}
