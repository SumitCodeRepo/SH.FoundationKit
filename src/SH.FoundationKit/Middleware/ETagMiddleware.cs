using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SH.FoundationKit.Middleware
{


    public class EnableETagFilter : IAsyncActionFilter
    {

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next();

            // Only operate on 200 OK results with a response body
            if (executedContext.Result is ObjectResult objectResult &&
                objectResult.StatusCode is null or 200 &&
                objectResult.Value is not null)
            {
                var request = context.HttpContext.Request;
                var response = context.HttpContext.Response;

                var etag = ETagHelper.GenerateETag(request, objectResult.Value);
                var clientETag = request.Headers["If-None-Match"].FirstOrDefault();

                if (clientETag == etag)
                {
                    executedContext.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
                    return;
                }
                response.Headers["Cache-Control"] = "public, max-age=60, must-revalidate";
                response.Headers["ETag"] = etag;
            }
        }

    }


    public static class ETagHelper
    {
        public static string GenerateETag(HttpRequest request, object data)
        {
            var path = $"{request.Method}:{request.Path}{request.QueryString}";

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            var combined = $"{path}|{json}";

            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
            return $"\"{Convert.ToBase64String(hash)}\"";
        }
    }




}
