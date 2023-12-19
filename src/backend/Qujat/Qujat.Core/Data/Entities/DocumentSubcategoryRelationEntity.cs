using System;

namespace Qujat.Core.Data.Entities
{
    public class DocumentSubcategoryRelationEntity : ITrackableEntity
    {
        public long DocumentId { get; set; }
        public DocumentEntity Document { get; set; }
        public long SubcategoryId { get; set; }
        public SubcategoryEntity Subcategory { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
