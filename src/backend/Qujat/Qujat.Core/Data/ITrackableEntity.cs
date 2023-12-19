using System;

namespace Qujat.Core.Data
{
    public interface ITrackableEntity
    {
        DateTime CreatedOn { get; set; }
        DateTime LastUpdatedOn { get; set; }
    }
}
