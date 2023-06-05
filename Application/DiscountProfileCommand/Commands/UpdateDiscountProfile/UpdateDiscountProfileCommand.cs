using Application.Context;
using Domain.Common.Models;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.DiscountProfileCommand.Commands
{
    public class UpdateDiscountProfileCommand : IRequest<Result>
    {
        public int DiscountProfileId { get; set; }
        public string Name { get; set; }
        public DiscountType DiscountType { get; set; }
        public QualificationRequirement QualificationRequirement { get; set; }
        public double DiscountPercentage { get; set; }
        public string UserId { get; set; }
    }

    public class UpdateDiscountProfileCommandHandler : IRequestHandler<UpdateDiscountProfileCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UpdateDiscountProfileCommandHandler> _logger;

        public UpdateDiscountProfileCommandHandler(IApplicationDbContext dbContext, ILogger<UpdateDiscountProfileCommandHandler> logger)
        {
            _context = dbContext;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateDiscountProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify user using the provided UserId


                // Retrieve the discount profile from the database
                var discountProfile = await _context.DiscountProfiles.Include(c => c.QualificationRequirement).FirstOrDefaultAsync(x => x.Id == request.DiscountProfileId);

                // If the discount profile is not found, return a failure result
                if (discountProfile == null)
                    return Result.Failure("Discount profile not found");

                // Update the properties of the discount profile
                discountProfile.Name = request.Name;
                discountProfile.DiscountType = request.DiscountType;
                discountProfile.QualificationRequirement = request.QualificationRequirement;
                discountProfile.DiscountPercentage = request.DiscountPercentage;
                discountProfile.LastModifiedDate = DateTime.UtcNow;

                // Save the changes to the database
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success("Discount profile updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating discount profile: {ex.Message}");
                return Result.Failure("Error updating discount profile");
            }
        }
    }
}
