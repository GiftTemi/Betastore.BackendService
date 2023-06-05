using Application.Context;
using Domain.Common.Models;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.DiscountProfileCommand.Queries
{
    public class GetApplicableDiscountQuery : IRequest<Result>
    {
        public int CustomerId { get; set; }
        public string UserId { get; set; }
    }
    public class GetApplicableDiscountQueryHandler : IRequestHandler<GetApplicableDiscountQuery, Result>
    {
        private readonly IApplicationDbContext _context;

        public GetApplicableDiscountQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetApplicableDiscountQuery request, CancellationToken cancellationToken)
        {
            // Retrieve the customer's profile
            var customer = await _context.Customers.FindAsync(request.CustomerId);
            if (customer == null)
            {
                return Result.Success("Customer not found");
            }

            // Retrieve all applicable discount profiles based on criteria
            var applicableDiscounts = await _context.DiscountProfiles
               .Where(d =>
               //DiscountType is MembershipDuration and the customer's membership duration is greater or equal to the Configured Membership Duration
               (d.DiscountType == DiscountType.MembershipDuration && CalculateMembershipDuration(customer.DateJoined) >= d.QualificationRequirement.MinimumMonthsAsMember)

               //DiscountType is AmountSpent and the customer's TotalAmountSpent is greater or equal to the Configured MinimumAmountSpent
               || (d.DiscountType == DiscountType.AmountSpent && customer.TotalAmountSpent >= d.QualificationRequirement.MinimumAmountSpent))

               //Pick the best of the two
               .OrderByDescending(d => d.DiscountPercentage)
               .ToListAsync();

            return Result.Success(applicableDiscounts);
        }

        private int CalculateMembershipDuration(DateTime dateJoined)
        {
            var today = DateTime.Today;
            var monthsAsMember = (today.Year - dateJoined.Year) * 12 + today.Month - dateJoined.Month;
            return monthsAsMember;
        }
    }

}
