namespace SH.FoundationKit.Resilience
{
    public class ResilienceOptions
    {
        public bool Enable { get; set; } = true;
        public int RetryCount { get; set; } = 3;
        public int CircuitBreakerFailureThreshold { get; set; } = 5;
        public int CircuitBreakerDurationSeconds { get; set; } = 30;
        public int TimeoutSeconds { get; set; } = 10;
    }
}
