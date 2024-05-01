using Common.Helpers;
using Common.Helpers.Impl;
using DocumentService.BL.Mapper;
using DocumentService.BL.Services;
using DocumentService.Common.Interface;
using DocumentService.BL.Services;

using Exceptions;
using UserService.Controllers.Policies.HITSBackEnd.Controllers.AttributeUsage;

namespace DocumentService.Configuration
{
    public static class ServiceConfig
    {
        public static void ConfigureDocumentServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IDocumentFormService, DocumentFormService>();
            builder.Services.AddScoped<IFileService, FilesService>();
            builder.Services.AddScoped<IRequestService, RequestService>();
            builder.Services.AddSingleton<ITokenHelper, TokenHelper>();
            builder.Services.AddTransient<ExceptionsHandler>();
            builder.Services.AddAutoMapper(typeof(DocumentMapper));
            builder.Services.AddScoped<TokenBlacklistFilterAttribute>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
