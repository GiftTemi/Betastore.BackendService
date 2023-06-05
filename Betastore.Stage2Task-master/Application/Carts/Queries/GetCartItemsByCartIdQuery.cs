using Application.Context;
using Application.Interfaces;
using AutoMapper;
using Domain.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Carts.Queries
{
    public class GetCartItemsByCartIdQuery : IRequest<Result>
    {
        public int CartId { get; set; }
        public string UserId { get; set; }
    }

    public class GetCartItemsByCartIdQueryHandler : IRequestHandler<GetCartItemsByCartIdQuery, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCartItemsByCartIdQueryHandler> _logger;
        private readonly IIdentityService _identityService;

        public GetCartItemsByCartIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper, ILogger<GetCartItemsByCartIdQueryHandler> logger, IIdentityService identityService)
        {
            _context = dbContext;
            _mapper = mapper;
            _logger = logger;
            _identityService = identityService;
        }

        public async Task<Result> Handle(GetCartItemsByCartIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify user using IIdentityService
                var user = await _identityService.GetUserById(request.UserId);
                if (!user.Result.Succeeded || user.user == null)
                    return Result.Failure("User not found");

                // Retrieve the cart items for the specified cart ID and user ID
                var cartItems = await _context.CartItems
                    .Include(ci => ci.Item)
                    .Include(ci => ci.Cart)
                    .Where(ci => ci.Cart.Id == request.CartId && ci.Cart.Customer.UserId == request.UserId)
                    .ToListAsync();

                // Map the cart items to DTOs
                var cartItemDtos = _mapper.Map<List<CartItemDto>>(cartItems);

                return Result.Success(cartItemDtos);
            }
            catch (Exception ex)
            {
                // Log and handle the exception
                _logger.LogError($"Error retrieving cart items: {ex.Message}");
                return Result.Failure("Error retrieving cart items");
            }
        }
    }
}
