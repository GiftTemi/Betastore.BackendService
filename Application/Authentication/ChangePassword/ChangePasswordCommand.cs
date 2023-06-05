using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Application.Interfaces;
using Domain.Common.Models;
using Application.Context;
using Domain.Enums;

namespace Application.Authentication.ChangePassword
{
    public partial class ChangePasswordCommand : IRequest<Result>
    {
        public string Username { get; set; }
        public string UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ChangePasswordCommand> _logger;
        private readonly IAuthenticateService _authenticationService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IIdentityService _identityService;

        public ChangePasswordCommandHandler(IApplicationDbContext context, ILogger<ChangePasswordCommand> logger,
            IAuthenticateService authenticationService, IEmailService emailService,
            IConfiguration configuration, IIdentityService identityService)
        {
            _context = context;
            _logger = logger;
            _authenticationService = authenticationService;
            _emailService = emailService;
            _configuration = configuration;
            _identityService = identityService;
        }

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) && string.IsNullOrEmpty(request.UserId))
                {
                    return Result.Failure($"Error!, Userid and UserName cannot be empty");
                }

                if (request.OldPassword == request.NewPassword)
                {
                    return Result.Failure($"Error!, Old Password is same as New Password");
                }
                var existingUser = await _identityService.GetUserByUsername(request.Username);
                if (existingUser.user == null)
                {
                    if (!string.IsNullOrEmpty(request.UserId))
                    {
                        existingUser = await _identityService.GetUserById(request.UserId);
                    }
                }
                if (existingUser.user == null)
                {
                    return Result.Failure("Change Password Failed. User not found");
                }

                var isValidOldPassword = await _identityService.CheckPasswordAsync(existingUser.user, request.OldPassword);

                if (!isValidOldPassword)
                {
                    return Result.Failure("Change Password Failed. Incorrect Old Password");
                }

                await _authenticationService.ChangePassword(!string.IsNullOrEmpty(request.Username) ? request.Username : existingUser.user.Email, request.OldPassword, request.NewPassword);
                string webDomain = _configuration["WebDomain:AfreximUsers"];

                if (existingUser.user.AccessLevel != null)
                {
                    if (existingUser.user.AccessLevel == AccessLevel.Customer)
                    {
                        webDomain = _configuration["WebDomain:Vendors"];
                        if (existingUser.user.AccessLevel == AccessLevel.Customer && existingUser.user.Status != Status.Active)
                        {
                            await _identityService.ChangeUserStatusAsync(existingUser.user);
                        }
                    }
                }


                var email = new EmailVm
                {
                    Application = "Afrexim",
                    Subject = "Change Password",
                    CC = existingUser.user.Email,
                    Text = "",
                    RecipientEmail = request.Username,
                    RecipientName = existingUser.user.FirstName,
                    NewPassword = request.NewPassword,
                    OldPassword = request.OldPassword,
                    Body = "Your password has been updated successfully!",
                    Body1 = "Please click the button below to login",
                    ButtonText = "Login",
                    ButtonLink = webDomain
                };
                await _emailService.ChangePasswordEmail(email);
                return Result.Success("User password has been successfully changed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now} -Error changing password, {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Error changing password : Contact Admin for support");
            }
        }
    }
}
