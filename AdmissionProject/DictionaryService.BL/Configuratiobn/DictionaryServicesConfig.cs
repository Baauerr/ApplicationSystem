using DictionaryService.BL.Services;
using DictionaryService.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public static class DictionaryServiceConfig
{
    public static void ConfigureDictionaryServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped
            <IDictionaryInfoService, DictionaryInfoService>();
        builder.Services.AddScoped
            <IImportService, ImportService>();
    //    builder.Services.AddScoped<UserRepository>();
    //    builder.Services.AddAutoMapper(typeof(UserMapper));
   //     builder.Services.AddTransient<ExceptionsHandler>();
    }
}