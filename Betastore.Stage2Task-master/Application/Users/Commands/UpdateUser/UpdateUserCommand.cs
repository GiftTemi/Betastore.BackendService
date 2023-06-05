using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Application.Users.Queries;
using Domain.Common.Models;
using Application.Context;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Users.Commands
{
    public partial class UpdateUserCommand : IRequest<Result>
    {

        public string UserId { get; set; }
        public string ModifiedByUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DepartmentId { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public AccessLevel? AccessLevel { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly IBase64ToFileConverter _base64ToFileConverter;
        private readonly ILogger<UpdateUserCommand> _logger;


        public UpdateUserCommandHandler(IApplicationDbContext context, IIdentityService identityService, IMapper mapper,
                                        IBase64ToFileConverter base64ToFileConverter, ILogger<UpdateUserCommand> logger)
        {
            _context = context;
            _identityService = identityService;
            _mapper = mapper;
            _base64ToFileConverter = base64ToFileConverter;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var getUserForUpdate = await _identityService.GetUserById(request.UserId);
                if (getUserForUpdate.user == null)
                {
                    return Result.Failure("Invalid User details for update");
                }
                User adminUser = null;
                if (!string.IsNullOrEmpty(request.ModifiedByUserId))
                {
                    var adminDetail = await _identityService.GetUserById(request.ModifiedByUserId);
                    if (adminDetail.user != null)
                    {
                        adminUser = adminDetail.user;
                    }
                    else
                    {
                        return Result.Failure("Backend Service - Invalid Admin User detail ! Please contact support");
                    }
                }
                if (request.RoleId != getUserForUpdate.user?.RoleId)
                {

                    var assignedRole = await _context.Roles.FirstOrDefaultAsync(c => c.Id == request.RoleId);
                    if (assignedRole == null)
                    {
                        _logger.LogError($"Backend Service at {DateTime.Now} - Unable to assign roles and department. Invalid role to be assigned", request.RoleId);
                        return Result.Failure("Unable to assign roles and department. Invalid role to be assigned");
                    }

                    var roleForUpdate = await _context.Roles.FindAsync(request.RoleId);
                    if (roleForUpdate == null)
                    {
                        return Result.Failure("Backend Service - Invalid role selected");
                    }
                }

                if (getUserForUpdate.user?.Role == null)
                {
                    if (getUserForUpdate.user?.RoleId != null && getUserForUpdate.user.RoleId > 0)
                    {
                        getUserForUpdate.user.Role = await _context.Roles.FindAsync(getUserForUpdate.user.RoleId);
                        getUserForUpdate.user.Status = Status.Active;
                    }
                }

                getUserForUpdate.user.Email = request.Email.Trim();
                getUserForUpdate.user.UserName = request.Email.Trim();
                getUserForUpdate.user.LastModifiedDate = DateTime.Now;
                getUserForUpdate.user.LastModifiedBy = adminUser.UserName;
                getUserForUpdate.user.FirstName = request.FirstName.Trim();
                getUserForUpdate.user.LastName = request.LastName;
                getUserForUpdate.user.RoleId = request.RoleId > 0 ? request.RoleId : null;
                getUserForUpdate.user.Status = Status.Active;

                if (request.AccessLevel != null)
                {
                    getUserForUpdate.user.AccessLevel = (AccessLevel)request.AccessLevel;
                }

                await _identityService.UpdateUserAsync(getUserForUpdate.user);

                await _context.SaveChangesAsync(cancellationToken);
                var userResult = _mapper.Map<UserDto>(getUserForUpdate.user);
                return Result.Success("User details updated successfully", userResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now} - Error updating user  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Error updating user {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
