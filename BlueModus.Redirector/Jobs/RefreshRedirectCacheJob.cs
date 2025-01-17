using BlueModus.Redirector.Middleware;
using Quartz;

namespace BlueModus.Redirector.Web.Jobs
{
    public class RefreshRedirectCacheJob : IJob
    {
        private readonly IRedirectItemService _redirectItemService;
        private readonly ILogger<RefreshRedirectCacheJob> _logger;

        public RefreshRedirectCacheJob(IRedirectItemService redirectItemService, ILogger<RefreshRedirectCacheJob> logger)
        {
            _redirectItemService = redirectItemService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Starting RedirectItems refresh...");

            await _redirectItemService.RefreshRedirectItems();

            _logger.LogInformation("Completed RedirectItems refresh");
        }
    }
}
