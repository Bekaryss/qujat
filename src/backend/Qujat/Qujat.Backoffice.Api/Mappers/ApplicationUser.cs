using AutoMapper;
using Qujat.Core.Data.Entities;

namespace Qujat.Backoffice.Api.Models
{
    public class ApplicationUserMappers : Profile
    {
        public ApplicationUserMappers()
        {
            CreateMap<ApplicationUserEntity, AdminDto>();
        }
    }
}
