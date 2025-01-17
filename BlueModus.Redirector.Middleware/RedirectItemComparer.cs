namespace BlueModus.Redirector.Middleware
{
    public interface IRedirectItemComparer
    {
        RedirectResult? GetMatchingRedirectItem(string? requestUrl, IList<RedirectItem> redirectItems);
    }

    public class RedirectItemComparer : IRedirectItemComparer
    {
        public RedirectResult? GetMatchingRedirectItem(string? requestUrl, IList<RedirectItem> redirectItems)
        {
            var normalizedRequestUrl = requestUrl?.TrimEnd('/').ToLower();

            if (string.IsNullOrEmpty(normalizedRequestUrl))
            {
                return null;
            }

            var exactMatch = redirectItems.FirstOrDefault(x =>
                x.UseRelative == false && normalizedRequestUrl.Equals(x.RedirectUrl, StringComparison.OrdinalIgnoreCase));

            if (exactMatch != null)
            {
                return new RedirectResult
                {
                    TargetUrl = exactMatch.TargetUrl,
                    IsPermanent = exactMatch.RedirectType == 301
                };
            }

            var relativeMatch = redirectItems.FirstOrDefault(x =>
                x.UseRelative && normalizedRequestUrl.StartsWith(x.RedirectUrl, StringComparison.OrdinalIgnoreCase));

            if (relativeMatch != null)
            {
                return new RedirectResult
                {
                    TargetUrl = normalizedRequestUrl.Replace(relativeMatch.RedirectUrl, relativeMatch.TargetUrl),
                    IsPermanent = relativeMatch.RedirectType == 301
                };
            }

            return null;
        }
    }
}
