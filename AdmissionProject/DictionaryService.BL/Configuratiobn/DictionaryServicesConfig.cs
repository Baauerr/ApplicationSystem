using DictionaryService.BL.Mapper;
using DictionaryService.BL.Services;
using DictionaryService.Common.Interfaces;
using Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using UserService.Controllers.Policies.HITSBackEnd.Controllers.AttributeUsage;

public static class DictionaryServiceConfig
{
    public static void ConfigureDictionaryServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped
            <IDictionaryInfoService, DictionaryInfoService>();
        builder.Services.AddScoped
            <IImportService, ImportService>();
        builder.Services.AddAutoMapper(typeof(DictionaryMapper));
        builder.Services.AddTransient<ExceptionsHandler>();
        builder.Services.AddScoped<TokenBlacklistFilterAttribute>();
    }
}