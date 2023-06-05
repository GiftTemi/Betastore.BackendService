using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Domain.Common.Models;
using Infrastructure.Utility;
using Application.Common.Exceptions;
using Application.Users.Commands;
using Application.Users.Queries;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ApiController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IHttpContextAccessor httpContextAccessor, ILogger<UsersController> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString()?.ExtractToken();
            if (accessToken == null)
            {
                throw new Exception("You are not authorized!");
            }
        }
        [HttpPost("create")]
        public async Task<ActionResult<Result>> Create(CreateUserCommand command)
        {
            try
            {
                if (command == null)
                {
                    return BadRequest("Invalid Request");
                }
                accessToken.ValidateToken(command.UserId);
                return await Mediator.Send(command);
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. User creation was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"User creation was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("update/{userid}")]
        public async Task<ActionResult<Result>> Update(string userid, UpdateUserCommand command)
        {
            try
            {
                if (command == null)
                {
                    return BadRequest("Invalid Request");
                }
                accessToken.ValidateToken(userid);

                return await Mediator.Send(command);
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. User update was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"User update was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }


        [HttpPost("changeuserstatus")]
        public async Task<ActionResult<Result>> ChangeUserStatus(ChangeUserStatusCommand command)
        {
            try
            {
                if (command == null)
                {
                    return BadRequest("Invalid Request");
                }
                accessToken.ValidateToken(command.LoggedInUserId);
                return await Mediator.Send(command);
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. User update was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Changing User Status was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("getall")]
        public async Task<ActionResult<Result>> GetAll(GetUsersQuery query)
        {
            try
            {
                if (query == null)
                {
                    return BadRequest("Invalid Request");
                }
                accessToken.ValidateToken(query.UserId);
                return await Mediator.Send(query);
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. User update was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"User retrieval was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
        [HttpPost("getallcustomers")]
        public async Task<ActionResult<Result>> GetAllCustomers(GetUsersQuery query)
        {
            try
            {
                if (query == null)
                {
                    return BadRequest("Invalid Request");
                }
                accessToken.ValidateToken(query.UserId);
                return await Mediator.Send(query);
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. User update was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"User retrieval was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyid/{id}/{userid}")]
        public async Task<ActionResult<Result>> GetById(string id, string userid)
        {
            try
            {
                accessToken.ValidateToken(userid);
                return await Mediator.Send(new GetUserByIdQuery { Id = id });
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. User retrieval was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"User retrieval by id was not successful{ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
        [HttpGet("getbyemail/{email}")]
        public async Task<ActionResult<Result>> GetUserByUsernameOrEmail(string email)
        {
            try
            {
                return await Mediator.Send(new GetUserByUsernameOrEmailQuery { Email = email });
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. User retrieval was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"User retrieval by id was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyusername/{username}/{userid}")]
        public async Task<ActionResult<Result>> GetByUserName(string username, string userid)
        {
            try
            {
                accessToken.ValidateToken(userid);
                return await Mediator.Send(new GetUserByUsernameOrEmailQuery { UserName = username });
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. User retrieval was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"User retrieval by username was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
