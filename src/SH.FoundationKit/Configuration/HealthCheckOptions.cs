namespace SH.FoundationKit.Configuration
{
    public class CustomHealthCheckOptions
    {
        public bool Enable { get; set; } = true;
        public SqlHealthCheckConfig Sql { get; set; } = new();
        public RedisHealthCheckConfig Redis { get; set; } = new();
        public List<UrlHealthCheckConfig> Urls { get; set; } = new();
    }

    public class SqlHealthCheckConfig
    {
        public bool Enabled { get; set; } = false;
        public string? ConnectionString { get; set; }
    }

    public class RedisHealthCheckConfig
    {
        public bool Enabled { get; set; } = false;
        public string? ConnectionString { get; set; }
    }

    public class UrlHealthCheckConfig
    {
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
    }

}
