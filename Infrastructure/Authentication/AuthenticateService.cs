using AutoMapper;
using Domain.Entities;
using Application.Context;
using Application.Interfaces;
using Domain.Common.Models;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Authentication
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly IApplicationDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly IConfiguration _configuration;
        private readonly IStringHashingService _stringHashingService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthenticateService> _logger;


        public AuthenticateService(ITokenService tokenService, UserManager<User> userManager, IConfiguration configuration, IEmailService emailService,
         ILogger<AuthenticateService> logger, IApplicationDbContext context, IPasswordService passwordService, IStringHashingService stringHashingService,
            IMapper mapper)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _context = context;
            _passwordService = passwordService;
            _configuration = configuration;
            _stringHashingService = stringHashingService;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }


        public async Task<Result> ChangePassword(string userName, string oldPassword, string newPassword)
        {
            try
            {
                _logger.LogInformation($"About to change user password. Request paramenters. Username {userName}");
                var appUser = await _userManager.FindByNameAsync(userName);
                if (appUser == null)
                {
                    return Result.Failure("User does not exist with this password");
                }
                var isCorrect = await _userManager.CheckPasswordAsync(appUser, oldPassword);

                if (!isCorrect)
                {
                    _logger.LogInformation($"Change Password Not Successful. Wrong Old Password");
                    return Result.Success("Change Password Not Successful.  Wrong Old Password");
                }

                var result = await _userManager.ChangePasswordAsync(appUser, oldPassword, newPassword);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Change Password Successful");
                    return Result.Success("Change Password Successful");
                }
                else
                {
                    _logger.LogInformation($"Change Password not Successful");
                    return Result.Failure("Change Password not Successful");
                }
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error changing password - {ex?.Message ?? ex.InnerException?.Message}");
            }
        }

        public async Task<AuthResult> Login(string userName, string password)
        {
            var authResult = new AuthResult();
            try
            {
                #region DO NOT DELETE: IT IS NEEDED IN VENDOR SERVICE
                if (userName == _configuration["AfreximUtilityApi:Username"])
                    userName = _configuration["DefaultUserDetails:SystemAdminEmail"];
                _logger.LogInformation($"About to login. Email:{userName}");
                #endregion

                var appUser = await _userManager.FindByEmailAsync(userName);
                if (appUser == null)
                {
                    _logger.LogInformation($"Login not successful. User does not exist: {userName}");
                    authResult.Message = "User does not exist";
                    authResult.IsSuccess = false;
                    return authResult;
                }
                if (true)
                {
                    appUser.Status = Status.Active;
                    appUser.EmailConfirmed = true;
                }

                var isLoginValid = await _userManager.CheckPasswordAsync(appUser, password);

                if (!isLoginValid)
                {
                    await _userManager.AccessFailedAsync(appUser);
                    if (await _userManager.IsLockedOutAsync(appUser))
                    {
                        _logger.LogInformation($"Login not successful. Incorrect Password! User Account has been locked out: {userName}");
                        authResult.Message = "Incorrect Password! User Account has been locked out";
                        authResult.IsSuccess = false;
                        return authResult;
                    }
                    _logger.LogInformation($"Login not successful. Incorrect Password! User Login was not successful: {userName}");
                    authResult.Message = "Incorrect Password! User Login was not successful";
                    authResult.IsSuccess = false;
                    return authResult;
                }

                if (appUser.Status == Status.Inactive || appUser.Status == Status.Deactivated)
                {
                    _logger.LogInformation($"Login not successful. You have been disabled by Administrator. Contact your administrator for more information: {userName}");
                    authResult.Message = "You have been disabled by Administrator. Contact your administrator for more information";
                    authResult.IsSuccess = false;
                    return authResult;
                }

                _logger.LogInformation($"User Login was successful: {userName}");
                authResult.Message = "User Login was successful";
                authResult.IsSuccess = true;

                if (appUser.RoleId != null && appUser.RoleId > 0)
                {
                    var userRole = await _context.Roles.Include(C=>C.RolePermissions).FirstOrDefaultAsync(x => x.Id == appUser.RoleId);
                    if (userRole != null)
                    {
                        appUser.Role = userRole;
                    }
                }
                authResult.Token = await _tokenService.GenerateAccessToken(appUser);

                if (appUser.Role != null && appUser.Role.RolePermissions != null)
                {
                    authResult.Token.UserToken = await _tokenService.GenerateUserToken(appUser, appUser.Role.RolePermissions.ToList());
                }
                
                //Add refreshtoken details to the db
                await _userManager.ResetAccessFailedCountAsync(appUser);
                await _userManager.UpdateAsync(appUser);
                CancellationToken cancellationToken = new CancellationToken();
                await _context.SaveChangesAsync(cancellationToken);

                return authResult;

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error logging in : " + ex?.Message + ex?.InnerException?.Message);
                authResult.Message = "Error logging in : " + ex?.Message + ex?.InnerException?.Message;
                authResult.IsSuccess = false;
                return authResult;
            }
        }

        public async Task<Result> LogOut(string userName)
        {
            try
            {
                var appUser = await _userManager.FindByNameAsync(userName);
                if (appUser == null)
                {
                    return Result.Failure($"User does not exist");
                }
                appUser.LastLoginDate = DateTime.Now;
                await _userManager.UpdateAsync(appUser);
                return Result.Success("User Logout was successful");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error logging out:{ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        public async Task<Result> ForgotPassword(string email)
        {
            var appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null)
            {
                return Result.Failure("User does not exist with this email");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            return Result.Success("Password reset token successful", token);
        }
        public async Task<Result> ResetPassword(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return Result.Failure("Invalid email");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var res = await _userManager.ResetPasswordAsync(user, token, password);
            if (res.Succeeded)
            {

                return Result.Success("Password Reset Successful", true);
            }
            return Result.Failure("Reset Password Failed");
        }

    }

}
