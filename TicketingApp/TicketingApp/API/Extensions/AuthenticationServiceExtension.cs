using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TicketingApp.Infrastructure.Settings;

namespace TicketingApp.API.Extensions
{
    public static class AuthenticationServiceExtension
    {
        public static void RegisterAuthentication(this WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;
            var services = builder.Services;

            var jwtSettings = new JwtSettings();
            configuration.Bind("JwtSettings", jwtSettings);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = key
                };
                options.Events = new JwtBearerEvents



                {
                    OnAuthenticationFailed = ctx =>
                    {
                        Console.WriteLine("Authentication failed: " + ctx.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = ctx =>
                    {
                        Console.WriteLine("Token validated for user: " + ctx.Principal.Identity.Name);
                        return Task.CompletedTask;
                    }
                };
    



            });
        }
    }
}
