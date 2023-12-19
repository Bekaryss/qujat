using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Qujat.Core.Data.Entities
{
    /// <summary>
    /// Документ
    /// </summary>
    public class DocumentEntity : ITrackableEntity, ISoftEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Название Документа на казахском языке
        /// </summary>
        public string NameKz { get; set; }

        public string DescriptionKz { get; set; }

        /// <summary>
        /// Название Документа на русском языке
        /// </summary>
        public string NameRu { get; set; }

        public string DescriptionRu { get; set; }

        /// <summary>
        /// N-N отношение с Подкатегориями
        /// </summary>
        public ICollection<DocumentSubcategoryRelationEntity> RelatedParentSubcategories { get; set; }
        
        
        /// <summary>
        /// 1-N отношение с файлами документа
        /// </summary>
        public ICollection<DocumentBlobEntity> DocumentBlobs { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
