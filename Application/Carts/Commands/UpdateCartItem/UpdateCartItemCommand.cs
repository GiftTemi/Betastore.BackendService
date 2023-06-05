using Application.Context;
using Application.Interfaces;
using Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Carts.Commands
{
    public class UpdateCartItemCommand : IRequest<Result>
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
        public string UserId { get; set; }
    }

    public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ILogger<UpdateCartItemCommandHandler> _logger;

        public UpdateCartItemCommandHandler(IApplicationDbContext dbContext, IIdentityService identityService, ILogger<UpdateCartItemCommandHandler> logger)
        {
            _context = dbContext;
            _identityService = identityService;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify user using IIdentityService
                var (userResult, user) = await _identityService.GetUserById(request.UserId);
                if (!userResult.Succeeded || user == null)
                    return Result.Failure("User not found");

                // Retrieve the cart item from the database
                var cartItem = await _context.CartItems.FindAsync(request.CartItemId);
                if (cartItem == null)
                    return Result.Failure("Cart item not found");

                // Update the quantity of the cart item
                cartItem.Quantity = request.Quantity;
                cartItem.LastModifiedBy = user.Email;
                cartItem.LastModifiedById = user.Id;
                cartItem.LastModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success("Cart item updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating cart item: {ex.Message}");
                return Result.Failure("Error updating cart item");
            }
        }
    }
}

