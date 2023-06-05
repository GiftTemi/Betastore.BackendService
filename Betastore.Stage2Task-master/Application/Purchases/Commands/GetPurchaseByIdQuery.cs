using Application.Context;
using Application.Interfaces;
using AutoMapper;
using Domain.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Purchases.Queries
{
    public class GetPurchaseByIdQuery : IRequest<Result>
    {
        public int PurchaseId { get; set; }
        public string UserId { get; set; }
    }

    public class GetPurchaseByIdQueryHandler : IRequestHandler<GetPurchaseByIdQuery, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;

        private readonly ILogger<GetPurchaseByIdQueryHandler> _logger;

        public GetPurchaseByIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper, ILogger<GetPurchaseByIdQueryHandler> logger, IIdentityService identityService)
        {
            _context = dbContext;
            _mapper = mapper;
            _logger = logger;
            _identityService = identityService;
        }

        public async Task<Result> Handle(GetPurchaseByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify user using IIdentityService
                var (userResult, user) = await _identityService.GetUserById(request.UserId);
                if (!userResult.Succeeded || user == null)
                    return Result.Failure("User not found");

                // Retrieve the purchase by ID
                var purchase = await _context.Purchases
                    .Include(p => p.PurchaseItems)
                    .FirstOrDefaultAsync(p => p.Id == request.PurchaseId);

                if (purchase == null)
                    return Result.Failure("Purchase not found");

                // Map the purchase to DTO
                var purchaseDto = _mapper.Map<PurchaseDto>(purchase);

                return Result.Success(purchaseDto);
            }
            catch (Exception ex)
            {
                // Log and handle the exception
                _logger.LogError($"Error retrieving purchase: {ex.Message}");
                return Result.Failure("Error retrieving purchase");
            }
        }
    }
}
