using BlueModus.Redirector.Middleware.Models;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BlueModus.Redirector.Middleware.Services
{
    public interface IRedirectItemService
    {
        Task<List<RedirectItem>> GetRedirectItemsAsync(CancellationToken token);
        Task RefreshRedirectItems(CancellationToken token = default);
    }

    public class RedirectItemService : IRedirectItemService
    {
        private readonly HybridCache _hybridCache;
        private readonly ILogger<RedirectItemService> _logger;

        private const string RedirectsCacheKey = "CK_REDIRECTS";

        public RedirectItemService(HybridCache hybridCache, ILogger<RedirectItemService> logger)
        {
            _logger = logger;
            _hybridCache = hybridCache;
        }

        public async Task<List<RedirectItem>> GetRedirectItemsAsync(CancellationToken token = default)
        {
            return await _hybridCache.GetOrCreateAsync(
                RedirectsCacheKey,
                async cancellationToken => await GetDataFromTheSourceAsync(cancellationToken),
                cancellationToken: token);
        }

        public async Task RefreshRedirectItems(CancellationToken token = default)
        {
            var redirectItems = await GetDataFromTheSourceAsync(token);

            _logger.LogInformation("Refreshing redirect items in cache...");

            await _hybridCache.SetAsync(RedirectsCacheKey, redirectItems, cancellationToken: token);
        }

        private async Task<List<RedirectItem>> GetDataFromTheSourceAsync(CancellationToken token)
        {
            try
            {
                _logger.LogInformation("Starting RedirectItem API request...");

                string? json;

                using (var streamReader = new StreamReader(@"d:\redirects.json"))
                {
                    json = await streamReader.ReadToEndAsync(token);
                }

                var redirectItems = JsonSerializer.Deserialize<List<RedirectItem>>(json) ?? [];


                _logger.LogInformation("Completed RedirectItem API request.{itemCount} redirect item(s) found", redirectItems.Count);

                return await Task.FromResult(redirectItems);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "RedirectItem API call failed");
                throw;
            }
        }
    }
}
