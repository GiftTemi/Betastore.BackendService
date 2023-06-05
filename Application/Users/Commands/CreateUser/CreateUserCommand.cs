using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using Application.Users.Queries;
using Domain.Common.Models;
using Application.Context;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using static Domain.Constants.DefaultPermissions;

namespace Application.Users.Commands
{
    public class CreateUserCommand : IRequest<Result>
    {
        public string FirstName { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? UserId { get; set; }
        public string Address { get; set; }
        public int? RoleId { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<CreateUserCommand> _logger;
        private readonly IConfiguration _configuration;

        public CreateUserCommandHandler(IApplicationDbContext context, IIdentityService identityService,
            IMapper mapper, ILogger<CreateUserCommand> logger, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _identityService = identityService;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Backend Service - About to create user details - request parameters: " + request);
                var newUser = await _identityService.GetUserByEmail(request.Email);
                if (newUser.user != null)
                {

                    _logger.LogError($"Backend Service at {DateTime.Now} - User with this email already exists");
                    return Result.Failure("User with this email already exists");
                }
                User adminUser = null;
                if (!string.IsNullOrEmpty(request.UserId))
                {
                    var adminDetail = await _identityService.GetUserById(request.UserId);
                    if (adminDetail.user != null)
                    {
                        adminUser = adminDetail.user;
                    }
                }
                int? roleId = await _context.Roles.Where(x => x.Id == request.RoleId).Select(x => (int?)x.Id).FirstOrDefaultAsync();

                var user = new User
                {
                    Email = request.Email.Trim(),
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    Password = request.Password.Trim(),
                    UserName = request.UserName.Trim(),
                    EmailConfirmed = false,
                    PhoneNumber = request.Phone,
                    Address = request.Address,
                    Status = Status.Active,
                    RoleId = roleId,
                    CreatedDate = DateTime.Now,
                    CreatedBy = adminUser == null ? "SYSTEM" : adminUser.UserName,
                    AccessLevel = AccessLevel.Customer
                };

                string webDomain = _configuration["WebDomain:Backend"];

                var hashValue = (user.Email + DateTime.Now).ToString();
                

                var result = await _identityService.CreateUserAsync(user);
                
                if (!result.Result.Succeeded)
                {
                    _logger.LogError($"Backend Service at {DateTime.Now} - Error creating new user details - {result.Result.Message}");
                    return Result.Failure(result.Result.Message);
                }
                if (user.AccessLevel == AccessLevel.Customer)
                {
                    var customer = new Domain.Entities.Customer
                    {
                        UserId = result.UserId,
                        CreatedBy = request.Email,
                        CreatedDate = user.CreatedDate,
                    };
                    await _context.Customers.AddAsync(customer);
                }                        

                await _context.SaveChangesAsync(cancellationToken);

                var userResult = _mapper.Map<UserDto>(user);
                _logger.LogInformation("Backend Service - Done creating new user details - response parameters: " + userResult);
                return Result.Success(" User creation was successful", userResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now} - Error creating new user details - response parameters, {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"User creation was not successful due to {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        private async Task SendEmail(User adminUser, User user, string webDomain, IEmailService _emailService)
        {
            _logger.LogInformation("Backend Service - About to send notification email.");
            var email = new EmailVm();

            //admin notification
            email=new EmailVm
            {
                Application = "Backend ",
                Subject = $"New user {adminUser.UserName} has been created.",
                BCC = "",
                CC = "",
                RecipientEmail = adminUser.Email,
                FirstName = adminUser.Name,
                Body = $"Hello, A new user {adminUser.UserName} has been created. ",
                ButtonText = "Login",
                ButtonLink = webDomain + "/admin-login",
                DisplayButton = "display:none"
            };
            //send email
            //user notification
            email=new EmailVm
            {
                Application = "Backend ",
                Subject = "Welcome to Backend ",
                BCC = "",
                CC = "",
                RecipientEmail = user.Email,
                FirstName = user.Name,
                Body = $"Hello {user.UserName},Your account on Backend  has been created successfully!",
                ButtonText = "Login",
                ButtonLink = webDomain + "/admin-login",
                DisplayButton = "display:none"
            };

            await _emailService.SendForgotPasswordEmailAsync(email);

            _logger.LogInformation("Backend Service - Email notifications sent successfully!!");
        }
    }
}