using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qujat.Core.Data.Entities
{
    public enum DocumentBlobType
    {
        DocumentSource,
        DocumentFilledSample
    }

    /// <summary>
    /// Файл документа
    /// </summary>
    public class DocumentBlobEntity : ITrackableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DocumentEntity ParentDocument { get; set; }
        public long? ParentDocumentId { get; set; }

        public string FileName { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }
        public byte[] Content { get; set; }
        public long BlobId { get; set; }
        public string Uri { get; set; }
        public DocumentBlobType BlobType { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
    }
}
