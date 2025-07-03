using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace SH.FoundationKit.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, IHostEnvironment env, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var problem = new
                {
                    type = "https://httpstatuses.com/500",
                    title = "Internal Server Error",
                    status = 500,
                    detail = _env.IsDevelopment() ? ex.ToString() : "An unexpected error occurred.",
                    instance = context.TraceIdentifier
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
        }
    }


}
