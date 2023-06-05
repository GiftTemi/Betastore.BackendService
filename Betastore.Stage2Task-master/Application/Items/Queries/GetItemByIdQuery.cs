using Application.Context;
using Application.Interfaces;
using AutoMapper;
using Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Items.Queries
{
    public class GetItemByIdQuery : IRequest<Result>
    {
        public string ItemId { get; set; }
        public string UserId { get; set; }
    }

    public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public GetItemByIdQueryHandler(IApplicationDbContext dbContext, IIdentityService identityService, IMapper mapper)
        {
            _context = dbContext;
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<Result> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
        {
            // Verify user using IIdentityService
            var (userResult, user) = await _identityService.GetUserById(request.UserId);
            if (!userResult.Succeeded || user == null)
                return Result.Failure("User not found");

            // Retrieve the item from the database
            var item = await _context.Items.FindAsync(request.ItemId);
            if (item == null)
                return Result.Failure("Item not found");

            // Map the item to ItemDto
            var items = _mapper.Map<ItemDto>(item);

            return Result.Success(items);
        }
    }


}
