using Application.Context;
using Application.Interfaces;
using AutoMapper;
using Domain.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Purchases.Queries
{
    public class GetPurchasesByCustomerQuery : IRequest<Result>
    {
        public int CustomerId { get; set; }
        public string UserId { get; set; }
    }

    public class GetPurchasesByCustomerQueryHandler : IRequestHandler<GetPurchasesByCustomerQuery, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetPurchasesByCustomerQueryHandler> _logger;

        public GetPurchasesByCustomerQueryHandler(IApplicationDbContext dbContext, IMapper mapper, ILogger<GetPurchasesByCustomerQueryHandler> logger, IIdentityService identityService)
        {
            _context = dbContext;
            _mapper = mapper;
            _logger = logger;
            _identityService = identityService;
        }

        public async Task<Result> Handle(GetPurchasesByCustomerQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify user using IIdentityService
                var (userResult, user) = await _identityService.GetUserById(request.UserId);
                if (!userResult.Succeeded || user == null)
                    return Result.Failure("User not found");

                // Retrieve the purchases for the specified customer ID and user ID
                var purchases = await _context.Purchases
                    .Include(p => p.Customer).ThenInclude(p => p.User)
                    .Where(p => p.CustomerId == request.CustomerId && p.UserId == request.UserId)
                    .ToListAsync();

                // Map the purchases to DTOs
                var purchaseDtos = _mapper.Map<List<PurchaseDto>>(purchases);

                return Result.Success(purchaseDtos);
            }
            catch (Exception ex)
            {
                // Log and handle the exception
                _logger.LogError($"Error retrieving purchases: {ex.Message}");
                return Result.Failure("Error retrieving purchases");
            }
        }
    }
}

