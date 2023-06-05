using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Application.Interfaces;
using Domain.Enums;
using Domain.Common.Models;
using Application.Context;

namespace Application.Authentication.ResetPassword
{
    public class ResetPasswordCommand : IRequest<Result>
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }


    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuthenticateService _authenticationService;
        private readonly IEmailService _emailService;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;
        private readonly IStringHashingService _stringHashingService;

        public ResetPasswordCommandHandler(IApplicationDbContext context,
            IAuthenticateService authenticationService,
             IEmailService emailService, IIdentityService identityService, IConfiguration configuration, IStringHashingService stringHashingService)
        {
            _context = context;
            _authenticationService = authenticationService;
            _emailService = emailService;
            _identityService = identityService;
            _configuration = configuration;
            _stringHashingService = stringHashingService;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //check if password reset was initiated
                if (request.Email is null)
                {
                    return Result.Failure("Invalid Email");
                }
                var userDetail = await _identityService.GetUserByEmail(request.Email);
                if (userDetail.user == null)
                {
                    return Result.Failure("User cannot be found");
                }

                var passwordResetAttempt = await _context.PasswordResetAttempts.OrderByDescending(a => a.CreatedDate).FirstOrDefaultAsync(a => a.Email == request.Email && a.PasswordResetStatus == PasswordResetStatus.Initiated);
                if (passwordResetAttempt == null)
                {
                    return Result.Failure("Invalid Password Reset Attempt");
                }
                if (_stringHashingService.DecodeDESStringHash(request.Token) == passwordResetAttempt.Token)
                {
                    return Result.Failure("Invalid Password Reset Token");
                }

                TimeSpan timeDifference = DateTime.Now - passwordResetAttempt.CreatedDate;
                if (timeDifference.TotalHours > 2)
                {
                    return Result.Failure($"Reset Password link has expired.Kindly reinitiate another password reset");
                }
                var result = await _authenticationService.ResetPassword(request.Email, request.NewPassword);
                if (result.Succeeded)
                {

                    _context.PasswordResetAttempts.Update(passwordResetAttempt);

                    string webDomain = _configuration["WebDomain:AfreximUsers"];

                    if (userDetail.user.AccessLevel == AccessLevel.Customer && !userDetail.user.EmailConfirmed)
                    {
                        userDetail.user.Status = Status.Active;
                        userDetail.user.EmailConfirmed = true;
                        await _identityService.UpdateUserAsync(userDetail.user);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    var email = new EmailVm
                    {
                        Application = "Afrexim",
                        Subject = "Password Reset Successful",
                        RecipientEmail = request.Email,
                        FirstName = userDetail.user.FirstName,
                        LastName = userDetail.user.LastName,
                        Body = "Your password has been reset.You can now login to continue your activities.",
                        ButtonText = "Login",
                        ButtonLink = webDomain.Replace("/admin-login", "") + $"/reset-password?email={request.Email}&token={userDetail.user.Token}",
                        //ButtonLink = webDomain + $"login?email={request.Email}&token={userDetail.user.Token}'",
                        ImageSource = _configuration["SVG:EmailVerification"],
                    };

                    await _emailService.ResetPasswordEmailAsync(email);

                    return Result.Success("Password Reset Successful");
                }
                return Result.Failure("Reset Password failed");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error changing password!: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}