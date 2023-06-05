using Application.Context;
using AutoMapper;
using Domain.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Carts.Queries
{
    public class GetCartItemsByCustomerIdQuery : IRequest<Result>
    {
        public int CustomerId { get; set; }
        public string UserId { get; set; }
    }

    public class GetCartItemsByCustomerIdQueryHandler : IRequestHandler<GetCartItemsByCustomerIdQuery, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCartItemsByCustomerIdQueryHandler> _logger;

        public GetCartItemsByCustomerIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper, ILogger<GetCartItemsByCustomerIdQueryHandler> logger)
        {
            _context = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> Handle(GetCartItemsByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the cart items for the specified customer ID and user ID
                var cartItems = await _context.CartItems
                    .Include(ci => ci.Item)
                    .Where(c => c.Cart.CustomerId == request.CustomerId && c.Cart.Customer.UserId == request.UserId)
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

