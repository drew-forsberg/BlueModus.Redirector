using Microsoft.AspNetCore.Builder;

namespace BlueModus.Redirector.Middleware
{
    public static class RedirectRulesMiddlewareExtensions
    {
        public static IApplicationBuilder UseRedirectRules(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RedirectRulesMiddleware>();
        }
    }
}
