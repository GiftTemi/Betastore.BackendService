using Application.Context;
using Application.Interfaces;
using Domain.Common.Models;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Items.Commands
{
    public class CreateItemCommand : IRequest<Result>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int StockQuantity { get; set; }
        public string UserId { get; set; }
    }

    public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreateItemCommandHandler> _logger;
        private readonly IIdentityService _identityService;

        public CreateItemCommandHandler(IApplicationDbContext dbContext, ILogger<CreateItemCommandHandler> logger, IIdentityService identityService)
        {
            _context = dbContext;
            _logger = logger;
            _identityService = identityService;
        }

        public async Task<Result> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify the user
                (Result userResult, User user) = await _identityService.GetUserById(request.UserId);
                if (!userResult.Succeeded)
                {
                    return Result.Failure("User not found");
                }

                // Create the item
                var item = new Item
                {
                    Description = request.Description,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity,
                    CreatedBy = user.Email,
                    CreatedById = user.Id,
                    CreatedDate= DateTime.Now
                };

                // Save the item to the database
                _context.Items.Add(item);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success("Item created successfully", item.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now} - Error creating new item - response parameters, {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Item creation was not successful due to {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
