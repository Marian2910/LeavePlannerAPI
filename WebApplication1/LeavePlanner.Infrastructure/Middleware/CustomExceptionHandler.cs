using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using LeavePlanner.Infrastructure.Exceptions;

namespace LeavePlanner.Infrastructure.Middleware
{
    public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger, RequestDelegate next)
    {
        private const string JsonContentType = "application/json";
        private const string ForbiddenText = "forbidden";
        private const string GenericErrorMessage = "Sorry, it’s not you—it’s us. Our server is having some trouble. Hang tight!";

        public async Task InvokeAsync(HttpContext context)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = JsonContentType;
            string message;
            switch (ex)
            {
                case NotNullEntityException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;
                case NullEntityException:
                case LessThanZeroNumbersException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    message = ex.Message;
                    break;
                default:
                    if (ex.Message.Contains(ForbiddenText, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        message = ex.Message;
                        break;
                    }
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    message = GenericErrorMessage;
                    break;
            }
            logger.LogError(ex, "Unhandled exception while processing request.");
            var result = JsonSerializer.Serialize(new { errorMessage = message });
            await context.Response.WriteAsync(result);
        }
    }
}
