using Newtonsoft.Json;

namespace Qujat.Backoffice.Api.Models
{
    public class SubcategoryDto
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

        [JsonProperty("displayOrder")]
        public int DisplayOrder { get; set; }

        [JsonProperty("parentCategoryId")]
        public long ParentCategoryId { get; set; }

        [JsonProperty("parentCategory")]
        public CategoryDto ParentCategory { get; set; }
    }

    public class CreateSubcategoryRq
    {
        [JsonProperty("nameKz")]
        public string NameKz { get; set; }

        [JsonProperty("descriptionKz")]
        public string DescriptionKz { get; set; }

        [JsonProperty("nameRu")]
        public string NameRu { get; set; }

        [JsonProperty("descriptionRu")]
        public string DescriptionRu { get; set; }
    }

    public class UpdateSubcategoryRq
    {
        [JsonProperty("nameKz")]
        public string NameKz { get; set; }

        [JsonProperty("descriptionKz")]
        public string DescriptionKz { get; set; }

        [JsonProperty("nameRu")]
        public string NameRu { get; set; }

        [JsonProperty("descriptionRu")]
        public string DescriptionRu { get; set; }
    }

    public class UpdateSubcategoryDisplayOrdersRq
    {
        [JsonProperty("subcategoryIdsByDisplayOrder")]
        public long[] SubcategoryIdsByDisplayOrder { get; set; }
    }
}
