using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SH.FoundationKit.Configuration;
using System.Text.Json;

namespace SH.FoundationKit.HealthCheck
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration config)
        {
            var hcConfig = config.GetSection("HealthChecks").Get<CustomHealthCheckOptions>();
            if (hcConfig == null || !hcConfig.Enable) return services;

            var hc = services.AddHealthChecks();

            if (hcConfig.Sql.Enabled && !string.IsNullOrWhiteSpace(hcConfig.Sql.ConnectionString))
            {
                hc.AddSqlServer(hcConfig.Sql.ConnectionString, name: "SQL Server");
            }

            if (hcConfig.Redis.Enabled && !string.IsNullOrWhiteSpace(hcConfig.Redis.ConnectionString))
            {
                hc.AddRedis(hcConfig.Redis.ConnectionString, name: "Redis");
            }

            foreach (var url in hcConfig.Urls)
            {
                if (!string.IsNullOrWhiteSpace(url.Url))
                {
                    hc.AddUrlGroup(new Uri(url.Url), name: url.Name);
                }
            }

            return services;
        }

        public static IApplicationBuilder UseHealthCheckEndpoints(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            duration = e.Value.Duration.TotalMilliseconds
                        })
                    });

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(result);
                }
            });

            return app;
        }
    }
}
