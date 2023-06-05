using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;
using Domain.Common.Models;
using Application.Context;
using Domain.Entities;

namespace Application.Authentication.Login
{
    public partial class LoginCommand : UserAuth, IRequest<AuthResult>
    {
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly IApplicationDbContext _context;

        private readonly IAuthenticateService _authenticationService;
        private readonly ILogger<LoginCommand> _logger;

        public LoginCommandHandler(IApplicationDbContext context, IAuthenticateService authenticationService, ILogger<LoginCommand> logger)
        {
            _context = context;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"About to login. Email:{request.Email}");
                return await _authenticationService.Login(request.Email, request.Password);
            }
            catch (Exception ex)
            {
                return new AuthResult { IsSuccess = false, Message = " Error logging in : " + ex?.Message + ex?.InnerException?.Message };
            }
        }
    }
}
