using Application.Context;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Infrastructure.Context;

public class ApplicationSeed : IApplicationSeed
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly ILogger<ApplicationSeed> _logger;

    public ApplicationSeed(IApplicationDbContext context, IIdentityService identityService, ILogger<ApplicationSeed> logger)
    {
        _context = context;
        _identityService = identityService;
        _logger = logger;
    }

    public void SeedData()
    {
        try
        {
            _logger.LogInformation("About to start data seeding");

            // Check if the data has already been seeded
            if (_context.Roles.Any() && _context.Permissions.Any())
            {
                Console.WriteLine("Data has already been seeded. Skipping data seeding process.");
                return;
            }
            // Perform your data seeding logic here
            SeedPermissions();
            SeedRoles();
            SeedRolePermissions();
            SeedItems();
            CreateDiscountProfiles();
            // Save changes to the database
            _context.SaveChangesAsync(new CancellationToken());

            _ = SeedAdminUser();
            _ = PerformUserCreationAndPurchase();

            _logger.LogInformation("Data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            // Log and handle the exception
            _logger.LogError($"Error seeding data: {ex.Message}");
        }
    }

    private void SeedPermissions()
    {
        var permissions = typeof(DefaultPermissions)
            .GetNestedTypes(BindingFlags.Public)
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            .Where(f => f.FieldType == typeof(string))
            .Select(f => new Permission { Name = f.GetValue(null)?.ToString() })
            .ToList();

        _context.Permissions.AddRange(permissions);
    }

    private void SeedRoles()
    {
        var roles = typeof(DefaultRoles)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(string))
            .Select(f => new Role { Name = f.GetValue(null)?.ToString() })
            .ToList();

        _context.Roles.AddRange(roles);
    }

    private void SeedRolePermissions()
    {
        var roles = _context.Roles.ToList();
        var permissions = _context.Permissions.ToList();

        // Assign role permissions as needed

        // Example: Assign all permissions to the SuperAdmin role
        var superAdminRole = roles.FirstOrDefault(r => r.Name == DefaultRoles.SuperAdmin);
        var rolePermissions = permissions.Select(permission => new RolePermission
        {
            Role = superAdminRole,
            Permission = permission
        });

        _context.RolePermissions.AddRange(rolePermissions);

        // Example: Assign appropriate permissions to other roles

        // Assign permissions to the Admin role
        var adminRole = roles.FirstOrDefault(r => r.Name == DefaultRoles.Admin);
        var adminPermissions = permissions.Where(p =>
            p.Name.StartsWith("Permissions.Product") ||
            p.Name.StartsWith("Permissions.Order") ||
            p.Name.StartsWith("Permissions.Payment") ||
            p.Name.StartsWith("Permissions.Shipping") ||
            p.Name.StartsWith("Permissions.Reporting") ||
            p.Name.StartsWith("Permissions.Support")
        ).ToList();
        var adminRolePermissions = adminPermissions.Select(permission => new RolePermission
        {
            Role = adminRole,
            Permission = permission
        });

        _context.RolePermissions.AddRange(adminRolePermissions);

        // Assign permissions to the CustomerSupport role
        var customerSupportRole = roles.FirstOrDefault(r => r.Name == DefaultRoles.CustomerSupport);
        var customerSupportPermissions = permissions.Where(p =>
            p.Name.StartsWith("Permissions.Customer") ||
            p.Name.StartsWith("Permissions.Support")
        ).ToList();
        var customerSupportRolePermissions = customerSupportPermissions.Select(permission => new RolePermission
        {
            Role = customerSupportRole,
            Permission = permission
        });

        _context.RolePermissions.AddRange(customerSupportRolePermissions);

        // Assign permissions to the Customer role
        var customerRole = roles.FirstOrDefault(r => r.Name == DefaultRoles.Customer);
        var customerPermissions = permissions.Where(p =>
            p.Name.StartsWith("Permissions.Customer")
        ).ToList();
        var customerRolePermissions = customerPermissions.Select(permission => new RolePermission
        {
            Role = customerRole,
            Permission = permission
        });

        _context.RolePermissions.AddRange(customerRolePermissions);
    }
    //CreateUserCommand
    private async Task SeedAdminUser()
    {
        // Create the admin user
        var adminUser = new User
        {
            UserName = "Admin", // Replace with the desired username
            Email = "admin@yopmail.com", // Replace with the desired email
            FirstName = "Admin",
            LastName = "User",
            DateOfBirth = DateTime.UtcNow.Date,
            LastLoginDate = DateTime.UtcNow,
            AccessLevel = AccessLevel.SystemAdmin,
            CreatedDate = DateTime.UtcNow,
            Status = Status.Active,
            Password = "PasswordAdmin12$",
        };

        // Use the identity service to create the admin user
        var result = await _identityService.CreateUserAsync(adminUser); // Replace with the desired password
        if (result.Result.Succeeded)
        {
            // Assign the admin role to the admin user
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == DefaultRoles.Admin);
            adminUser.Role = adminRole;

            // Save changes to the database
            await _context.SaveChangesAsync(new CancellationToken());
        }
        else
        {
            Console.WriteLine("Failed to create admin user.");
        }
    }

    private async Task PerformUserCreationAndPurchase()
    {
        //CreateUserCommand
        #region Create Customer
        // Create the admin user
        var customerUser = new User
        {
            UserName = "TomiL",
            Email = "tomi@yopmail.com",
            FirstName = "Tomi",
            LastName = "Layo",
            MiddleName = "Asabi",
            DateOfBirth = DateTime.UtcNow.Date,
            LastLoginDate = DateTime.UtcNow,
            AccessLevel = AccessLevel.Customer,
            CreatedDate = DateTime.UtcNow,
            Status = Status.Active,
            Password = "TomiLayoAsa12$",
        };

        // Use the identity service to create the admin user
        var result = await _identityService.CreateUserAsync(customerUser); // Replace with the desired password
        if (result.Result.Succeeded)
        {
            if (customerUser.AccessLevel == AccessLevel.Customer)
            {
                var newCustomer = new Domain.Entities.Customer
                {
                    UserId = result.UserId,
                    CreatedBy = "SYSTEM",
                    CreatedDate = DateTime.Now,
                };
                await _context.Customers.AddAsync(newCustomer);
                // Save changes to the database
                await _context.SaveChangesAsync(new CancellationToken());
            }
        }
        else
        {
            Console.WriteLine("Failed to create customer.");
        }
        #endregion

        //AddToCartCommand
        #region Add Items to Cart and Purchase
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == customerUser.Id);
        if (customer != null)
        {
            #region Add Items to Cart
            // Retrieve the item from the database
            var items = await _context.Items.ToListAsync();
            if (items != null && items.Count > 0)
            {
                // Retrieve the user's cart from the database
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.CustomerId == customer.Id);

                if (cart == null)
                {
                    // Create a new cart for the user if it doesn't exist
                    cart = new Cart
                    {
                        CustomerId = customer.Id,
                        //CartItems = new List<CartItem>(),
                        CartItems = items.Select(item => new CartItem
                        {
                            ItemId = item.Id,
                            Item = item,
                            Quantity = new Random().Next(1, 11)
                        }).ToList()
                    };

                    await _context.Carts.AddAsync(cart);

                    //CreatePurchaseCommand
                    #region Purchase Items in the Cart 
                    var purchase = new Purchase
                    {
                        CustomerId = cart.CustomerId,
                        UserId = customer.UserId,
                        PurchaseDate = DateTime.UtcNow,
                        PurchaseItems = cart.CartItems.Select(ci => new PurchaseItem
                        {
                            ItemId = ci.ItemId,
                            Quantity = ci.Quantity,
                            Price = ci.Item.Price
                        }).ToList()
                    };


                    var discountProfile = await _context.DiscountProfiles.FirstOrDefaultAsync();

                    if (discountProfile != null && discountProfile.Status == Domain.Enums.Status.Active)
                    {
                        purchase.DiscountAmount = (discountProfile.DiscountPercentage / 100) * purchase.OriginalAmount;
                        purchase.DiscountApplied = true;
                    }

                    // Add the purchase to the database
                    _context.Purchases.Add(purchase);
                    #endregion

                    // Save changes to the database
                    await _context.SaveChangesAsync(new CancellationToken());
                }
            }

            #endregion
        }
        #endregion
    }
    //CreateItemCommand
    private void SeedItems()
    {
        // Create a list of items
        //{{baseUrl}}/api/Item/create
        /// 
        var items = new List<Item>
        {
            new Item
            {
                Name="Bag",
                Description = "Item 1 Description",
                Price = 10.99,
                StockQuantity = 5,
                CreatedBy = "SYSTEM",
                CreatedById = "SYSTEM",
                CreatedDate = DateTime.Now
            },
            new Item
            {
                Name="Shoe",
                Description = "Item 2 Description",
                Price = 19.99,
                StockQuantity = 3,
                CreatedBy = "SYSTEM",
                CreatedById = "SYSTEM",
                CreatedDate = DateTime.Now
            },
            new Item
            {
                Name="Phone",
                Description = "Item 3 Description",
                Price = 7.99,
                StockQuantity = 8,
                CreatedBy = "SYSTEM",
                CreatedById = "SYSTEM",
                CreatedDate = DateTime.Now
            },
            new Item
            {
                Name = "Laptop",
                Description = "Item 4 Description",
                Price = 12.5,
                StockQuantity = 2,
                CreatedBy = "SYSTEM",
                CreatedById = "SYSTEM",
                CreatedDate = DateTime.Now
            },
            new Item
            {
                Name="Ring",
                Description = "Item 5 Description",
                Price = 14.9,
                StockQuantity = 6,
                CreatedBy = "SYSTEM",
                CreatedById = "SYSTEM",
                CreatedDate = DateTime.Now
            }
        };

        // Save the items to the database
        _context.Items.AddRange(items);
    }
    //CreateDiscountProfileCommand
    private void CreateDiscountProfiles()
    {
        var items = new List<DiscountProfile>
        {
            new DiscountProfile
            {
                Name = "DIS 1",
                DiscountType = DiscountType.AmountSpent,
                QualificationRequirement = new QualificationRequirement
                {
                    MinimumAmountSpent = 10
                },
                DiscountPercentage = 20,
                CreatedDate = DateTime.Now,
                Status = Status.Active,
                CreatedBy = "SYSTEM"
            },
            new DiscountProfile
            {
                Name = "DIS 2",
                DiscountType = DiscountType.MembershipDuration,
                QualificationRequirement = new QualificationRequirement
                {
                    MinimumMonthsAsMember = 3
                },
                DiscountPercentage = 5,
                CreatedDate = DateTime.Now,
                Status = Status.Active,
                CreatedBy = "SYSTEM"
            },
            new DiscountProfile
            {
                Name = "DIS 3",
                DiscountType = DiscountType.AmountSpent,
                QualificationRequirement = new QualificationRequirement
                {
                    MinimumAmountSpent = 5
                },
                DiscountPercentage = 10,
                CreatedDate = DateTime.Now,
                Status = Status.Active,
                CreatedBy = "SYSTEM"
            },
            new DiscountProfile
            {
                Name = "DIS 4",
                DiscountType = DiscountType.MembershipDuration,
                QualificationRequirement = new QualificationRequirement
                {
                    MinimumMonthsAsMember = 1
                },
                DiscountPercentage = 2.5,
                CreatedDate = DateTime.Now,
                Status = Status.Active,
                CreatedBy = "SYSTEM"
            }
        };

        // Save the items to the database
        _context.DiscountProfiles.AddRange(items);
    }
}
