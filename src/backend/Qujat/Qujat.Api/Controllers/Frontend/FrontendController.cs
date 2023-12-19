using AutoMapper;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using Qujat.Api.Models;
using Qujat.Api.Services;
using Qujat.Core.Data;
using Qujat.Core.Data.Entities;
using Qujat.Core.DTOs.Shared;
using Qujat.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Qujat.Api.Controllers.Frontend
{
    [ApiController]
    [Route("api/1/frontend")]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "frontend")]
    public class FrontendController(ApplicationDbContext database, IMapper mapper) : ControllerBase
    {
        private readonly ApplicationDbContext _database = database;
        private readonly IMapper _mapper = mapper;

        #region main-page

        /// <summary>
        /// Общее количество уникальных документов, без учета дублирования в различных подкатегориях
        /// </summary>
        [HttpGet("main-page/documents/count")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<DocumentCountDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetTotalDocumentsCount(CancellationToken ct)
        {
            var count = await _database.Documents
                .Where(p => !p.IsDeleted)
                .CountAsync(ct);

            return Ok(new ApiResponse<DocumentCountDto>(new DocumentCountDto { DocumentCount = count }));
        }


        [HttpGet("main-page/documents/search")]
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
                    EF.Functions.ILike(i.Document.NameRu, $"{nameSearchPattern}%")); //||
                    //EF.Functions.ILike(i.Document.NameKz, $"%{nameSearchPattern}%") ||
                    //EF.Functions.ILike(i.Document.NameRu, $"%{nameSearchPattern}%") ||
                    //EF.Functions.ILike(i.Document.NameKz, $"%{nameSearchPattern}") ||
                    //EF.Functions.ILike(i.Document.NameRu, $"%{nameSearchPattern}"));
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


        [HttpGet("main-page/documents/top")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<DocumentDto[]>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetTopDocumentsByPopularity(CancellationToken ct)
        {
            const int TOP_LIMIT = 10;

            var documentsByPopularity = await _database.DocumentActionEventEntities
                .Where(p => p.EventType == ActionEventType.Viewed)
                .GroupBy(p => new { p.DocumentId, p.SubcategoryId })
                .OrderBy(p => p.Count())
                .Take(TOP_LIMIT)
                .Select(p => p.Key)
                .ToArrayAsync(ct);

            var response = new List<DocumentDto>();

            foreach (var relation in documentsByPopularity)
            {
                var relationFromDb = await _database.DocumentSubcategoryRelations
                    .Include(p => p.Subcategory)
                        .ThenInclude(p => p.ParentCategory)
                    .Include(p => p.Document)
                        .ThenInclude(p => p.DocumentBlobs)
                    .SingleOrDefaultAsync(p => p.DocumentId == relation.DocumentId && p.SubcategoryId == relation.SubcategoryId, ct);

                if(relationFromDb != null)
                {
                    var documentDto = _mapper.Map<DocumentDto>(relationFromDb.Document);
                    documentDto.ParentSubcategory = _mapper.Map<SubcategoryDto>(relationFromDb.Subcategory);

                    var sourceContent = relationFromDb
                        .Document
                        .DocumentBlobs
                        .SingleOrDefault(p => p.BlobType == DocumentBlobType.DocumentSource);

                    var sampleContent = relationFromDb
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
            }

            var apiResposne = ApiResponse<DocumentDto[]>.Ok(response.ToArray(), 0, TOP_LIMIT, TOP_LIMIT);
            return Ok(apiResposne);
        }


        [HttpGet("main-page/categories")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto[]>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetCategories(
            [FromQuery(Name = "pageIndex")] int pageIndex = 0,
            [FromQuery(Name = "pageSize")] int pageSize = 10,
            CancellationToken ct = default)
        {
            var categoriesFromDb = await _database.Categories
                .Include(p => p.Subcategories)
                .Include(p => p.IconBlob)
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.DisplayOrder)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToArrayAsync(ct);

            var totalCategoriesCount = await _database.Categories
                .Where(p => !p.IsDeleted)
                .CountAsync(ct);

            var categories = categoriesFromDb
                .Select(_mapper.Map<CategoryDto>)
                .ToArray();

            // TODO: rewrite with group-by
            foreach (var category in categoriesFromDb)
            {
                var subcategoryIds = category.Subcategories
                    .Select(p => p.Id)
                    .ToArray();

                var documentsInCategory = await _database.Documents
                    .Where(p => p.RelatedParentSubcategories.Any(x => subcategoryIds.Contains(x.SubcategoryId)))
                    .CountAsync(ct);

                var categoryModel = categories.SingleOrDefault(p => p.Id == category.Id);
                categoryModel.DocumentCount = new DocumentCountDto { DocumentCount = documentsInCategory };
            }

            var response = ApiResponse<CategoryDto[]>.Ok(categories, pageIndex, pageSize, totalCategoriesCount);
            return Ok(response);
        }

        #endregion

        #region subcategories-page

        [HttpGet("subcategories-page/{parentCategoryId:long}/subcategories")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<SubcategoryDto[]>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetSubcategories(
            [FromRoute] long parentCategoryId,
            [FromQuery(Name = "pageIndex")] int pageIndex = 0,
            [FromQuery(Name = "pageSize")] int pageSize = 10,
            CancellationToken ct = default)
        {
            var subcategoriesFromDb = await _database
                .Subcategories
                .Where(p => !p.IsDeleted && p.ParentCategoryId == parentCategoryId)
                .OrderBy(p => p.Id)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToArrayAsync(ct);

            var totalSubcategoriesCount = await _database
                .Subcategories
                .Where(p => !p.IsDeleted && p.ParentCategoryId == parentCategoryId)
                .CountAsync(ct);

            var subcategories = subcategoriesFromDb
                .Select(_mapper.Map<SubcategoryDto>)
                .ToArray();

            // TODO: rewrite with group-by
            foreach (var subcategory in subcategoriesFromDb)
            {
                var documentsInSubcategory = await _database.DocumentSubcategoryRelations
                    .Where(p => p.SubcategoryId == subcategory.Id)
                    .CountAsync(ct);

                var categoryModel = subcategories.SingleOrDefault(p => p.Id == subcategory.Id);
                categoryModel.DocumentCount = new DocumentCountDto { DocumentCount = documentsInSubcategory };
            }

            var response = ApiResponse<SubcategoryDto[]>.Ok(subcategories, pageIndex, pageSize, totalSubcategoriesCount);
            return Ok(response);
        }


        [HttpGet("subcategories-page/{parentCategoryId:long}/subcategories/{parentSubcategoryId:long}/documents")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<DocumentDto[]>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetDocumentsBySubcategories(
            [FromRoute] long parentCategoryId,
            [FromRoute] long parentSubcategoryId,
            [FromQuery(Name = "pageIndex")] int pageIndex = 0,
            [FromQuery(Name = "pageSize")] int pageSize = 10,
            [FromQuery(Name = "nameSearchPattern")] string nameSearchPattern = null,
            CancellationToken ct = default)
        {
            var subcategory = await _database.Subcategories
                .Include(p => p.ParentCategory)
                .SingleOrDefaultAsync(p => p.Id == parentSubcategoryId && p.ParentCategoryId == parentCategoryId, ct) ??
                throw new ResourceNotFoundException();

            var query = _database.Documents
                .Include(p => p.RelatedParentSubcategories)
                    .ThenInclude(p => p.Subcategory)
                .Include(p => p.DocumentBlobs)
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Where(p => p.RelatedParentSubcategories.Any(x => 
                    x.SubcategoryId == parentSubcategoryId &&
                    x.Subcategory.ParentCategoryId == parentCategoryId));

            if (!string.IsNullOrEmpty(nameSearchPattern))
            {
                query = query.Where(i =>
                    EF.Functions.ILike(i.NameKz, $"{nameSearchPattern}%") ||
                    EF.Functions.ILike(i.NameRu, $"{nameSearchPattern}%")); //||
                    //EF.Functions.ILike(i.NameKz, $"%{nameSearchPattern}%") ||
                    //EF.Functions.ILike(i.NameRu, $"%{nameSearchPattern}%") ||
                    //EF.Functions.ILike(i.NameKz, $"%{nameSearchPattern}") ||
                    //EF.Functions.ILike(i.NameRu, $"%{nameSearchPattern}"));
            }

            var totalCount = await query.CountAsync(ct);

            var documents = await query
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToArrayAsync(ct);

            var response = new List<DocumentDto>();

            foreach (var item in documents)
            {
                var documentDto = _mapper.Map<DocumentDto>(item);
                documentDto.ParentSubcategory = _mapper.Map<SubcategoryDto>(subcategory);

                var sourceContent = item
                    .DocumentBlobs
                    .SingleOrDefault(p => p.BlobType == DocumentBlobType.DocumentSource);

                var sampleContent = item
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

        #endregion



        #region single-document-page

        [HttpGet("single-document-page/category/{parentCategoryId:long}/subcategories/{parentSubcategoryId:long}/document/{documentId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<DocumentDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetSingleDocumentById(
            [FromRoute] long parentCategoryId,
            [FromRoute] long parentSubcategoryId,
            [FromRoute] long documentId, CancellationToken ct)
        {
            var subcategory = await _database.Subcategories
                .Include(p => p.ParentCategory)
                .SingleOrDefaultAsync(p => p.Id == parentSubcategoryId && p.ParentCategoryId == parentCategoryId, ct) ??
                throw new ResourceNotFoundException();

            var document = await _database.Documents
                .Include(p => p.RelatedParentSubcategories)
                    .ThenInclude(p => p.Subcategory)
                .Include(p => p.DocumentBlobs)
                .SingleOrDefaultAsync(p => p.Id == documentId && !p.IsDeleted, ct) ??
                throw new ResourceNotFoundException($"Документ с ID {documentId} не найден!");

            if(!document.RelatedParentSubcategories.Any(x => x.SubcategoryId == parentSubcategoryId))
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

            response.ParentSubcategory = _mapper.Map<SubcategoryDto>(subcategory);

            var apiResposne = ApiResponse<DocumentDto>.Ok(response);
            return Ok(apiResposne);
        }

        public class SendDocumentToEmailRq
        {
            [JsonProperty("email")]
            public string Email { get; set; }
        }

        [HttpPost("single-document-page/category/{parentCategoryId:long}/subcategories/{parentSubcategoryId:long}/document/{documentId:long}/actions/send-to")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> SendSingleDocumentById(
            [FromRoute] long parentCategoryId,
            [FromRoute] long parentSubcategoryId,
            [FromRoute] long documentId,
            [FromBody] SendDocumentToEmailRq rq,
            [FromServices] HttpClient httpClient,
            [FromServices] EmailSenderProviderFactory emailSenderFactory,
            CancellationToken ct)
        {
            if (rq == null) throw new EmptyRqBodyException();
            
            var relationExist = await _database.DocumentSubcategoryRelations.AnyAsync(p =>
               p.SubcategoryId == parentSubcategoryId &&
               p.Subcategory.ParentCategoryId == parentCategoryId &&
               p.DocumentId == documentId, ct);

            if (!relationExist)
                throw new ResourceNotFoundException();

            var document = await _database.Documents
                .AsNoTracking()
                .Include(p => p.DocumentBlobs)
                .SingleOrDefaultAsync(p => p.Id == documentId, ct) ??
                throw new ResourceNotFoundException();

            var documentBlob = document.DocumentBlobs
                .SingleOrDefault(p => p.BlobType == DocumentBlobType.DocumentSource);

            var documentBlobContent = await httpClient.GetByteArrayAsync(documentBlob.Uri, ct);

            _database.DocumentActionEventEntities.Add(new DocumentActionEventEntity 
            { 
                EventType = ActionEventType.SentToEmail, 
                DocumentId = document.Id,
                SentEmail = rq.Email,
                SubcategoryId = parentSubcategoryId
            });

            await _database.SaveChangesAsync(ct);

            var emailSenderProvider = await emailSenderFactory.GetEmailSenderProvider();

            await emailSenderProvider.SendEmail(rq.Email, 
                $"Пример документа от qujat.kz, {document.NameKz}", $"Пример документа от qujat.kz, {document.NameKz}", 
                documentBlob.FileName, documentBlobContent, ct);

            return Ok(ApiResponse.Ok());
        }

        #endregion


        #region links-page

        [HttpGet("links-page/links")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<LinkDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetLinks(
            [FromQuery(Name = "pageIndex")] int pageIndex = 0,
            [FromQuery(Name = "pageSize")] int pageSize = 10,
            CancellationToken ct = default)
        {
            var linksFromDb = await _database
                .Links
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.DisplayOrder)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToArrayAsync(ct);

            var totalLinksCount = await _database.Links
                .Where(p => !p.IsDeleted)
                .CountAsync(ct);

            var links = linksFromDb
                .Select(_mapper.Map<LinkDto>)
                .ToArray();

            var response = ApiResponse<LinkDto[]>.Ok(links, pageIndex, pageSize, totalLinksCount);
            return Ok(response);
        }

        #endregion


        #region statistics-triggers

        // category/{parentCategoryId:long}/subcategories/{parentSubcategoryId:long}/document/{documentId:long}
        [HttpPost("statistics-triggers/category/{parentCategoryId:long}/subcategories/{parentSubcategoryId:long}/document/{documentId:long}/viewed")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> PostDocumentViewdEvent(
            [FromRoute] long parentCategoryId,
            [FromRoute] long parentSubcategoryId,
            [FromRoute] long documentId, CancellationToken ct = default)
        {
            var relationExist = await _database.DocumentSubcategoryRelations.AnyAsync(p =>
                p.SubcategoryId == parentSubcategoryId &&
                p.Subcategory.ParentCategoryId == parentCategoryId &&
                p.DocumentId == documentId, ct);

            if (!relationExist)
                throw new ResourceNotFoundException();

            _database.DocumentActionEventEntities.Add(new DocumentActionEventEntity
            {
                EventType = ActionEventType.Viewed,
                DocumentId = documentId,
                SubcategoryId = parentSubcategoryId,
            });

            await _database.SaveChangesAsync(ct);

            return Ok(ApiResponse.Ok());
        }


        [HttpPost("statistics-triggers/category/{parentCategoryId:long}/subcategories/{parentSubcategoryId:long}/document/{documentId:long}/downloaded")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> PostDocumentDownloadedEvent(
            [FromRoute] long parentCategoryId,
            [FromRoute] long parentSubcategoryId,
            [FromRoute] long documentId, CancellationToken ct = default)
        {
            var relationExist = await _database.DocumentSubcategoryRelations.AnyAsync(p =>
                p.SubcategoryId == parentSubcategoryId &&
                p.Subcategory.ParentCategoryId == parentCategoryId &&
                p.DocumentId == documentId, ct);

            if (!relationExist)
                throw new ResourceNotFoundException();

            _database.DocumentActionEventEntities.Add(new DocumentActionEventEntity
            {
                EventType = ActionEventType.Downloaded,
                DocumentId = documentId,
                SubcategoryId = parentSubcategoryId
            });

            await _database.SaveChangesAsync(ct);

            return Ok(ApiResponse.Ok());
        }

        [HttpPost("statistics-triggers/category/{parentCategoryId:long}/subcategories/{parentSubcategoryId:long}/document/{documentId:long}/printed")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> PostDocumentPrintedEvent(
            [FromRoute] long parentCategoryId,
            [FromRoute] long parentSubcategoryId,
            [FromRoute] long documentId, 
            CancellationToken ct = default)
        {
            var relationExist = await _database.DocumentSubcategoryRelations.AnyAsync(p =>
                p.SubcategoryId == parentSubcategoryId &&
                p.Subcategory.ParentCategoryId == parentCategoryId &&
                p.DocumentId == documentId, ct);

            if (!relationExist)
                throw new ResourceNotFoundException();

            _database.DocumentActionEventEntities.Add(new DocumentActionEventEntity
            {
                EventType = ActionEventType.Printed,
                DocumentId = documentId,
                SubcategoryId = parentSubcategoryId,
            });

            await _database.SaveChangesAsync(ct);

            return Ok(ApiResponse.Ok());
        }

        #endregion
    }
}
