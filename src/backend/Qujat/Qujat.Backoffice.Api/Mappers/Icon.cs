using AutoMapper;
using Qujat.Core.Data.Entities;

namespace Qujat.Backoffice.Api.Models
{
    public class IconMappers : Profile
    {
        public IconMappers()
        {
            CreateMap<IconBlobEntity, BlobDto>();
        }
    }
}
