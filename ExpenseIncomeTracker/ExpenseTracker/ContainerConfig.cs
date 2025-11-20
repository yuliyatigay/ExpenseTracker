using System.Security.Claims;
using System.Text;
using BusinessLogic.Services;
using BusinessLogic.Utilities;
using DataAccess.Data;
using DataAccess.Repository;
using Domain.Models;
using Domain.RepositoryInterfaces;
using Domain.ServiceInterfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace IncomeExpenseTracker;

public static class ContainerConfig
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetConnectionString("DefaultConnection");
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(config));
        services.AddSingleton(_ => new DataBaseInitializer(config)); 
        services.Configure<AuthSettings>(configuration.GetSection("AuthSettings"));
        
        var authConfig = configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.SecretKey)),
                    RoleClaimType = ClaimTypes.Role
                };
            });
        services.AddScoped<IValidator<Account>, PasswordValidator>();
        services.AddScoped<IOperationRepository, OperationRepository>();
        services.AddScoped<IOperationService, OperationService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IJwtService ,JwtService>();
    }
}