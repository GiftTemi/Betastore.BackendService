using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Domain.Common.Models;
using Infrastructure.Utility;
using Application.Carts.Commands;
using Application.Common.Exceptions;
using Application.Carts.Queries;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CartController : ApiController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CartController> _logger;

        public CartController(IHttpContextAccessor httpContextAccessor, ILogger<CartController> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString()?.ExtractToken();
            if (accessToken == null)
            {
                throw new Exception("You are not authorized!");
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<Result>> AddToCart(AddToCartCommand command)
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
                _logger.LogError($"Backend Service at {DateTime.Now}. Adding item to cart was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Adding item to cart was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("remove")]
        public async Task<ActionResult<Result>> RemoveFromCart(RemoveFromCartCommand command)
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
                _logger.LogError($"Backend Service at {DateTime.Now}. Removing item from cart was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Removing item from cart was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("update")]
        public async Task<ActionResult<Result>> UpdateCartItem(UpdateCartItemCommand command)
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
                _logger.LogError($"Backend Service at {DateTime.Now}. Updating cart item was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Updating cart item was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
        [HttpGet("getbycustomerid/{userid}/{customerid}")]
        public async Task<ActionResult<Result>> GetCartItemsByUserId(string userid, int customerid)
        {
            try
            {
                accessToken.ValidateToken(userid);
                return await Mediator.Send(new GetCartItemsByCustomerIdQuery { UserId = userid, CustomerId = customerid });
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. Retrieving cart items by user id was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Retrieving cart items by user id was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbycartid/{cartid}")]
        public async Task<ActionResult<Result>> GetCartItemsByCartId(int cartid)
        {
            try
            {
                return await Mediator.Send(new GetCartItemsByCartIdQuery { CartId = cartid });
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. Retrieving cart items by cart id was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Retrieving cart items by cart id was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}

