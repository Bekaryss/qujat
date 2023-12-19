using Newtonsoft.Json;

namespace Qujat.Backoffice.Api.Models
{
    public class LinkDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("nameKz")]
        public string NameKz { get; set; }

        [JsonProperty("nameRu")]
        public string NameRu { get; set; }
    }


    public class CreateLinkRq
    {
        [JsonProperty("nameKz")]
        public string NameKz { get; set; }

        [JsonProperty("nameRu")]
        public string NameRu { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }
    }

    public class UpdateLinkRq
    {
        [JsonProperty("nameKz")]
        public string NameKz { get; set; }

        [JsonProperty("nameRu")]
        public string NameRu { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
