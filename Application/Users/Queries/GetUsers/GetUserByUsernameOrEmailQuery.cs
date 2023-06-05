using AutoMapper;
using Application.Context;
using Application.Interfaces;
using Domain.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries
{
    public class GetUserByUsernameOrEmailQuery : IRequest<Result>
    {
        public string Email { get; set; }
        public string UserName { get; set; }
    }

    public class GetUserByUsernameOrEmailueryHandler : IRequestHandler<GetUserByUsernameOrEmailQuery, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetUserByUsernameOrEmailQuery> _logger;
        private readonly IMapper _mapper;

        public GetUserByUsernameOrEmailueryHandler(IApplicationDbContext context, ILogger<GetUserByUsernameOrEmailQuery> logger, IIdentityService identityService, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result> Handle(GetUserByUsernameOrEmailQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Backend Service - About to get user details by id - request parameters: " + request);
                var result = await _identityService.GetUserByUsernameOrEmail(request.Email.Trim(), request.UserName);

                var user = _mapper.Map<UserDto>(result.user);
                if (user == null)
                {
                    _logger.LogError($"Backend Service at {DateTime.Now} - {result.Result.Message}");
                    return Result.Failure($"{result.Result.Message}");
                }
                if (user.RoleId != 0)
                    user.Role = await _context.Roles.Where(a => a.Id == user.RoleId).FirstOrDefaultAsync();
                _logger.LogInformation("Backend Service - user details", user);
                return Result.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving user by Id: " + ex?.Message ?? ex?.InnerException?.Message);
                return Result.Failure($"Error retrieving user by Id: " + ex?.Message ?? ex?.InnerException?.Message);
            }
        }
    }
}

