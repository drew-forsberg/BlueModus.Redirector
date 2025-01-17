using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BlueModus.Redirector.Middleware
{
    public class RedirectRulesMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectRulesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRedirectItemService redirectItemService,
            IRedirectItemComparer redirectItemComparer, ILogger<RedirectRulesMiddleware> logger)
        {
            var redirectItems = await redirectItemService.GetRedirectItemsAsync(CancellationToken.None);

            if (redirectItems.Any())
            {
                var requestUrl = context.Request.Path.Value;

                var redirectResult = redirectItemComparer.GetMatchingRedirectItem(requestUrl, redirectItems);

                if (redirectResult != null)
                {
                    logger.LogInformation("Redirect match found! Redirecting from {requestUrl} to {targetUrl} (IsPermanent = {isPermanent}",
                        requestUrl, redirectResult.TargetUrl, redirectResult.IsPermanent);

                    context.Response.Redirect(redirectResult.TargetUrl, redirectResult.IsPermanent);

                    return;
                }
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
