using Common.Helpers;
using Common.Helpers.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Configuration
{
    public static class HelpersConfiguration
    {
        public static void ConfigureHelpersServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped
                <ITokenHelper, TokenHelper>();
        }
    }
}
