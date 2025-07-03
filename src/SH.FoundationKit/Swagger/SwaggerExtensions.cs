using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SH.FoundationKit.Swagger
{
    public class SwaggerOptions
    {
        public bool Enable { get; set; } = true;
        public string Title { get; set; } = "SH.FoundationKit API";
        public string Version { get; set; } = "v1";
        public List<SwaggerHeaderOption> Headers { get; set; } = new();

    }

    public class SwaggerHeaderOption
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Default { get; set; } = "";
    }

    public class AddCustomHeadersOperationFilter : IOperationFilter
    {
        private readonly SwaggerOptions _options;

        public AddCustomHeadersOperationFilter(IOptions<SwaggerOptions> options)
        {
            _options = options.Value;
        }
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();


            foreach (var header in _options.Headers)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = header.Name,
                    In = ParameterLocation.Header,
                    Required = false,
                    Description = header.Description,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Default = new OpenApiString(header.Default)
                    }
                });
            }
        }
    }
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerSetup(this IServiceCollection services, IConfiguration config)
        {
            var options = config.GetSection("Swagger").Get<SwaggerOptions>() ?? new();

            if (!options.Enable) return services;

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(options.Version, new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = options.Title,
                    Version = options.Version
                });

                c.OperationFilter<AddCustomHeadersOperationFilter>();

                // Optional: Include XML comments, auth headers, etc.
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerUISetup(this IApplicationBuilder app, IConfiguration config)
        {
            var options = config.GetSection("Swagger").Get<SwaggerOptions>() ?? new();

            if (!options.Enable) return app;

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{options.Version}/swagger.json", options.Title);
                //c.InjectJavascript("/swagger-custom.js");
            });

            return app;
        }
    }
}
