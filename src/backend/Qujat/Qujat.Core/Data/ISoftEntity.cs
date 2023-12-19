using System;

namespace Qujat.Core.Data
{
    public interface ISoftEntity
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedOn { get; set; }
    }
}
