using Domain.Entities;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Context;

public interface IApplicationDbContext
{
    DbSet<Domain.Entities.Beach> Beach { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<Cart> Carts { get; set; }
    DbSet<CartItem> CartItems { get; set; }
    DbSet<Role> Roles { get; set; }
    DbSet<Item> Items { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<Feature> Features { get; set; }
    DbSet<RolePermission> RolePermissions { get; set; }
    DbSet<Permission> Permissions { get; set; }
    DbSet<PasswordResetAttempt> PasswordResetAttempts { get; set; }
    DbSet<DiscountProfile> DiscountProfiles { get; set; }
    DbSet<Purchase> Purchases { get; set; }
    DbSet<PurchaseItem> PurchaseItems { get; set; }
    DbSet<QualificationRequirement> QualificationRequirements { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}