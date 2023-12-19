using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Qujat.Core.DTOs.Shared;
using Qujat.Api.Models;
using AutoMapper;
using Qujat.Core.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Qujat.Core.Data.Entities;
using Qujat.Core.Exceptions;

namespace Qujat.Api.Controllers.Frontend
{
    [ApiController]
    [Route("open-api/1")]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "openapi")]
    public class OpenApiController(ApplicationDbContext database, IMapper mapper) : ControllerBase
    {
        private readonly ApplicationDbContext _database = database;
        private readonly IMapper _mapper = mapper;
        
        [HttpGet("documents")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<DocumentDto[]>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetDocumentsBySearch(
            [FromQuery(Name = "pageIndex")] int pageIndex = 0,
            [FromQuery(Name = "pageSize")] int pageSize = 10,
            [FromQuery(Name = "nameSearchPattern")] string nameSearchPattern = null,
            CancellationToken ct = default)
        {
            var query = _database.DocumentSubcategoryRelations
                .Include(p => p.Subcategory)
                    .ThenInclude(p => p.ParentCategory)
                .Include(p => p.Document)
                    .ThenInclude(p => p.DocumentBlobs)
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrEmpty(nameSearchPattern))
            {
                query = query.Where(i =>
                    EF.Functions.ILike(i.Document.NameKz, $"{nameSearchPattern}%") ||
                    EF.Functions.ILike(i.Document.NameRu, $"{nameSearchPattern}%") ||
                    EF.Functions.ILike(i.Document.NameKz, $"%{nameSearchPattern}%") ||
                    EF.Functions.ILike(i.Document.NameRu, $"%{nameSearchPattern}%") ||
                    EF.Functions.ILike(i.Document.NameKz, $"%{nameSearchPattern}") ||
                    EF.Functions.ILike(i.Document.NameRu, $"%{nameSearchPattern}"));
            }

            var totalCount = await query.CountAsync(ct);

            var documentSubcategoryRelations = await query
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToArrayAsync(ct);

            var response = new List<DocumentDto>();
            foreach (var relation in documentSubcategoryRelations)
            {
                var documentDto = _mapper.Map<DocumentDto>(relation.Document);
                documentDto.ParentSubcategory = _mapper.Map<SubcategoryDto>(relation.Subcategory);

                var sourceContent = relation
                    .Document
                    .DocumentBlobs
                    .SingleOrDefault(p => p.BlobType == DocumentBlobType.DocumentSource);

                var sampleContent = relation
                    .Document
                    .DocumentBlobs
                    .SingleOrDefault(p => p.BlobType == DocumentBlobType.DocumentFilledSample);

                documentDto.SourceContentBlob = _mapper.Map<BlobDto>(sourceContent);

                if (sampleContent != null)
                {
                    documentDto.FilledContentBlob = _mapper.Map<BlobDto>(sampleContent);
                }

                response.Add(documentDto);
            }

            var apiResposne = ApiResponse<DocumentDto[]>.Ok(response.ToArray(), pageIndex, pageSize, totalCount);
            return Ok(apiResposne);
        }


        [HttpGet("documents/{documentId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<DocumentDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetSingleDocumentById(
            [FromRoute] long documentId, CancellationToken ct)
        {
            var document = await _database.Documents
                .Include(p => p.RelatedParentSubcategories)
                    .ThenInclude(p => p.Subcategory)
                .Include(p => p.DocumentBlobs)
                .SingleOrDefaultAsync(p => p.Id == documentId && !p.IsDeleted, ct) ??
                throw new ResourceNotFoundException($"Документ с ID {documentId} не найден!");


            var response = _mapper.Map<DocumentDto>(document);

            var sourceContent = document.DocumentBlobs
                .SingleOrDefault(p => p.BlobType == DocumentBlobType.DocumentSource);

            var sampleContent = document.DocumentBlobs
                .SingleOrDefault(p => p.BlobType == DocumentBlobType.DocumentFilledSample);

            response.SourceContentBlob = _mapper.Map<BlobDto>(sourceContent);

            if (sampleContent != null)
            {
                response.FilledContentBlob = _mapper.Map<BlobDto>(sampleContent);
            }

            var apiResposne = ApiResponse<DocumentDto>.Ok(response);
            return Ok(apiResposne);
        }
    }
}
