using AutoMapper;
using Qujat.Core.Data.Entities;

namespace Qujat.Backoffice.Api.Models
{
    public class SubcategoryMappers : Profile
    {
        public SubcategoryMappers()
        {
            CreateMap<SubcategoryEntity, SubcategoryDto>();
            CreateMap<CreateSubcategoryRq, SubcategoryEntity>();
            CreateMap<UpdateSubcategoryRq, SubcategoryEntity>();
        }
    }
}
