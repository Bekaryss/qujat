using System;

namespace Qujat.Core.Data.Entities
{
    public class LinkEntity : ISoftEntity, ITrackableEntity
    {
        public long Id { get; set; }
        public string NameKz { get; set; }
        public string NameRu { get; set; }
        public string Uri { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
