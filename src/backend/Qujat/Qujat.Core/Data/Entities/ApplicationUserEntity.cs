using Microsoft.AspNetCore.Identity;
using System;

namespace Qujat.Core.Data.Entities
{
    public enum ApplicationUserType
    {
        RootAdmin,
        Admin,
        //User
    }

    public class ApplicationUserEntity : IdentityUser<long>,
        ITrackableEntity, ISoftEntity
    {
        public string FullName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public ApplicationUserType UserType { get; set; }
        public bool IsAccountActivatedByUser { get; set; }
    }
}
