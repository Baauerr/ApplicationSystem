using Exceptions.ExceptionTypes;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Exceptions
{
    public class ExceptionsHandler : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode = StatusCodes.Status500InternalServerError;

            switch (exception)
            {
                case BadRequestException:
                    statusCode = StatusCodes.Status400BadRequest;
                    break;
                case NotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    break;
                case ForbiddenException:
                    statusCode = StatusCodes.Status403Forbidden;
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    break;

            }

            var errorResponse = new ExceptionResponseModel
            {
                status = statusCode.ToString(),
                message = exception.Message
            };

            var errorResponseJson = JsonConvert.SerializeObject(errorResponse);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(errorResponseJson);
        }
    }
}
