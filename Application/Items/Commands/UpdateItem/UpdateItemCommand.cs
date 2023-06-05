using Application.Context;
using Application.Interfaces;
using Domain.Common.Models;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Items.Commands
{
    public class UpdateItemCommand : IRequest<Result>
    {
        public string ItemId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; internal set; }
    }
    public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UpdateItemCommandHandler> _logger;
        private readonly IIdentityService _identityService;

        public UpdateItemCommandHandler(IApplicationDbContext dbContext, ILogger<UpdateItemCommandHandler> logger, IIdentityService identityService)
        {
            _context = dbContext;
            _logger = logger;
            _identityService = identityService;
        }

        public async Task<Result> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify the user
                (Result userResult, User user) = await _identityService.GetUserById(request.UserId);
                if (!userResult.Succeeded)
                {
                    return Result.Failure("User not found");
                }

                // Get the item from the database
                var item = await _context.Items.FindAsync(request.ItemId);
                if (item == null)
                {
                    return Result.Failure("Item not found");
                }

                // Update the item
                item.Description = request.Description;
                item.Price = request.Price;
                item.StockQuantity = request.StockQuantity;
                item.LastModifiedBy = user.Email;
                item.LastModifiedById = user.Id;
                item.LastModifiedDate = DateTime.UtcNow;

                // Save the updated item to the database
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success("Item updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now} - Error updating item - response parameters, {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Item update was not successful due to {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }

}
