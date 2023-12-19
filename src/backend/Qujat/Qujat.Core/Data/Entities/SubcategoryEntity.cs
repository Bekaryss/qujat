using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Qujat.Core.Data.Entities
{
    /// <summary>
    /// Подкатегория документов
    /// </summary>
    public class SubcategoryEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string NameKz { get; set; }
        public string DescriptionKz { get; set; }
        public string NameRu { get; set; }
        public string DescriptionRu { get; set; }

        /// <summary>
        /// Порядок отображения Подкатегории в UI
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// ID родительской Категории
        /// </summary>
        public long ParentCategoryId { get; set; }

        public CategoryEntity ParentCategory { get; set; }

        /// <summary>
        /// N-N отношение с Документами
        /// </summary>
        public ICollection<DocumentSubcategoryRelationEntity> RelatedChildrenDocuments { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
