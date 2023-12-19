using AutoMapper;
using Qujat.Core.Data.Entities;

namespace Qujat.Backoffice.Api.Models
{
    public class DocumentMappers : Profile
    {
        public DocumentMappers()
        {
            CreateMap<DocumentEntity, DocumentDto>();
            CreateMap<DocumentBlobEntity, BlobDto>();
            CreateMap<CreateDocumentRq, DocumentEntity>();
            CreateMap<UpdateDocumentRq, DocumentEntity>();
        }
    }
}
