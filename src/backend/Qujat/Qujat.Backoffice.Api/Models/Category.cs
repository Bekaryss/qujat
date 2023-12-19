using Newtonsoft.Json;

namespace Qujat.Backoffice.Api.Models
{
    public class CategoryDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nameKz")]
        public string NameKz { get; set; }

        [JsonProperty("descriptionKz")]
        public string DescriptionKz { get; set; }

        [JsonProperty("nameRu")]
        public string NameRu { get; set; }

        [JsonProperty("descriptionRu")]
        public string DescriptionRu { get; set; }

        [JsonProperty("iconBlobId")]
        public long? IconBlobId { get; set; }

        [JsonProperty("iconBlob")]
        public BlobDto IconBlob { get; set; }

        [JsonProperty("displayOrder")]
        public int DisplayOrder { get; set; }
    }

    public class UpdateCategoryDisplayOrdersRq
    {
        [JsonProperty("categoryIdsByDisplayOrder")]
        public long[] CategoryIdsByDisplayOrder { get; set; }
    }

    public class CreateCategoryRq
    {
        [JsonProperty("nameKz")]
        public string NameKz { get; set; }

        [JsonProperty("descriptionKz")]
        public string DescriptionKz { get; set; }

        [JsonProperty("nameRu")]
        public string NameRu { get; set; }

        [JsonProperty("descriptionRu")]
        public string DescriptionRu { get; set; }

        [JsonProperty("iconBlobId")]
        public long? IconBlobId { get; set; }
    }

    public class UpdateCategoryRq
    {
        [JsonProperty("nameKz")]
        public string NameKz { get; set; }

        [JsonProperty("descriptionKz")]
        public string DescriptionKz { get; set; }

        [JsonProperty("nameRu")]
        public string NameRu { get; set; }

        [JsonProperty("descriptionRu")]
        public string DescriptionRu { get; set; }

        [JsonProperty("iconBlobId")]
        public long? IconBlobId { get; set; }
    }
}
