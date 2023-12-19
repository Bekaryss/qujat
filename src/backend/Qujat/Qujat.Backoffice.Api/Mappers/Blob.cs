using AutoMapper;
using Qujat.Core.Data.Entities;

namespace Qujat.Backoffice.Api.Models
{
    public class BlobMappers : Profile
    {
        public BlobMappers()
        {
            CreateMap<BlobEntity, BlobDto>();
        }
    }
}
