using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace SH.FoundationKit.Middleware
{
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<IdempotencyMiddleware> _logger;

        public IdempotencyMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<IdempotencyMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;

            // Only apply to POST/PUT
            if (method != HttpMethods.Post && method != HttpMethods.Put)
            {
                await _next(context);
                return;
            }

            var key = await GenerateIdempotencyKeyAsync(context);

            if (_cache.TryGetValue(key, out var cachedResponse))
            {
                _logger.LogInformation("Duplicate request detected, returning cached response for key {Key}", key);
                context.Response.Headers.Append("Duplicate", "Request already processed");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync((string)cachedResponse!);
                return;
            }

            var originalBodyStream = context.Response.Body;
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _next(context);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);

            _cache.Set(key, responseBody, TimeSpan.FromMinutes(5)); // configurable TTL

            await memoryStream.CopyToAsync(originalBodyStream);
        }

        private async Task<string> GenerateIdempotencyKeyAsync(HttpContext context)
        {
            var method = context.Request.Method;
            var pathAndQuery = context.Request.Path + context.Request.QueryString;

            context.Request.EnableBuffering();

            string body;
            using (var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            var rawKey = $"{method}|{pathAndQuery}|{body}";
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(rawKey));
            return Convert.ToBase64String(hash);
        }
    }
}
