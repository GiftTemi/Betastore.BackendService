using Domain.Entities;
using Application.Context;
using Application.Interfaces;
using Domain.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Context;

public class ApplicationDbContext : IdentityDbContext, IApplicationDbContext
{
    private readonly IDateTime _dateTime;
    private readonly IConfiguration _configuration;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTime dateTime, IConfiguration configuration) : base(options)
    {
        _dateTime = dateTime;
        _configuration = configuration;
    }
    #region Use InMemory
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(databaseName: "Betastore.AuthServiceDb");
    }
    #endregion

    #region Using Sql Server
    //protected override void OnConfiguring(DbContextOptionsBuilder options)
    //{
    //    options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
    //}
    #endregion

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // entry.Entity.CreatedBy = _currentUserService.UserId;
                    entry.Entity.CreatedDate = _dateTime.Now;
                    break;
                case EntityState.Modified:
                    //entry.Entity.LastModifiedBy = _currentUserService.UserId;
                    entry.Entity.LastModifiedDate = _dateTime.Now;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }


    public DbSet<User> Users { get; set; }
    public DbSet<Beach> Beach { get; set; }

    public DbSet<Role> Roles { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<PasswordResetAttempt> PasswordResetAttempts { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<DiscountProfile> DiscountProfiles { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Item> Items { get; set; }
    public  DbSet<Customer> Customers { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PurchaseItem> PurchaseItems { get; set; }
    public DbSet<QualificationRequirement> QualificationRequirements { get; set; }
}