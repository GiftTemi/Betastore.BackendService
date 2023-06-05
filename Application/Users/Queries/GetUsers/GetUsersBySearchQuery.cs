using Application.Context;
using Application.Interfaces;
using Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries
{
    public class GetUsersBySearchQuery : IRequest<Result>
    {
        public string SearchValue { get; set; }
        public List<string> Filter { get; set; }
        public string UserId { get; set; }
    }

    public class GetUsersBySearchQueryHandler : IRequestHandler<GetUsersBySearchQuery, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetUsersBySearchQuery> _logger;
        private readonly IIdentityService _identityService;
        public GetUsersBySearchQueryHandler(IApplicationDbContext context, ILogger<GetUsersBySearchQuery> logger, IIdentityService identityService)
        {
            _context = context;
            _logger = logger;
            _identityService = identityService;
        }
        public async Task<Result> Handle(GetUsersBySearchQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Backend Service - About to get user details - request parameters: " + request);
                var result = await _identityService.GetAll(0, 0);
                if (result.users == null)
                {
                    _logger.LogError("Backend Service -Invalid Users");
                    return Result.Failure($"Invalid Users");
                }
                var filteredUsers = new List<Domain.Entities.User>();
                var usernameMatch = request.Filter.FirstOrDefault(c => c.ToLower().Contains("name"));
                if (!string.IsNullOrEmpty(usernameMatch))
                {
                    var filterUsersByUsername = result.users.Where(c => c.UserName != null && c.UserName.ToLower().Contains(request.SearchValue)
                    || c.FirstName != null && c.FirstName.ToLower().Contains(request.SearchValue)
                    || c.LastName != null && c.LastName.ToLower().Contains(request.SearchValue)).ToList();
                    filteredUsers.AddRange(filterUsersByUsername);
                }

                var emailMatch = request.Filter.FirstOrDefault(c => c.ToLower().Contains("email"));
                if (!string.IsNullOrEmpty(emailMatch))
                {
                    var filterUsersByEmail = result.users.Where(c => c.Email != null && c.Email.ToLower().Contains(request.SearchValue)).ToList();
                    filteredUsers.AddRange(filterUsersByEmail);
                }

                var rolesMatch = request.Filter.FirstOrDefault(c => c.ToLower().Contains("role"));
                if (!string.IsNullOrEmpty(rolesMatch))
                {
                    var filterUsersByRoles = result.users.Where(c => c.Role != null && c.Role.Name.ToLower().Contains(request.SearchValue)).ToList();
                    filteredUsers.AddRange(filterUsersByRoles);
                }

                // Search on department

                //var filterResult = filteredUsers.Select(c => c.Email).Distinct();
                var filterResult = filteredUsers.GroupBy(x => x.Email)
                .Select(g => g.First())
                .ToList();
                return Result.Success("Filtering Users by search value was successful", filterResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now} - Retrieving users was not successful, {ex?.Message ?? ex?.InnerException?.Message}", ex.StackTrace);
                return Result.Failure($"Retrieving users was not successful, {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
