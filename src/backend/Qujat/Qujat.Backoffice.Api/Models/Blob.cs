using Newtonsoft.Json;

namespace Qujat.Backoffice.Api.Models
{
    public class BlobDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string FileName { get; set; }

        [JsonProperty("extension")]
        public string Extension { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
