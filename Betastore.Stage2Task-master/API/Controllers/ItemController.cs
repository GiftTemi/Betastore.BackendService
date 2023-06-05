using Application.Common.Exceptions;
using Application.Items.Commands;
using Application.Items.Queries;
using Domain.Common.Models;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ItemController : ApiController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ItemController> _logger;

        public ItemController(IHttpContextAccessor httpContextAccessor, ILogger<ItemController> logger)
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
        public async Task<ActionResult<Result>> CreateItem(CreateItemCommand command)
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
                _logger.LogError($"Backend Service at {DateTime.Now}. Item creation was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Item creation was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("update/{itemid}")]
        public async Task<ActionResult<Result>> UpdateItem(string itemid, UpdateItemCommand command)
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
                _logger.LogError($"Backend Service at {DateTime.Now}. Item update was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Item update was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("delete/{itemid}")]
        public async Task<ActionResult<Result>> DeleteItem(string itemid, DeleteItemCommand command)
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
                _logger.LogError($"Backend Service at {DateTime.Now}. Item deletion was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Item deletion was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyid/{id}/{userId}")]
        public async Task<ActionResult<Result>> GetItemById(string id, string userId)
        {
            try
            {
                return await Mediator.Send(new GetItemByIdQuery { ItemId = id, UserId = userId });
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. Item retrieval by id was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Item retrieval by id was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getall")]
        public async Task<ActionResult<Result>> GetAllItems()
        {
            try
            {
                return await Mediator.Send(new GetAllItemsQuery());
            }
            catch (ValidationException ex)
            {
                return Result.Failure($"{ex?.Message ?? ex?.InnerException?.Message}. Error: {ex.GetErrors()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now}. Item retrieval was not successful:  {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Item retrieval was not successful: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}