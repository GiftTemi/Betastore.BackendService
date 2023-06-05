using Application.Context;
using Application.Interfaces;
using Domain.Common.Models;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Carts.Commands
{

    public class AddToCartCommand : IRequest<Result>
    {
        public int CustomerId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string UserId { get; set; }
    }

    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ILogger<AddToCartCommandHandler> _logger;

        public AddToCartCommandHandler(IApplicationDbContext dbContext, IIdentityService identityService, ILogger<AddToCartCommandHandler> logger)
        {
            _context = dbContext;
            _identityService = identityService;
            _logger = logger;
        }

        public async Task<Result> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {

            try
            {
                // Verify user using IIdentityService
                var (userResult, user) = await _identityService.GetUserById(request.UserId);
                if (!userResult.Succeeded || user == null)
                    return Result.Failure("User not found");

                var customer = await _context.Customers.FindAsync(request.CustomerId);
                if (customer == null)
                    return Result.Failure("Customer not found");
                if (customer.UserId != request.UserId)
                {
                    return Result.Failure("You can't add items to someone else's cart");
                }
                // Retrieve the item from the database
                var item = await _context.Items.FindAsync(request.ItemId);
                if (item == null)
                    return Result.Failure("Item not found");

                // Retrieve the user's cart from the database
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId);

                if (cart == null)
                {
                    // Create a new cart for the user if it doesn't exist
                    cart = new Cart
                    {
                        CustomerId = request.CustomerId,
                        CartItems = new List<CartItem>()
                    };
                }

                // Check if the item is already in the cart
                var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ItemId == request.ItemId);
                if (existingCartItem != null)
                {
                    // Update the quantity of the existing cart item
                    existingCartItem.Quantity += request.Quantity;
                }
                else
                {
                    // Create a new cart item for the item
                    var cartItem = new CartItem
                    {
                        ItemId = item.Id,
                        Quantity = request.Quantity,
                        CreatedBy = user.Email,
                        CreatedById = user.Id,
                        CreatedDate = DateTime.UtcNow
                    };

                    cart.CartItems.Add(cartItem);
                }

                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success("Item added to cart successfully");
            }
            catch (Exception ex)
            {
                // Log and handle the exception
                _logger.LogError($"Error adding item to cart: {ex.Message}");
                return Result.Failure("Error adding item to cart");


                _logger.LogError($"Backend Service at {DateTime.Now} - Error adding item to cart - response parameters, {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Error adding item to cart due to {ex?.Message ?? ex?.InnerException?.Message}");

            }
        }
    }
}