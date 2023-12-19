﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Qujat.Core.Data.Entities
{
    public class BlobEntity : ITrackableEntity, ISoftEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }
        public byte[] Content { get; set; }
        public string Uri { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsUsed { get; set; }
    }
}
