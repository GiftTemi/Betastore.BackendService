using Application.Common.Exceptions;
using Application.DiscountProfileCommand.Commands;
using Application.DiscountProfiles.Commands;
using Domain.Common.Models;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DiscountProfileController : ApiController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DiscountProfileController> _logger;

        public DiscountProfileController(IHttpContextAccessor httpContextAccessor, ILogger<DiscountProfileController> logger)
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
        public async Task<ActionResult<Result>> Create(CreateDiscountProfileCommand command)
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
                _logger.LogError($"Backend Service at {DateTime.Now}. Discount profile creation was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Discount profile creation was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("update/{profileid}")]
        public async Task<ActionResult<Result>> Update(string profileid, UpdateDiscountProfileCommand command)
        {
            try
            {
                if (command == null)
                {
                    return BadRequest("Invalid Request");
                }
                accessToken.ValidateToken(profileid);

                return await Mediator.Send(command);
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. Discount profile update was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Discount profile update was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
        #region Commented code

        //[HttpPost("getall")]
        //public async Task<ActionResult<Result>> GetAll(GetDiscountProfilesQuery query)
        //{
        //    try
        //    {
        //        if (query == null)
        //        {
        //            return BadRequest("Invalid Request");
        //        }
        //        accessToken.ValidateToken(query.UserId);
        //        return await Mediator.Send(query);
        //    }
        //    catch (ValidationException ex)
        //    {
        //        return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Backend Service at {DateTime.Now}. Discount profile retrieval was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
        //        return Result.Failure($"Discount profile retrieval was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
        //    }
        //}

        //[HttpGet("getbyid/{id}/{userid}")]
        //public async Task<ActionResult<Result>> GetById(string id, string userid)
        //{
        //    try
        //    {
        //        accessToken.ValidateToken(userid);
        //        return await Mediator.Send(new GetDiscountProfileByIdQuery { Id = id });
        //    }
        //    catch (ValidationException ex)
        //    {
        //        return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Backend Service at {DateTime.Now}. Discount profile retrieval was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
        //        return Result.Failure($"Discount profile retrieval by id was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
        //    }
        //}

        //[HttpGet("getbyuserid/{userid}")]
        //public async Task<ActionResult<Result>> GetByUserId(string userid)
        //{
        //    try
        //    {
        //        accessToken.ValidateToken(userid);
        //        return await Mediator.Send(new GetDiscountProfileByUserIdQuery { UserId = userid });
        //    }
        //    catch (ValidationException ex)
        //    {
        //        return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Backend Service at {DateTime.Now}. Discount profile retrieval was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
        //        return Result.Failure($"Discount profile retrieval by user id was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
        //    }
        //} 
        #endregion
    }
}
