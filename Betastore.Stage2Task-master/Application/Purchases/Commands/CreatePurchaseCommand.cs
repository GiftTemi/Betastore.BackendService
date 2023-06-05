using Application.Context;
using Application.Interfaces;
using Domain.Common.Models;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Purchases.Commands
{
    public class CreatePurchaseCommand : IRequest<Result>
    {
        public int CartId { get; set; }
        public string UserId { get; set; }
        public bool? DiscountApplied { get; set; } = false;
        public string? DisountProfileName { get; set; }
    }

    public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ILogger<CreatePurchaseCommandHandler> _logger;

        public CreatePurchaseCommandHandler(IApplicationDbContext dbContext, IIdentityService identityService, ILogger<CreatePurchaseCommandHandler> logger)
        {
            _context = dbContext;
            _identityService = identityService;
            _logger = logger;
        }

        public async Task<Result> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify user using IIdentityService
                var (userResult, user) = await _identityService.GetUserById(request.UserId);
                if (!userResult.Succeeded || user == null)
                    return Result.Failure("User not found");

                // Retrieve the cart for the specified cart ID and user ID
                var cart = await _context.Carts
                    .Include(c => c.CartItems).ThenInclude(c => c.Item)
                    .FirstOrDefaultAsync(c => c.Id == request.CartId && c.Customer.UserId == request.UserId);

                if (cart == null)
                    return Result.Failure("Cart not found");

                // Create a new purchase based on the cart
                var purchase = new Purchase
                {
                    CustomerId = cart.CustomerId,
                    UserId = user.Id,
                    PurchaseDate = DateTime.UtcNow,
                    PurchaseItems = cart.CartItems.Select(ci => new PurchaseItem
                    {
                        ItemId = ci.ItemId,
                        Quantity = ci.Quantity,
                        Price = ci.Item.Price
                    }).ToList()
                };
                if (request.DiscountApplied.HasValue && request.DiscountApplied == true)
                {
                    var discountProfile = await _context.DiscountProfiles.FirstOrDefaultAsync(d => d.Name == request.DisountProfileName);

                    if (discountProfile != null && discountProfile.Status == Domain.Enums.Status.Active) 
                    {
                        purchase.DiscountAmount = (discountProfile.DiscountPercentage / 100) * purchase.OriginalAmount;
                        purchase.DiscountApplied = request.DiscountApplied.Value;
                    }
                }

                // Add the purchase to the database
                _context.Purchases.Add(purchase);

                // Remove the cart and its items from the database
                _context.Carts.Remove(cart);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success("Purchase created successfully");
            }
            catch (Exception ex)
            {
                // Log and handle the exception
                _logger.LogError($"Error creating purchase: {ex.Message}");
                return Result.Failure("Error creating purchase");
            }
        }
    }
}