using AutoMapper;
using Qujat.Core.Data.Entities;

namespace Qujat.Backoffice.Api.Models
{
    public class LinkMappers : Profile
    {
        public LinkMappers()
        {
            CreateMap<LinkEntity, LinkDto>();
            CreateMap<CreateLinkRq, LinkEntity>();
            CreateMap<UpdateLinkRq, LinkEntity>(); 
        }
    }
}
