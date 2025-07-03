namespace SH.FoundationKit.Configuration
{
    public class FoundationKitOptions
    {
        public bool EnableLogging { get; set; } = true;
        public bool EnableHealthChecks { get; set; } = true;
        public bool EnableGlobalExceptionHandling { get; set; } = true;
        public bool EnableAuthentication { get; set; } = true;
        public bool EnableResilience { get; set; } = true;
        public bool EnableCaching { get; set; } = true;
        public bool EnableLocalization { get; set; } = true;

        public bool EnableRateLimiting { get; set; } = true;
        public bool EnableSwagger { get; set; } = true;
        public bool EnableValidation { get; set; } = true;
        public bool EnableTimeZones { get; set; } = true;
        public bool EnableProblemDetails { get; set; } = true;
        public bool EnableApiResultWrapper { get; set; } = true;
        public bool EnableCorrelationId { get; set; } = true;
        public bool EnableSecurityHeaders { get; set; } = true;
        public bool EnableBenchmarking { get; set; } = true;
    }
}
