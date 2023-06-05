using Domain.Entities;
using Application.Context;
using Application.Interfaces;
using Application.Users.Commands;
using Infrastructure.Authentication;
using Infrastructure.Context;
using Infrastructure.Identity;
using Infrastructure.Services;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void ConfigureInfraStructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<ApplicationDbContext>();

        services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

        services.AddDefaultIdentity<User>().AddRoles<ApplicationRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IIdentityService, IdentityService>();




        services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(configuration["TokenConstants:key"])),
                ValidIssuer = configuration["Token:issuer"],
                ValidAudience = configuration["Token:aud"]
            };
            options.Authority = configuration["Token:issuer"];
            options.SaveToken = true;
            options.Audience = configuration["Token:aud"];
            options.RequireHttpsMetadata = false;
            options.Configuration = new OpenIdConnectConfiguration();
        });

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IAuthenticateService, AuthenticateService>();
        services.AddTransient<IPasswordService, PasswordService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IStringHashingService, StringHashingService>();
        services.AddTransient<IBase64ToFileConverter, Base64ToFileConverter>();
        services.AddTransient<IApplicationSeed, ApplicationSeed>();

        // Seed the data
        using (var scope = services.BuildServiceProvider().CreateScope())
        {
            var seed = scope.ServiceProvider.GetRequiredService<IApplicationSeed>();
            seed.SeedData();
        }
    }

}