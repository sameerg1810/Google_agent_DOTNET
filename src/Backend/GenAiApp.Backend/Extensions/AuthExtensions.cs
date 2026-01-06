using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GenAiApp.Backend.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddGoogleIdentityAuth(this IServiceCollection services, IConfiguration config)
    {
        // 1. Get your Project ID from appsettings.json
        var projectId = config["GoogleCloud:ProjectId"];

        // 2. Register Authentication and JWT Bearer service
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // The Authority is the Google Discovery Document for your project
                options.Authority = $"https://securetoken.google.com/{projectId}";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{projectId}",
                    ValidateAudience = true,
                    ValidAudience = projectId, // Validates the token was made for your app
                    ValidateLifetime = true     // Rejects expired tokens
                };
            });

        // 3. Register Authorization services
        services.AddAuthorization();

        return services;
    }
}