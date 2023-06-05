using Application.Context;
using Domain.Common.Models;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.DiscountProfileCommand.Queries
{
    public class GetDiscountProfilesQuery : IRequest<Result>
    {
        public int CustomerId { get; set; }
        public string UserId { get; set; }
    }
    public class GetDiscountProfilesQueryHandler : IRequestHandler<GetDiscountProfilesQuery, Result>
    {
        private readonly IApplicationDbContext _context;

        public GetDiscountProfilesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetDiscountProfilesQuery request, CancellationToken cancellationToken)
        {
            // Retrieve the customer's profile
            var customer = await _context.Customers.FindAsync(request.CustomerId);
            if (customer == null)
            {
                return Result.Success("Customer not found");
            }

            var discounts = await _context.DiscountProfiles.ToListAsync();

            return Result.Success(discounts);
        }

        private int CalculateMembershipDuration(DateTime dateJoined)
        {
            var today = DateTime.Today;
            var monthsAsMember = (today.Year - dateJoined.Year) * 12 + today.Month - dateJoined.Month;
            return monthsAsMember;
        }
    }

}
