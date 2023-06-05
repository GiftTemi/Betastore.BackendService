using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Domain.Common.Models;
using Application.Context;
using Application.Interfaces;

namespace Application.Users.Queries
{
    public class GetUserByIdQuery : IRequest<Result>
    {
        public string Id { get; set; }
    }

    public class GetCustomerByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetUserByIdQuery> _logger;
        private readonly IMapper _mapper;

        public GetCustomerByIdQueryHandler(IApplicationDbContext context, ILogger<GetUserByIdQuery> logger, IIdentityService identityService, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Backend Service - About to get user details by id - request parameters: " + request);
                var result = await _identityService.GetUserById(request.Id.Trim());
                if (result.user == null)
                {
                    _logger.LogError($"Backend Service at {DateTime.Now} - User does not exist with Id: {request.Id}");
                    return Result.Failure($"User does not exist with Id: {request.Id}");
                }
                var user = _mapper.Map<UserDto>(result.user);
                if (user.RoleId.HasValue && user.RoleId > 0 && user.Role == null)
                    user.Role = await _context.Roles.Include(p => p.RolePermissions).Where(a => a.Id == user.RoleId).FirstOrDefaultAsync();

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
