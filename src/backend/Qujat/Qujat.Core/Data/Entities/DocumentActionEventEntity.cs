using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qujat.Core.Data.Entities
{
    public enum ActionEventType
    {
        Uploaded,
        Viewed,
        Downloaded,
        Printed,
        SentToEmail
    }


    public class DocumentActionEventEntity : ITrackableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long DocumentId { get; set; }
        public long SubcategoryId { get; set; }
        public ActionEventType EventType { get; set; }
        public string SentEmail { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
    }
}
