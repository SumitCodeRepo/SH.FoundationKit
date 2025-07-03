using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;

namespace SH.FoundationKit.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;
        private readonly string _appName;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _appName = env.ApplicationName;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var request = context.Request;
            var traceId = context.TraceIdentifier;
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["Application"] = _appName,
                ["TraceId"] = traceId,
                ["User"] = userId,
                ["RequestMethod"] = request.Method,
                ["RequestPath"] = request.Path
            }))
            {
                await _next(context);
                stopwatch.Stop();

                _logger.LogInformation(
                    "Request {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                    request.Method,
                    request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds
                );
            }
        }
        //public async Task InvokeAsync(HttpContext context)

        //{

        //    var stopwatch = Stopwatch.StartNew();

        //    var request = context.Request;
        //    var traceId = context.TraceIdentifier;
        //    var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        //    using (_logger.BeginScope(new Dictionary<string, object>
        //    {
        //        ["Application"] = "MyApp",
        //        ["TraceId"] = traceId,
        //        ["User"] = userId,
        //        ["RequestMethod"] = request.Method,
        //        ["RequestPath"] = request.Path
        //    }))
        //    var originalBodyStream = context.Response.Body;
        //    using var responseBody = new MemoryStream();
        //    context.Response.Body = responseBody;


        //    var request = context.Request;
        //    _logger.LogInformation("Request {method} {url}", request.Method, request.Path);

        //    var originalBodyStream = context.Response.Body;
        //    using var responseBody = new MemoryStream();
        //    context.Response.Body = responseBody;

        //    await _next(context);

        //    context.Response.Body.Seek(0, SeekOrigin.Begin);
        //    var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        //    context.Response.Body.Seek(0, SeekOrigin.Begin);

        //    _logger.LogInformation("Response {statusCode}: {body}", context.Response.StatusCode, responseText);

        //    await responseBody.CopyToAsync(originalBodyStream);
        //}
    }


}
