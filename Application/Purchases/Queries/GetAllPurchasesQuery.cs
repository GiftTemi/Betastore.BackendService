using Application.Context;
using AutoMapper;
using Domain.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Purchases.Queries
{
    public class GetAllPurchasesQuery : IRequest<Result>
    {
        public string UserId { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Search { get; set; }
        public List<int> Filters { get; set; }
    }

    public class GetAllPurchasesQueryHandler : IRequestHandler<GetAllPurchasesQuery, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllPurchasesQueryHandler> _logger;

        public GetAllPurchasesQueryHandler(IApplicationDbContext dbContext, IMapper mapper, ILogger<GetAllPurchasesQueryHandler> logger)
        {
            _context = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> Handle(GetAllPurchasesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Purchases
                    .Include(p => p.Customer)
                    .ThenInclude(c => c.User)
                    .Where(p => p.UserId == request.UserId);

                if (!string.IsNullOrEmpty(request.Search))
                {
                    query = query.Where(p => p.Name.Contains(request.Search));
                }

                //if (request.Filters != null && request.Filters.Any())
                //{
                //    query = query.Where(p => request.Filters.Contains(p.Name));
                //}

                var totalItems = await query.CountAsync();

                var purchases = await query
                    .Skip(request.Skip)
                    .Take(request.Take)
                    .ToListAsync();

                var purchaseDtos = _mapper.Map<List<PurchaseDto>>(purchases);

                return Result.Success(purchaseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving purchases: {ex.Message}");
                return Result.Failure("Error retrieving purchases");
            }
        }
    }
}
