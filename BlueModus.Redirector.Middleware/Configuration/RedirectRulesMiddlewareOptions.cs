namespace BlueModus.Redirector.Middleware.Configuration
{
    public class RedirectRulesMiddlewareOptions
    {
        public const string RedirectRulesMiddleware = "RedirectRulesMiddleware";

        public int CacheDurationSeconds { get; set; } = 60 * 5; // default to 5 min
        public int CacheRefreshIntervalSeconds { get; set; } = 60; // default to 1 min
    }
}
