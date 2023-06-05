using Application.Common.Exceptions;
using Application.Purchases.Commands;
using Application.Purchases.Queries;
using Domain.Common.Models;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CustomerPurchaseController : ApiController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CustomerPurchaseController> _logger;

        public CustomerPurchaseController(IHttpContextAccessor httpContextAccessor, ILogger<CustomerPurchaseController> logger)
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
        public async Task<ActionResult<Result>> CreatePurchase(CreatePurchaseCommand command)
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
                _logger.LogError($"Backend Service at {DateTime.Now}. Purchase creation was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Purchase creation was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult<Result>> GetPurchaseById(int id)
        {
            try
            {
                return await Mediator.Send(new GetPurchaseByIdQuery { PurchaseId = id });
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. Purchase retrieval by id was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Purchase retrieval by id was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbycustomer/{customerId}/{userId}")]
        public async Task<ActionResult<Result>> GetPurchasesByCustomer(int customerId, string userId)
        {
            try
            {
                accessToken.ValidateToken(userId);
                return await Mediator.Send(new GetPurchasesByCustomerQuery { CustomerId = customerId , UserId=userId});
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. Purchase retrieval by customer was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Purchase retrieval by customer was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getall")]
        public async Task<ActionResult<Result>> GetAllPurchases()
        {
            try
            {
                return await Mediator.Send(new GetAllPurchasesQuery());
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. Purchase retrieval was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Purchase retrieval was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}