using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Infrastructure.Data.Context;
using GestaoRestaurante.API.Authorization;

namespace GestaoRestaurante.API.Extensions;

public static class IdentityConfiguration
{
    public static IServiceCollection AddIdentityAndJwt(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar Identity
        services.AddIdentity<Usuario, IdentityRole<Guid>>(options =>
        {
            // Configurações de senha
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // Configurações de usuário
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

            // Configurações de bloqueio
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // Configurações de confirmação de email (desabilitadas para simplificar)
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<GestaoRestauranteContext>()
        .AddDefaultTokenProviders();

        // Configurar JWT Authentication
        var jwtSettings = configuration.GetSection("JWT");
        var secretKey = jwtSettings["SecretKey"] ?? "MinhaChaveSecretaSuperSegura123456789";
        var issuer = jwtSettings["Issuer"] ?? "GestaoRestaurante";
        var audience = jwtSettings["Audience"] ?? "GestaoRestauranteAPI";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // Para desenvolvimento
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        // Configurar Authorization com policies de módulos
        services.AddAuthorization();
        services.AddSingleton<IAuthorizationPolicyProvider, ModuleAuthorizationPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, ModuleAuthorizationHandler>();

        return services;
    }
}