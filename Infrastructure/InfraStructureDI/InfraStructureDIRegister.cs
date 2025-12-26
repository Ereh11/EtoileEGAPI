using Domain.Configurations;
using Domain.Entities;
using Domain.Interfaces.JwtTokenGenerator;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.UnitOfWork;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.InfraStructureDI;

public static class InfraStructureDIRegister
{
    public static void AddInfraStructureDIRegister(this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddDbContext<EtolieEGDbContext>(options => options.UseSqlServer(Config.Read_DefaultConnection));
        services.AddTransient<UnitOfWork>();
        #region Repositories Registration
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        #endregion
    }
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettingsSection = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSettingsSection);

        var jwtSettings = jwtSettingsSection.Get<JwtSettings>() ??
            throw new InvalidOperationException("JWT settings are not configured");

        var secretKey = Encoding.UTF8.GetBytes(jwtSettings.Secret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateIssuer = false,
                ValidateAudience = false,
                //ValidIssuer = jwtSettings.Issuer,
                //ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        return services;
    }
}
