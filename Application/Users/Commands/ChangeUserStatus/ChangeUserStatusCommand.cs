using Application.Context;
using Application.Interfaces;
using Domain.Common.Models;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands
{
    public class ChangeUserStatusCommand : AuthToken, IRequest<Result>
    {
        public string UserId { get; set; }
        public string LoggedInUserId { get; set; }
        public Status Status { get; set; }

    }

    public class ChangeUserStatusCommandHandler : IRequestHandler<ChangeUserStatusCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ILogger<ChangeUserStatusCommand> _logger;

        public ChangeUserStatusCommandHandler(IApplicationDbContext context, ILogger<ChangeUserStatusCommand> logger, IIdentityService identityService)
        {
            _context = context;
            _logger = logger;
            _identityService = identityService;
        }

        public async Task<Result> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Backend  Service - About to change user details - request parameters: " + request);
                var adminUser = await _identityService.GetUserById(request.LoggedInUserId);
                if (adminUser.user == null)
                {
                    _logger.LogError("Backend  Service - Invalid User to initiate status change", request.LoggedInUserId);
                    return Result.Failure($"User Status change was not successful. Invalid User to initiate status change");
                }

                var userStatusForChange = await _identityService.GetUserById(request.UserId);
                if (userStatusForChange.user == null)

                {
                    _logger.LogError("Backend  Service - Invalid User for status change");
                    return Result.Failure($"User Status change was not successful . Invalid User for status change");
                }

                var result = await _identityService.ChangeUserStatusAsync(userStatusForChange.user);
                await _context.SaveChangesAsync(cancellationToken);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Backend  Service - status changed successfully", result.Message != null ? result.Message : result.Messages[0]);
                    return Result.Success(result.Message != null ? result.Message : result.Messages[0]);
                }

                else
                {
                    _logger.LogError("Backend  Service - error occured while changing status", result.Message != null ? result.Message : result.Messages[0]);
                    return Result.Failure(result.Message != null ? result.Message : result.Messages[0]);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend  Service at {DateTime.Now} - User status change was not successful, {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"User status change was not successful, {ex?.Message + ex?.InnerException?.Message}");
            }
        }
    }
}
