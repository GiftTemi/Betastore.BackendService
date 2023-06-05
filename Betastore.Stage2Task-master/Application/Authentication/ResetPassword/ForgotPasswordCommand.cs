
using MediatR;
using Microsoft.Extensions.Configuration;
using Application.Interfaces;
using Domain.Common.Models;
using Application.Context;
using Domain.Entities;
using Domain.Enums;

namespace Application.Authentication.ResetPassword
{
    public class ForgotPasswordCommand : IRequest<Result>
    {
        public string Email { get; set; }
    }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IEmailService _emailservice;
        private readonly IAuthenticateService _authenticationService;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;
        private readonly IApplicationDbContext _context;
        private readonly IStringHashingService _stringHashingService;
        public ForgotPasswordCommandHandler(IEmailService emailService,
            IAuthenticateService authenticationService,
            IConfiguration configuration,
            IIdentityService identityService,
            IApplicationDbContext context, IStringHashingService stringHashingService)
        {
            _emailservice = emailService;
            _authenticationService = authenticationService;
            _identityService = identityService;
            _configuration = configuration;
            _context = context;
            _stringHashingService = stringHashingService;
        }
        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Email is null)
                {
                    return Result.Failure("Invalid Email");
                }
                var userDetail = await _identityService.GetUserByEmail(request.Email);
                if (userDetail.user == null)
                {

                    return Result.Failure("User cannot be found");
                }
                var passwordAttempts = new PasswordResetAttempt
                {
                    Email = request.Email,
                    PasswordResetStatus = PasswordResetStatus.Initiated,
                    CreatedBy = request.Email,
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = request.Email,
                    LastModifiedDate = DateTime.Now,
                    Status = Status.Active,
                };
                await _context.PasswordResetAttempts.AddAsync(passwordAttempts);
                var hashValue = (userDetail.user.Email + DateTime.Now).ToString();
                var hashedValue = _stringHashingService.CreateDESStringHash(hashValue);
                //customerDetail.Customer.Token = emailhash;
                userDetail.user.Token = hashedValue;

                await _identityService.UpdateUserAsync(userDetail.user);

                await _context.SaveChangesAsync(cancellationToken);
                string webDomain = _configuration["WebDomain:EmailVerificationUrl"];
                if (userDetail.user.AccessLevel != null)
                {
                    webDomain = _configuration["WebDomain:EmailVerificationUrl"];
                }
                var email = new EmailVm
                {
                    Application = "Afrexim",
                    Subject = "Forgot Password",
                    BCC = "",
                    RecipientName = userDetail.user.Name,
                    CC = userDetail.user.Email,
                    RecipientEmail = userDetail.user.Email,
                    FirstName = userDetail.user.FirstName,
                    LastName = userDetail.user.LastName,
                    Body = $"We recently received a request to reset your vendor account password for the username {userDetail.user.UserName}",
                    Body1 = $"If you did not request a new password, rest assured that your password is safe. \n Password changes requested through our website are only sent to the verified vendor email on the account. If you are worried that someone is trying to gain unauthorized access to your account, go ahead and change your password or contact us {_configuration["DefaultUserDetails:SystemAdminEmail"]} for assistance",
                    ButtonText = "Reset Password",
                    ButtonLink = webDomain + $"/vendor/reset-password?email={userDetail.user.Email}&token={userDetail.user.Token}'",
                    ImageSource = _configuration["SVG:EmailVerification"],
                };

                await _emailservice.SendForgotPasswordEmailAsync(email);

                return Result.Success("Reset Password Initiated successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Reset Password Initiation failed: {ex?.Message ?? ex?.InnerException?.Message}");
            }

        }
    }
}
