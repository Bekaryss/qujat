using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Qujat.Core.Data.Entities
{
    /// <summary>
    /// Категория документов
    /// </summary>
    public class CategoryEntity : ITrackableEntity, ISoftEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string NameKz { get; set; }
        public string DescriptionKz { get; set; }

        public string NameRu { get; set; }
        public string DescriptionRu { get; set; }

        /// <summary>
        /// ID файла с иконкой в БД
        /// </summary>
        public long? IconBlobId { get; set; }
        public IconBlobEntity IconBlob { get; set; }

        /// <summary>
        /// Порядок отображения Категории в UI
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Подкатегории 2-ого уровня, 1-N
        /// </summary>
        public ICollection<SubcategoryEntity> Subcategories { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
