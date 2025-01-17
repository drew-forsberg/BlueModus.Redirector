using System.Text.Json.Serialization;

namespace BlueModus.Redirector.Middleware.Models
{
    public class RedirectItem
    {
        [JsonPropertyName("redirectUrl")]
        public required string RedirectUrl { get; set; }

        [JsonPropertyName("targetUrl")]
        public required string TargetUrl { get; set; }

        [JsonPropertyName("redirectType")]
        public int RedirectType { get; set; }

        [JsonPropertyName("useRelative")]
        public bool UseRelative { get; set; }
    }
}