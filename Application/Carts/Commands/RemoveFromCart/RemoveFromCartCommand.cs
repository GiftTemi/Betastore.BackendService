using Application.Context;
using Application.Interfaces;
using Domain.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Carts.Commands
{
    public class RemoveFromCartCommand : IRequest<Result>
    {
        public int CustomerId { get; set; }
        public int ItemId { get; set; }
        public string UserId { get; set; }
    }

    public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ILogger<RemoveFromCartCommandHandler> _logger;

        public RemoveFromCartCommandHandler(IApplicationDbContext dbContext, IIdentityService identityService, ILogger<RemoveFromCartCommandHandler> logger)
        {
            _context = dbContext;
            _identityService = identityService;
            _logger = logger;
        }

        public async Task<Result> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
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
                    return Result.Failure("You can't remove items from someone else's cart");
                }

                // Retrieve the user's cart from the database
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId);

                if (cart == null)
                    return Result.Failure("Cart not found");

                // Find the cart item to be removed
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ItemId == request.ItemId);
                if (cartItem == null)
                    return Result.Failure("Item not found in the cart");

                // Remove the cart item from the cart
                cart.CartItems.Remove(cartItem);
                //_context.Carts.UpdateRange(cart);
                // Save the changes to the database
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success("Item removed from cart successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing item from cart: {ex.Message}");
                return Result.Failure("Error removing item from cart");
            }
        }
    }
}
