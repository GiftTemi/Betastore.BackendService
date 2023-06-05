using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Context;
using Application.Interfaces;
using Domain.Common.Models;
using Domain.Entities;
using Application.Users.Queries;

namespace Application.Users.Queries
{
    public class GetUsersQuery : IRequest<Result>
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string SearchValue { get; set; }
        public List<string> Filter { get; set; }
        public string UserId { get; set; }
    }

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetUsersQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IApplicationDbContext context, ILogger<GetUsersQueryHandler> logger, IIdentityService identityService, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Backend Service - About to get user details - request parameters: " + JsonSerializer.Serialize(request));

                var result = await _identityService.GetAll(0, 0);
                var users = result.users ?? Enumerable.Empty<User>();

                var filteredUsers = users
                    .Where(u => string.IsNullOrEmpty(request.SearchValue) ||
                                (u.FirstName != null && u.FirstName.ToLower().Contains(request.SearchValue)) ||
                                (u.LastName != null && u.LastName.ToLower().Contains(request.SearchValue)) ||
                                (u.Email != null && u.Email.ToLower().Contains(request.SearchValue)) ||
                                (u.Role != null && u.Role.Name.ToLower().Contains(request.SearchValue)) ||
                                (u.StatusDesc.ToLower().Contains(request.SearchValue)))
                    .Distinct()
                    .ToList();

                foreach (var filterBy in request.Filter)
                {
                    if (filterBy.ToLower().Contains("name"))
                    {
                        filteredUsers.AddRange(users
                            .Where(u => (u.FirstName != null && u.FirstName.ToLower().Contains(request.SearchValue)) ||
                                        (u.LastName != null && u.LastName.ToLower().Contains(request.SearchValue))));
                    }
                    if (filterBy.ToLower().Contains("email"))
                    {
                        filteredUsers.AddRange(users
                            .Where(u => u.Email != null && u.Email.ToLower().Contains(request.SearchValue)));
                    }
                    if (filterBy.ToLower().Contains("role"))
                    {
                        filteredUsers.AddRange(users
                            .Where(u => u.Role != null && u.Role.Name.ToLower().Contains(request.SearchValue)));
                    }
                    if (filterBy.ToLower().Contains("status"))
                    {
                        filteredUsers.AddRange(users
                            .Where(u => u.StatusDesc.ToLower().Contains(request.SearchValue)));
                    }
                }

                var userList = filteredUsers.Distinct().ToList();

                if (userList.Count <= 0)
                {
                    return Result.Failure("No users exist with the specified details");
                }

                if (request.Skip != 0 || request.Take != 0)
                {
                    userList = userList.Skip(request.Skip).Take(request.Take).ToList();
                }

                var roles = await _context.Roles.ToListAsync();
                var usersResult = _mapper.Map<List<UserDto>>(userList);

                _logger.LogError("Backend Service - Done retrieving user details - response parameters: " + JsonSerializer.Serialize(usersResult));

                return Result.Success(new { Entities = usersResult, Count = result.users.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now} - Error retrieving user: {ex?.Message ?? ex?.InnerException?.Message}", ex.StackTrace);
                return Result.Failure($"Error retrieving user: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
