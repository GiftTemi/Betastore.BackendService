using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Domain.Common.Models;
using Domain.Enums;
using Domain.Entities;
using Application.Context;
using Application.Interfaces;
using System.Data.Common;
using Infrastructure;

namespace Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IApplicationDbContext _context;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(UserManager<User> userManager,
            IConfiguration configuration, IApplicationDbContext context, ILogger<IdentityService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public async Task<(Result Result, string UserId)> CreateUserAsync(User user)
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                _logger.LogInformation($"Backend Service - About to create User");
               
                var result = await _userManager.CreateAsync(user, user.Password);
                if (!result.Succeeded)
                {
                    return (Result.Failure(result.Errors.FirstOrDefault().Description), "");
                }
                await _context.SaveChangesAsync(new CancellationToken());

                if (user.RoleId == 0)//If customer
                {
                    // Send notification to customer
                }
                else
                {

                }

                return (result.ToApplicationResult(), user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now} - Invalid Admin User detail ! Please contact support");
                return (Result.Failure($"Error retrieving username, {ex?.Message ?? ex?.InnerException?.Message}"), "");
            }
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            try
            {

                var appUser = await _userManager.FindByEmailAsync(user.Email);
                var isLoginValid = await _userManager.CheckPasswordAsync(appUser, password);
                if (!isLoginValid)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now} - Error verifying oldpassword - response parameters, {ex?.Message ?? ex?.InnerException?.Message}");
                return true;
            }
        }

        public async Task<Result> ChangeUserStatusAsync(User user)
        {
            try
            {
                var appUser = await _userManager.Users.FirstOrDefaultAsync(a => a.Id == user.UserId);
                string message = "";
                if (appUser != null)
                {
                    switch (appUser.Status)
                    {
                        case Status.Active:
                            appUser.Status = Status.Active;
                            message = "User activation was successful";
                            break;
                        case Status.Inactive:
                            appUser.Status = Status.Inactive;
                            message = "User deactivation was successful";
                            break;
                        case Status.Deactivated:
                            appUser.Status = Status.Inactive;
                            message = "User deactivation was successful";
                            break;
                        case Status.Deleted:
                            appUser.Status = Status.Active;
                            message = "User has been deleted successfully";
                            break;
                        default:
                            break;
                    }
                };
                return await ChangeUserStatusAsync(appUser, message);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error changing user status, {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
        public async Task<Result> ChangeUserStatusAsync(User user, string message)
        {
            try
            {
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                    return Result.Success(message);
                else
                    return Result.Failure(result.ToApplicationResult().Messages);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Result> UpdateUserAsync(User user)
        {
            try
            {
                var userToUpdate = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == user.UserId);
                if (userToUpdate == null)
                {
                    return Result.Failure($"User for update does not exist");
                }
                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.Email = user.UserName;
                userToUpdate.CreatedBy = user.CreatedBy;
                userToUpdate.CreatedDate = user.CreatedDate;
                userToUpdate.LastModifiedBy = user.Email;
                userToUpdate.LastModifiedDate = DateTime.Now;
                userToUpdate.RoleId = user.RoleId;
                userToUpdate.UserName = user.UserName;
                userToUpdate.Status = user.Status;
                userToUpdate.AccessLevel = user.AccessLevel;
                userToUpdate.EmailConfirmed = user.EmailConfirmed;
                userToUpdate.Address = user.Address;
                userToUpdate.DateOfBirth = user.DateOfBirth;
                userToUpdate.LastLoginDate = user.LastLoginDate;
                userToUpdate.Role = user.Role;
                userToUpdate.Password = user.Password;
                userToUpdate.Token = user.Token;
                userToUpdate.Otp = user.Otp;
                userToUpdate.LastModifiedBy = user.LastModifiedBy;
                userToUpdate.LastModifiedDate = user.LastModifiedDate;
                userToUpdate.MiddleName = user.MiddleName;


                await _userManager.UpdateAsync(userToUpdate);
                await _context.SaveChangesAsync(new CancellationToken());
                return Result.Success("User was updated successfully", user);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error updating user: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        public async Task<(Result result, List<User> users)> GetUsersByDepartment(string role)
        {

            List<User> users = new List<User>();
            try
            {
                users = await _userManager.Users.Include(x => x.Role).Where(x => x.Role != null && x.Role.Name.Trim().ToLower() == role.ToLower()).ToListAsync();

                return (Result.Success("Total Count: ", users.Count), users);
            }
            catch (Exception ex)
            {
                return (Result.Failure($"Error retrieving users", ex?.Message ?? ex?.InnerException?.Message), users);
            }
        }

        public async Task<(Result result, List<User> users)> GetAll(int skip, int take)
        {

            List<User> users = new List<User>();
            try
            {
                if (skip == 0 && take == 0)
                {
                    users = await _userManager.Users.Include(a => a.Role).ToListAsync();
                }
                else
                {
                    users = await _userManager.Users.Include(a => a.Role).Skip(skip).Take(take).ToListAsync();
                }
                return (Result.Success("Total Count: ", users.Count), users);
            }
            catch (Exception ex)
            {
                return (Result.Failure($"Error retrieving users", ex?.Message ?? ex?.InnerException?.Message), users);
            }
        }
        public async Task<(Result result, List<User> users)> GetAllCustomers(int skip, int take)
        {

            List<User> users = new List<User>();
            try
            {
                if (skip == 0 && take == 0)
                {
                    users = await _userManager.Users.Include(a => a.Role).ToListAsync();
                }
                else
                {
                    users = await _userManager.Users.Include(a => a.Role).Skip(skip).Take(take).ToListAsync();
                }
                return (Result.Success("Total Count: ", users.Count), users);
            }
            catch (Exception ex)
            {
                return (Result.Failure($"Error retrieving users", ex?.Message ?? ex?.InnerException?.Message), users);
            }
        }
        public async Task<(Result result, List<User> users)> GetAll(int skip, int take, string searchValue, List<string> filter)
        {
            try
            {
                IQueryable<User> query = _userManager.Users.Where(a => a.AccessLevel == AccessLevel.Customer).Include(a => a.Role);

                if (!string.IsNullOrEmpty(searchValue))
                {
                    searchValue = searchValue.ToLower();

                    query = query.Where(u =>
                        u.FirstName.ToLower().Contains(searchValue) ||
                        u.LastName.ToLower().Contains(searchValue) ||
                        u.Email.ToLower().Contains(searchValue) ||
                        (u.Role != null && u.Role.Name.ToLower().Contains(searchValue)) ||
                        u.StatusDesc.ToLower().Contains(searchValue)
                    );
                }

                if (filter != null && filter.Count > 0)
                {
                    var filterOptions = filter.Select(f => f.ToLower()).ToList();

                    query = query.Where(u =>
                        (filterOptions.Contains("name") && (u.FirstName.ToLower().Contains(searchValue) || u.LastName.ToLower().Contains(searchValue))) ||
                        (filterOptions.Contains("email") && u.Email.ToLower().Contains(searchValue)) ||
                        (filterOptions.Contains("role") && u.Role != null && u.Role.Name.ToLower().Contains(searchValue)) ||
                        (filterOptions.Contains("status") && u.StatusDesc.ToLower().Contains(searchValue))
                    );
                }

                if (skip > 0)
                {
                    query = query.Skip(skip);
                }

                if (take > 0)
                {
                    query = query.Take(take);
                }

                List<User> users = await query.ToListAsync();

                return (Result.Success("Total Count: ", users.Count), users);
            }
            catch (Exception ex)
            {
                return (Result.Failure($"Error retrieving users", ex?.Message ?? ex?.InnerException?.Message), default);
            }
        }

        public async Task<bool> RoleInUse(int roleId)
        {
            var inUse = false;
            List<User> users = new List<User>();
            try
            {
                inUse = await _userManager.Users.AnyAsync(x => x.RoleId == roleId);
                return inUse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now} - Error checking if Role has been assigned to any user - response parameters, {ex?.Message ?? ex?.InnerException?.Message}");

                return inUse;
            }
        }


        public async Task<(Result Result, User user)> GetUserById(string userId)
        {
            try
            {
                // var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return (Result.Failure($"User does not exist with Id: {userId}"), null);
                }
                if (user.Role == null)
                {
                    if (user?.RoleId != null && user?.RoleId > 0)
                    {
                        user.Role = await _context.Roles.FindAsync(user.RoleId);
                    }
                }
                return (Result.Success(), user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}: Error retrieving user: {ex?.Message ?? ex?.InnerException?.Message}");
                return (Result.Failure($"Error retrieving users : Contact Admin for support"), null);
            }
        }

        public async Task<(Result Result, User user)> GetUserByUsername(string userName)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                if (user == null)
                {
                    return (Result.Failure($"User does not exist with username: {userName}"), null);
                }

                return (Result.Success(), user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}: Error retrieving Username: {ex?.Message ?? ex?.InnerException?.Message}");
                return (Result.Failure($"Error retrieving user by Username: {ex?.Message ?? ex?.InnerException?.Message}"), null);
            }
        }

        public async Task<(Result Result, string userName)> GetUserNameAsync(string userId)
        {
            try
            {
                var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

                return (Result.Success(), user.UserName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}: Error retrieving Username: {ex?.Message ?? ex?.InnerException?.Message}");
                return (Result.Failure($"Error retrieving Username: {ex?.Message ?? ex?.InnerException?.Message}"), null);
            }
        }
        public async Task<(Result Result, User user)> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userManager.Users.Include(a => a.Role).FirstOrDefaultAsync(u => u.Email.ToLower().Trim() == email.ToLower().Trim());

                if (user == null)
                {
                    return (Result.Failure($"User does not exist with email: {email}"), null);
                }
                return (Result.Success(), user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}: Error retrieving User by email: {ex?.Message ?? ex?.InnerException?.Message}");
                return (Result.Failure($"Error retrieving User by email: {ex?.Message ?? ex?.InnerException?.Message}"), null);
            }
        }
        public async Task<(Result Result, User user)> GetUserByUsernameOrEmail(string email,string username)
        {
            try
            {
                User? user = await _userManager.Users.Include(a => a.Role).FirstOrDefaultAsync(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) ||
                               u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    string errorMessage = !string.IsNullOrEmpty(email) ? $"User does not exist with email: {email}" :
                                                                              $"User does not exist with username: {username}";
                    return (Result.Failure(errorMessage), null);
                }

                return (Result.Success(), user);
            }
            catch (DbException ex)
            {
                string errorMessage = $"Database error occurred while retrieving user: {ex.Message}";
                _logger.LogError($"Backend Service at {DateTime.Now}: {errorMessage}");
                return (Result.Failure(errorMessage), null);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error occurred while retrieving user: {ex.Message}";
                _logger.LogError($"Backend Service at {DateTime.Now}: {errorMessage}");
                return (Result.Failure(errorMessage), null);
            }
        }
        public async Task<(Result Result, User user)> GetDefaultAdmin()
        {
            try
            {
                var user = await _userManager.Users.Include(a => a.Role).FirstOrDefaultAsync(u => u.Role.Name.Contains("Admin"));
                if (user == null)
                {
                    return (Result.Failure($"No Admin user exists with email:"), null);
                }
                return (Result.Success(), user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}: Error retrieving User by username: {ex?.Message ?? ex?.InnerException?.Message}");
                return (Result.Failure($"Error retrieving User by username: {ex?.Message ?? ex?.InnerException?.Message}"), null);
            }
        }
    }
}