
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SH.FoundationKit.Configuration;
using SH.FoundationKit.HealthCheck;
using SH.FoundationKit.Logging;
using SH.FoundationKit.Middleware;
using SH.FoundationKit.Swagger;

namespace SH.FoundationKit
{
    public static class DependecyInjetion
    {
        public static IServiceCollection AddFoundationKit(this IServiceCollection services, IConfiguration config)
        {
            var options = config.GetSection("FoundationKit").Get<FoundationKitOptions>() ?? new();

            if (options.EnableLogging) services.AddLoggingExtension(config);
            if (options.EnableHealthChecks) services.AddHealthChecks(config);
            //if (options.EnableAuthentication) services.AddFoundationAuth(config);
            //if (options.EnableResilience) services.AddResiliencePolicies(config);
            //if (options.EnableCaching) services.AddCaching(config);
            //if (options.EnableLocalization) services.AddLocalizationSupport();

            //if (options.EnableRateLimiting) services.AddRateLimiting(config);
            services.Configure<SwaggerOptions>(config.GetSection("Swagger"));
            if (options.EnableSwagger) services.AddSwaggerSetup(config);
            //if (options.EnableValidation) services.AddValidationPipeline();
            //if (options.EnableTimeZones) services.AddTimeZoneSupport();
            //if (options.EnableProblemDetails) services.AddProblemDetails();
            //if (options.EnableApiResultWrapper) services.AddApiResultWrapper();
            //if (options.EnableCorrelationId) services.AddCorrelationId();
            //if (options.EnableSecurityHeaders) services.AddSecurityHeaders(config);
            //if (options.EnableBenchmarking) services.AddBenchmarking();
            services.AddMemoryCache();
            return services;
        }


        public static IApplicationBuilder UseFoundationKit(this IApplicationBuilder app, IConfiguration config)
        {

            var options = config.GetSection("FoundationKit").Get<FoundationKitOptions>() ?? new();

            if (options.EnableHealthChecks) app.UseHealthCheckEndpoints();
            if (options.EnableLogging) app.UseRequestResponseLogging();
            if (options.EnableGlobalExceptionHandling) app.UseGlobalExceptionHandler();

            if (options.EnableSwagger)
            {
                //app.UseFoundationKitStaticFiles();
                app.UseSwaggerUISetup(config);
            }

            //if (options.EnableExceptionHandling)
            //    app.UseGlobalExceptionHandler();

            //if (options.EnableCorrelationId)
            //    app.UseMiddleware<CorrelationIdMiddleware>();

            //if (options.EnableLogging)
            //    app.UseRequestResponseLogging();

            //if (options.EnableHealthChecks)
            //    app.UseHealthCheckEndpoints();

            // ...add others (Swagger, TimeZoneMiddleware, etc.)


            app.UseIdempotencyMiddleware();
            return app;
        }

    }

    public static class MiddlewareExtensions
    {

        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
  => app.UseMiddleware<ExceptionHandlingMiddleware>();

        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggingMiddleware>();
        }



        public static IApplicationBuilder UseIdempotencyMiddleware(this IApplicationBuilder app)
                            => app.UseMiddleware<IdempotencyMiddleware>();

        //public static IApplicationBuilder UseFoundationKitStaticFiles(this IApplicationBuilder app)
        //{
        //    var assembly = typeof(MiddlewareExtensions).Assembly;
        //    var provider = new ManifestEmbeddedFileProvider(assembly, "wwwroot");

        //    app.UseStaticFiles(new StaticFileOptions
        //    {
        //        FileProvider = provider,
        //        RequestPath = ""
        //    });

        //    return app;
        //}
    }
}
