using AutoMapper;
using Qujat.Core.Data.Entities;

namespace Qujat.Backoffice.Api.Models
{
    public class CategoryMappers : Profile
    {
        public CategoryMappers()
        {
            CreateMap<CategoryEntity, CategoryDto>();
            CreateMap<CreateCategoryRq, CategoryEntity>();
            CreateMap<UpdateCategoryRq, CategoryEntity>();
        }
    }
}
