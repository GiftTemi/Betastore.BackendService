using MediatR;
using Application.Context;
using Application.Interfaces;
using Domain.Common.Models;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands
{
    public class VerifyUserEmailCommand : IRequest<Result>
    {
        public string Email { get; set; }
    }

    public class VerifyUserEmailCommandHandler : IRequestHandler<VerifyUserEmailCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ILogger<VerifyUserEmailCommand> _logger;

        public VerifyUserEmailCommandHandler(IApplicationDbContext context, ILogger<VerifyUserEmailCommand> logger, IIdentityService identityService)
        {
            _context = context;
            _logger = logger;
            _identityService = identityService;
        }
        public async Task<Result> Handle(VerifyUserEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Backend Service - About to get user details by id - request parameters: " + request);
                var getUserForVerification = await _identityService.GetUserByEmail(request.Email);
                if (getUserForVerification.user == null)
                {
                    _logger.LogError($"Backend Service at {DateTime.Now} - User does not exist with email: {request.Email}");
                    return Result.Failure($"User does not exist with email: {request.Email}");
                }
                getUserForVerification.user.EmailConfirmed = true;
                await _identityService.UpdateUserAsync(getUserForVerification.user);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("User email verification was successful", getUserForVerification.user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating user email: " + ex?.Message ?? ex?.InnerException?.Message);
                return Result.Failure($"Error validating user email: " + ex?.Message ?? ex?.InnerException?.Message);
            }
        }
    }
}
