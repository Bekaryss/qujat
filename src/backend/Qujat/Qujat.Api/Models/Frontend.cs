using Newtonsoft.Json;
using System;

namespace Qujat.Api.Models
{
    public class DocumentCountDto
    {
        [JsonProperty("documentCount")]
        public long DocumentCount { get; set; }
    }


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

    public class CategoryDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nameKz")]
        public string NameKz { get; set; }

        [JsonProperty("iconBlobId")]
        public long? IconBlobId { get; set; }

        [JsonProperty("iconBlob")]
        public BlobDto IconBlob { get; set; }

        [JsonProperty("displayOrder")]
        public int? DisplayOrder { get; set; }

        [JsonProperty("documentsCount")]
        public DocumentCountDto DocumentCount { get; set; }
    }


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

        [JsonProperty("documentsCount")]
        public DocumentCountDto DocumentCount { get; set; }
    }


    public class DocumentDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nameKz")]
        public string NameKz { get; set; }

        [JsonProperty("nameRu")]
        public string NameRu { get; set; }

        [JsonProperty("descriptionKz")]
        public string DescriptionKz { get; set; }

        [JsonProperty("descriptionRu")]
        public string DescriptionRu { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastUpdatedOn")]
        public DateTime LastUpdatedOn { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("deletedOn")]
        public DateTime? DeletedOn { get; set; }

        [JsonProperty("parentSubcategory")]
        public SubcategoryDto ParentSubcategory { get; set; }

        [JsonProperty("sourceContentBlob")]
        public BlobDto SourceContentBlob { get; set; }

        [JsonProperty("filledSampleContentBlob")]
        public BlobDto FilledContentBlob { get; set; }
    }


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
}
