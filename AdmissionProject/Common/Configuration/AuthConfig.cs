using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace Common.Configuration
{
    public static class ConfigureJWT
    {
        public static void configureJWTAuth(this WebApplicationBuilder builder)
        {

            var key = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");

            var issuer = builder.Configuration.GetValue<string>("ApiSettings:Issuer");

            var audience = builder.Configuration.GetValue<string>("ApiSettings:Audience");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidIssuer = issuer,
                        ValidateAudience = false,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(key ?? string.Empty)),
                        ValidateIssuerSigningKey = true,
                    };
                });
        }
    }
}
