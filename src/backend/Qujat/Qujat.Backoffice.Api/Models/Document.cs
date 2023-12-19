using Newtonsoft.Json;
using System;

namespace Qujat.Backoffice.Api.Models
{
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

        [JsonProperty("parentSubcategories")]
        public SubcategoryDto[] ParentSubcategories { get; set; }

        [JsonProperty("sourceContentBlob")]
        public BlobDto SourceContentBlob { get; set; }

        [JsonProperty("filledSampleContentBlob")]
        public BlobDto FilledContentBlob { get; set; }
    }


    public class CreateDocumentRq
    {
        [JsonProperty("nameKz")]
        public string NameKz { get; set; }

        [JsonProperty("nameRu")]
        public string NameRu { get; set; }

        [JsonProperty("descriptionKz")]
        public string DescriptionKz { get; set; }

        [JsonProperty("descriptionRu")]
        public string DescriptionRu { get; set; }

        [JsonProperty("parentSubcategoryIds")]
        public long[] ParentSubcategoryIds { get; set; }

        [JsonProperty("sourceContentBlobId")]
        public long? SourceContentBlobId { get; set; }

        [JsonProperty("filledContentBlobId")]
        public long? FilledContentBlobId { get; set; }
    }


    public class UpdateDocumentRq : CreateDocumentRq
    {
    }

    public enum SortProperty
    {
        NameKz,
        NameRu,
        CreatedOn,
        LastUpdatedOn
    }

    public enum SortOrder
    {
        Ascending,
        Descending,
    }
}
