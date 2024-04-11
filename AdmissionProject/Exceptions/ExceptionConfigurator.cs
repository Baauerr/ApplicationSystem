using Microsoft.AspNetCore.Builder;

namespace Exceptions
{
    public static class ExceptionMiddlewareExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionsHandler>();
        }
    }
}