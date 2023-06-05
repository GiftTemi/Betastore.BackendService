using MediatR;
using Application.Interfaces;
using Domain.Common.Models;
using Application.Context;

namespace Application.Authentication.Logout
{
    public partial class LogoutCommand : IRequest<Result>
    {
        public string Email { get; set; }
    }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        private readonly IAuthenticateService _authenticationService;

        public LogoutCommandHandler(IApplicationDbContext context, IAuthenticateService authenticationService)
        {
            _context = context;
            _authenticationService = authenticationService;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _authenticationService.LogOut(request.Email);
            }
            catch (Exception ex)
            {

                return Result.Failure($" Error logging out : " + ex?.Message + ex?.InnerException?.Message);
            }
        }
    }
}