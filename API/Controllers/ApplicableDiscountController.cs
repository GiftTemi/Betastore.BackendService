using Application.Common.Exceptions;
using Application.DiscountProfileCommand.Queries;
using Domain.Common.Models;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApplicableDiscountController : ApiController
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CartController> _logger;

        public ApplicableDiscountController(IHttpContextAccessor httpContextAccessor, ILogger<CartController> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString()?.ExtractToken();
            if (accessToken == null)
            {
                throw new Exception("You are not authorized!");
            }
        }

        [HttpPost("getdiscountprofiles")]
        public async Task<ActionResult<Result>> GetDiscountProfiles(GetDiscountProfilesQuery query)
        {
            try
            {
                accessToken.ValidateToken(query.UserId);
                return await Mediator.Send(query);
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. Error retrieving discount profiles: {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Error retrieving discountprofiles: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("getapplicablediscounts")]
        public async Task<ActionResult<Result>> GetApplicableDiscount(GetApplicableDiscountQuery query)
        {
            try
            {
                accessToken.ValidateToken(query.UserId);
                return await Mediator.Send(query);
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. Error retrieving applicable discount: {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Error retrieving applicable discount: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbestapplicablediscount")]
        public async Task<ActionResult<Result>> GetBestApplicableDiscountQuery(GetBestApplicableDiscountQuery query)
        {
            try
            {
                accessToken.ValidateToken(query.UserId);
                return await Mediator.Send(query);
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. Error retrieving applicable discount: {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Error retrieving the best applicable discount: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
