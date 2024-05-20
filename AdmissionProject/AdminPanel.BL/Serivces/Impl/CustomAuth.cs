using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AdminPanel.BL.Serivces.Impl
{
    public static class AuthWithCookieToken
    {
        public static void configureAuthWithCookieToken(this WebApplicationBuilder builder)
        {

            var key = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");

            var issuer = builder.Configuration.GetValue<string>("ApiSettings:Issuer");

            var audience = builder.Configuration.GetValue<string>("ApiSettings:Audience");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
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
                    options.Events = new JwtBearerEvents()
                    {
                        OnTokenValidated = context =>
                        {
                            var token = context.SecurityToken as JwtSecurityToken;
                            if (token != null)
                            {
                                context.Request.Headers["Authorization"] = "Bearer " + token.RawData;
                            }
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            var token = context.HttpContext.Request.Cookies["AccessToken"];
                            context.Token = token;
                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}
