using Application.Context;
using Application.Interfaces;
using Domain.Common.Models;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.DiscountProfiles.Commands
{
    public class CreateDiscountProfileCommand : IRequest<Result>
    {
        public string Name { get; set; }
        public DiscountType DiscountType { get; set; }
        public int MinimumMonthsAsMember { get; set; }
        public double MinimumAmountSpent { get; set; }
        public double DiscountPercentage { get; set; }
        public string UserId { get; set; }
    }

    public class CreateDiscountProfileCommandHandler : IRequestHandler<CreateDiscountProfileCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreateDiscountProfileCommand> _logger;
        private readonly IIdentityService _identityService;

        public CreateDiscountProfileCommandHandler(IApplicationDbContext dbContext, ILogger<CreateDiscountProfileCommand> logger, IIdentityService identityService)
        {
            _context = dbContext;
            _logger = logger;
            _identityService = identityService;
        }

        public async Task<Result> Handle(CreateDiscountProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify user using IIdentityService
                var user = await _identityService.GetUserById(request.UserId);
                if (!user.Result.Succeeded || user.user == null)
                    return Result.Failure("User not found");

                var discountProfile = new DiscountProfile
                {
                    Name = request.Name,
                    DiscountType = request.DiscountType,
                    QualificationRequirement = new QualificationRequirement
                    {
                        MinimumAmountSpent = request.MinimumAmountSpent,
                        MinimumMonthsAsMember = request.MinimumMonthsAsMember
                    },
                    DiscountPercentage = request.DiscountPercentage,
                    CreatedDate = DateTime.UtcNow,
                    Status = Status.Active,
                    CreatedBy = user.user.Email,
                    CreatedById = user.user.UserId,
                };

                _context.DiscountProfiles.Add(discountProfile);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success(discountProfile.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating discount profile: {ex.Message}");
                return Result.Failure("Error creating discount profile");
            }
        }
    }
}
