using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace SH.FoundationKit.Resilience
{
    public static class PollyExtensions
    {
        public static IServiceCollection AddResiliencePolicies(this IServiceCollection services, IConfiguration config)
        {
            var options = config.GetSection("FoundationKit:Resilience").Get<ResilienceOptions>() ?? new();

            if (!options.Enable) return services;

            services.AddHttpClient("ResilientClient")
                .AddPolicyHandler(GetRetryPolicyWithMethodFilter(options))
                .AddPolicyHandler(GetCircuitBreakerPolicy(options))
                .AddPolicyHandler(GetTimeoutPolicy(options));

            return services;
        }


        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicyWithMethodFilter(ResilienceOptions options)
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(r => (int)r.StatusCode >= 500)
                .RetryAsync(options.RetryCount, onRetry: (outcome, attempt, context) =>
                {
                    // Optional logging hook
                });
        }
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ResilienceOptions options) =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: options.CircuitBreakerFailureThreshold,
                durationOfBreak: TimeSpan.FromSeconds(options.CircuitBreakerDurationSeconds));

        public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(ResilienceOptions options) =>
            Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(options.TimeoutSeconds));


        //public static IAsyncPolicy<HttpResponseMessage> GetRetryOnlyForSafeMethods(ResilienceOptions options)
        //{
        //    return Policy<HttpResponseMessage>
        //        .Handle<HttpRequestException>()
        //        .OrResult(r => (int)r.StatusCode >= 500)                 
        //        .Where((response, context) =>
        //        {
        //            var method = context["HttpMethod"]?.ToString();
        //            return method == "GET" || (method == "POST" && context.ContainsKey("Idempotency-Key"));
        //        })
        //        .RetryAsync(options.RetryCount);
        //}

    }

    //public class AddHttpMethodToPollyContextHandler : DelegatingHandler
    //{
    //    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //    {
    //        request.SetPolicyExecutionContext(new Context
    //        {
    //            ["HttpMethod"] = request.Method.Method,
    //            ["IdempotencyKey"] = request.Headers.Contains("Idempotency-Key") ? "true" : "false"
    //        });

    //        return base.SendAsync(request, cancellationToken);
    //    }
    //}
}
