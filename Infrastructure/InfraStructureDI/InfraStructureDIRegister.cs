using Domain.Configurations;
using Domain.Entities;
using Domain.Interfaces.JwtTokenGenerator;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.UnitOfWork;
using Infrastructure.Security;
using Infrastructure.Security.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.InfraStructureDI;

public static class InfraStructureDIRegister
{
    public static void AddInfraStructureDIRegister(this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddDbContext<EtolieEGDbContext>(options => options.UseSqlServer(Config.Read_DefaultConnection));
        services.AddTransient<UnitOfWork>();
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<EtolieEGDbContext>()
        .AddDefaultTokenProviders();
        #region Repositories Registration
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddJwtAuthentication(configuration);
        #endregion
    }
    public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services
            .AddOptions<JwtSettings>()
            .Bind(configuration.GetSection("JwtSettings"))
            .Validate(s =>
                !string.IsNullOrWhiteSpace(s.Secret) &&
                s.Secret.Length >= 32,
                "JWT Secret must be at least 32 characters")
            .ValidateOnStart();

        var jwtSettings = configuration
            .GetSection("JwtSettings")
            .Get<JwtSettings>()!;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret)),

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;

    }
}
