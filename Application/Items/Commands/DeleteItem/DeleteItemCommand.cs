using Application.Context;
using Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Items.Commands
{
    public class DeleteItemCommand : IRequest<Result>
    {
        public string ItemId { get; set; }
        public string UserId { get; set; }
    }
    public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DeleteItemCommandHandler> _logger;

        public DeleteItemCommandHandler(IApplicationDbContext dbContext, ILogger<DeleteItemCommandHandler> logger)
        {
            _context = dbContext;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _context.Items.FindAsync(request.ItemId);

                if (item == null)
                {
                    return Result.Failure($"Item with ID '{request.ItemId}' not found.");
                }

                _context.Items.Remove(item);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success("Item deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now} - Error deleting item with ID '{request.ItemId}': {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Item deletion failed due to an error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }

}
