using AutoMapper;
using Qujat.Api.Models;
using Qujat.Core.Data.Entities;

namespace Qujat.Api.Mappers
{
    public class FrontendMappers : Profile
    {
        public FrontendMappers()
        {
            CreateMap<CategoryEntity, CategoryDto>();
            CreateMap<SubcategoryEntity, SubcategoryDto>();
            CreateMap<BlobEntity, BlobDto>();
            CreateMap<DocumentEntity, DocumentDto>();
            CreateMap<DocumentBlobEntity, BlobDto>();
            CreateMap<IconBlobEntity, BlobDto>();
            CreateMap<LinkEntity, LinkDto>();
        }
    }
}
