using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Qujat.Backoffice.Api.Models;
using Qujat.Core.Data;
using Qujat.Core.Data.Entities;
using Qujat.Core.DTOs;
using Qujat.Core.DTOs.Shared;
using Qujat.Core.Exceptions;
using Qujat.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Qujat.Backoffice.Api.Controllers
{
    [ApiController]
    [Route("api/1/documents")]
    [AuthorizeByAccessToken]
    public class DocumentController(ApplicationDbContext database, 
        ICurrentUserProvider currentUserProvider,
        IMapper mapper) : ControllerBase
    {
        private readonly ApplicationDbContext _database = database;
        private readonly CurrentRqUser _currentUser = currentUserProvider.GetCurrentUser();
        private readonly IMapper _mapper = mapper;

        [HttpGet("{documentId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<DocumentDto>), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> GetDocumentById(
            [FromRoute] long documentId,
            CancellationToken ct)
        {
            var document = await _database.Documents
                .Include(p => p.RelatedParentSubcategories)
                    .ThenInclude(p => p.Subcategory)
                        .ThenInclude(p => p.ParentCategory)
                .Include(p => p.DocumentBlobs)
                .SingleOrDefaultAsync(p => p.Id == documentId && !p.IsDeleted, ct) ??
                throw new ResourceNotFoundException($"Документ с ID {documentId} не найден!");

            var response = _mapper.Map<DocumentDto>(document);

            response.ParentSubcategories = document.RelatedParentSubcategories
                .Select(p => p.Subcategory)
                .Select(_mapper.Map<SubcategoryDto>).ToArray();

            var sourceContent = document.DocumentBlobs
                .SingleOrDefault(p => p.BlobType == DocumentBlobType.DocumentSource);

            var sampleContent = document.DocumentBlobs
                .SingleOrDefault(p => p.BlobType == DocumentBlobType.DocumentFilledSample);

            response.SourceContentBlob = _mapper.Map<BlobDto>(sourceContent);
            
            if(sampleContent != null)
            {
                response.FilledContentBlob = _mapper.Map<BlobDto>(sampleContent);
            }

            var apiResposne = ApiResponse<DocumentDto>.Ok(response);
            return Ok(apiResposne);
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<DocumentDto[]>), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> GetDocuments(
            [FromQuery(Name = "pageIndex")] int pageIndex = 0,
            [FromQuery(Name = "pageSize")] int pageSize = 10,
            [FromQuery(Name = "nameSearchPattern")] string nameSearchPattern = null,
            [FromQuery(Name = "parentCategoryIds")] long[] parentCategoryIds = null,
            [FromQuery(Name = "parentSubcategoryIds")] long[] parentSubcategoryIds = null,
            [FromQuery(Name = "sortProperty")] SortProperty sortProperty = SortProperty.NameKz,
            [FromQuery(Name = "sortOrder")] SortOrder sortOrder = SortOrder.Ascending,
            CancellationToken ct = default)
        {
            var query = _database.Documents
                .Include(p => p.RelatedParentSubcategories)
                    .ThenInclude(p => p.Subcategory)
                        .ThenInclude(p => p.ParentCategory)
                .Include(p => p.DocumentBlobs)
                .Where(p => !p.IsDeleted);

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

            if(parentCategoryIds != null && parentCategoryIds.Any())
            {
                query = query
                    .Where(p => p.RelatedParentSubcategories.Any(
                        x => parentCategoryIds.Contains(x.Subcategory.ParentCategoryId)));
            }

            if(parentSubcategoryIds != null && parentSubcategoryIds.Any())
            {
                query = query
                    .Where(p => p.RelatedParentSubcategories.Any(
                        x => parentSubcategoryIds.Contains(x.SubcategoryId)));
            }

            var totalCount = await query.CountAsync(ct);

            var orderedQuery = default(IOrderedQueryable<DocumentEntity>);

            if(sortProperty == SortProperty.NameKz && sortOrder == SortOrder.Ascending)
            {
                orderedQuery = query.OrderBy(p => p.NameKz);
            }
            else if(sortProperty == SortProperty.NameKz && sortOrder == SortOrder.Descending)
            {
                orderedQuery = query.OrderByDescending(p => p.NameKz);
            }
            else if(sortProperty == SortProperty.NameRu && sortOrder == SortOrder.Ascending)
            {
                orderedQuery = query.OrderBy(p => p.NameRu);
            }
            else if(sortProperty == SortProperty.NameRu && sortOrder == SortOrder.Descending)
            {
                orderedQuery = query.OrderByDescending(p => p.NameRu);
            }
            else if(sortProperty == SortProperty.CreatedOn && sortOrder == SortOrder.Ascending)
            {
                orderedQuery = query.OrderBy(p => p.CreatedOn);
            }
            else if(sortProperty == SortProperty.CreatedOn && sortOrder == SortOrder.Descending)
            {
                orderedQuery = query.OrderByDescending(p => p.CreatedOn);
            }
            else if(sortProperty == SortProperty.LastUpdatedOn && sortOrder == SortOrder.Ascending)
            {
                orderedQuery = query.OrderBy(p => p.LastUpdatedOn);
            }
            else if(sortProperty == SortProperty.LastUpdatedOn && sortOrder == SortOrder.Descending)
            {
                orderedQuery = query.OrderByDescending(p => p.LastUpdatedOn);
            }
            
            var documents = await orderedQuery
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToArrayAsync(ct);

            var response = new List<DocumentDto>();
            foreach (var item in documents)
            {
                var documentDto = _mapper.Map<DocumentDto>(item);

                documentDto.ParentSubcategories = item.RelatedParentSubcategories
                    .Select(p => p.Subcategory)
                    .Select(_mapper.Map<SubcategoryDto>).ToArray();

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


        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<DocumentDto>), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> CreateDocument(
            [FromBody] CreateDocumentRq rq,
            CancellationToken ct)
        {
            if (rq == null) throw new EmptyRqBodyException();

            if (rq.ParentSubcategoryIds == null || rq.ParentSubcategoryIds.Length == 0)
                throw new BadRequestException("Нужно указать хотя бы 1 родительскую подкатегорию");

            var parentSubcategories = await _database.Subcategories
                .AsNoTracking()
                .Where(p => rq.ParentSubcategoryIds.Contains(p.Id))
                .ToArrayAsync(ct);

            var selectedParentSubcategoryIds = rq.ParentSubcategoryIds
                .ToImmutableSortedSet()
                .ToArray();

            if (selectedParentSubcategoryIds.Length != parentSubcategories.Length)
                throw new BadRequestException(
                    "Одна или несколько из выбранных родительских подкатегории не найдена");

            if(!rq.SourceContentBlobId.HasValue)
                throw new BadRequestException("Нужно прикрепить оригинал документа!");

            var sourceDocumentBlob = await _database.Blobs
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == rq.SourceContentBlobId.Value, ct) ??
                throw new BadRequestException("Нужно прикрепить оригинал документа!");

            var allowedBlobExtensions = new string[] { ".docx", ".pdf" };

            if (!allowedBlobExtensions.Contains(sourceDocumentBlob.Extension))
                throw new BadRequestException(
                    $"Расширение документа может быть лишь [{string.Join(',', allowedBlobExtensions)}]");

            var filledSampleDocumentBlob = default(BlobEntity);

            if (rq.FilledContentBlobId.HasValue)
            {
                filledSampleDocumentBlob = await _database.Blobs
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.Id == rq.FilledContentBlobId.Value, ct) ??
                    throw new BadRequestException("Нужно прикрепить оригинал документа!");

                if (!allowedBlobExtensions.Contains(filledSampleDocumentBlob.Extension))
                    throw new BadRequestException(
                        $"Расширение документа может быть лишь [{string.Join(',', allowedBlobExtensions)}]");
            }

            var documentBlobs = new List<DocumentBlobEntity>
            {
                new() 
                {
                    Uri = sourceDocumentBlob.Uri,
                    BlobId = sourceDocumentBlob.Id,
                    Extension = sourceDocumentBlob.Extension,
                    MimeType = sourceDocumentBlob.MimeType,
                    FileName = sourceDocumentBlob.FileName,
                    Content = sourceDocumentBlob.Content,
                    BlobType = DocumentBlobType.DocumentSource
                }
            };

            if(filledSampleDocumentBlob != null)
            {
                documentBlobs.Add(new DocumentBlobEntity
                {
                    Uri = filledSampleDocumentBlob.Uri,
                    BlobId = filledSampleDocumentBlob.Id,
                    Extension = filledSampleDocumentBlob.Extension,
                    MimeType = filledSampleDocumentBlob.MimeType,
                    FileName = filledSampleDocumentBlob.FileName,
                    Content = filledSampleDocumentBlob.Content,
                    BlobType = DocumentBlobType.DocumentFilledSample
                });
            }

            var documentToCreate = new DocumentEntity
            {
                NameKz = rq.NameKz,
                NameRu = rq.NameRu,
                DescriptionKz = rq.DescriptionKz,
                DescriptionRu = rq.DescriptionRu,
                DocumentBlobs = documentBlobs
            };

            _database.Documents.Add(documentToCreate);
            await _database.SaveChangesAsync(ct);

            var documentSubcategoryRelationsToCreate = parentSubcategories
                .Select(p => new DocumentSubcategoryRelationEntity
                {
                    DocumentId = documentToCreate.Id,
                    SubcategoryId = p.Id
                })
                .ToArray();

            _database.DocumentSubcategoryRelations.AddRange(documentSubcategoryRelationsToCreate);

            await _database.SaveChangesAsync(ct);

            var document = await _database.Documents
                .Include(p => p.RelatedParentSubcategories)
                    .ThenInclude(p => p.Subcategory)
                        .ThenInclude(p => p.ParentCategory)
                .Include(p => p.DocumentBlobs)
                .SingleOrDefaultAsync(p => p.Id == documentToCreate.Id, ct);

            var response = _mapper.Map<DocumentDto>(document);

            response.ParentSubcategories = document.RelatedParentSubcategories
                .Select(p => p.Subcategory)
                .Select(_mapper.Map<SubcategoryDto>).ToArray();

            var sourceContent = document.DocumentBlobs
                .SingleOrDefault(p => p.BlobType == DocumentBlobType.DocumentSource);

            var sampleContent = document.DocumentBlobs
                .SingleOrDefault(p => p.BlobType == DocumentBlobType.DocumentFilledSample);

            response.SourceContentBlob = _mapper.Map<BlobDto>(sourceContent);

            if (sampleContent != null)
            {
                response.FilledContentBlob = _mapper.Map<BlobDto>(sampleContent);
            }


            foreach (var item in documentSubcategoryRelationsToCreate)
            {
                _database.DocumentActionEventEntities.Add(new DocumentActionEventEntity
                {
                    DocumentId = documentToCreate.Id,
                    EventType = ActionEventType.Uploaded,
                    SubcategoryId = item.SubcategoryId
                });
            }

            await _database.SaveChangesAsync(ct);

            var apiResposne = ApiResponse<DocumentDto>.Ok(response);
            return Ok(apiResposne);
        }


        [HttpPatch("{documentId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<DocumentDto>), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> UpdateDocument(
            [FromRoute] long documentId,
            [FromBody] UpdateDocumentRq rq,
            CancellationToken ct)
        {
            if (rq == null) throw new EmptyRqBodyException();

            var documentToUpdate = await _database.Documents
                .Include(p => p.DocumentBlobs)
                .SingleOrDefaultAsync(p => p.Id == documentId && !p.IsDeleted, ct) ??
                throw new ResourceNotFoundException($"Документ с ID {documentId} не найден!");

            if (rq.ParentSubcategoryIds == null || rq.ParentSubcategoryIds.Length == 0)
                throw new BadRequestException("Нужно указать хотя бы 1 родительскую подкатегорию");

            var parentSubcategories = await _database.Subcategories
                .AsNoTracking()
                .Where(p => rq.ParentSubcategoryIds.Contains(p.Id))
                .ToArrayAsync(ct);

            var selectedParentSubcategoryIds = rq.ParentSubcategoryIds
                .ToImmutableSortedSet()
                .ToArray();

            if (selectedParentSubcategoryIds.Length != parentSubcategories.Length)
                throw new BadRequestException(
                    "Одна или несколько из выбранных родительских подкатегории не найдена");

            if (!rq.SourceContentBlobId.HasValue)
                throw new BadRequestException("Нужно прикрепить оригинал документа!");

            var sourceDocumentBlob = await _database.Blobs
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == rq.SourceContentBlobId.Value, ct) ??
                throw new BadRequestException("Нужно прикрепить оригинал документа!");

            var allowedBlobExtensions = new string[] { ".docx", ".pdf" };

            if (!allowedBlobExtensions.Contains(sourceDocumentBlob.Extension))
                throw new BadRequestException(
                    $"Расширение документа может быть лишь [{string.Join(',', allowedBlobExtensions)}]");

            var filledSampleDocumentBlob = default(BlobEntity);

            if (rq.FilledContentBlobId.HasValue)
            {
                filledSampleDocumentBlob = await _database.Blobs
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.Id == rq.FilledContentBlobId.Value, ct) ??
                    throw new BadRequestException("Нужно прикрепить оригинал документа!");

                if (!allowedBlobExtensions.Contains(filledSampleDocumentBlob.Extension))
                    throw new BadRequestException(
                        $"Расширение документа может быть лишь [{string.Join(',', allowedBlobExtensions)}]");
            }

            _database.DocumentBlobs.RemoveRange(documentToUpdate.DocumentBlobs);

            var documentCategoryRelationsToRemove = await _database
                .DocumentSubcategoryRelations
                .AsNoTracking()
                .Where(p => p.DocumentId == documentToUpdate.Id)
                .ToArrayAsync(ct);

            _database.DocumentSubcategoryRelations.RemoveRange(documentCategoryRelationsToRemove);

            await _database.SaveChangesAsync(ct);

            documentToUpdate.NameKz = rq.NameKz;
            documentToUpdate.NameRu = rq.NameRu;
            documentToUpdate.DescriptionKz = rq.DescriptionKz;
            documentToUpdate.DescriptionRu = rq.DescriptionRu;   

            var documentBlobs = new List<DocumentBlobEntity>
            {
                new()
                {
                    Uri = sourceDocumentBlob.Uri,
                    BlobId = sourceDocumentBlob.Id,
                    Extension = sourceDocumentBlob.Extension,
                    MimeType = sourceDocumentBlob.MimeType,
                    FileName = sourceDocumentBlob.FileName,
                    Content = sourceDocumentBlob.Content,
                    BlobType = DocumentBlobType.DocumentSource
                }
            };

            if (filledSampleDocumentBlob != null)
            {
                documentBlobs.Add(new DocumentBlobEntity
                {
                    Uri = filledSampleDocumentBlob.Uri,
                    BlobId = filledSampleDocumentBlob.Id,
                    Extension = filledSampleDocumentBlob.Extension,
                    MimeType = filledSampleDocumentBlob.MimeType,
                    FileName = filledSampleDocumentBlob.FileName,
                    Content = filledSampleDocumentBlob.Content,
                    BlobType = DocumentBlobType.DocumentFilledSample
                });
            }

            documentToUpdate.DocumentBlobs = documentBlobs;

            _database.Documents.Update(documentToUpdate);
            await _database.SaveChangesAsync(ct);

            var documentSubcategoryRelationsToCreate = parentSubcategories
                .Select(p => new DocumentSubcategoryRelationEntity
                {
                    DocumentId = documentToUpdate.Id,
                    SubcategoryId = p.Id
                })
                .ToArray();

            _database.DocumentSubcategoryRelations.AddRange(documentSubcategoryRelationsToCreate);
            await _database.SaveChangesAsync(ct);

            var document = await _database.Documents
                .Include(p => p.RelatedParentSubcategories)
                    .ThenInclude(p => p.Subcategory)
                        .ThenInclude(p => p.ParentCategory)
                .Include(p => p.DocumentBlobs)
                .SingleOrDefaultAsync(p => p.Id == documentToUpdate.Id, ct);

            var response = _mapper.Map<DocumentDto>(document);

            response.ParentSubcategories = document.RelatedParentSubcategories
                .Select(p => p.Subcategory)
                .Select(_mapper.Map<SubcategoryDto>).ToArray();

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


        [HttpDelete("{documentId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> DeleteDocument(
            [FromRoute] long documentId,
            CancellationToken ct)
        {
            var documentToRemove = await _database.Documents
                .Include(p => p.DocumentBlobs)
                .SingleOrDefaultAsync(p => p.Id == documentId && !p.IsDeleted, ct) ??
                throw new ResourceNotFoundException($"Документ с ID {documentId} не найден!");

            _database.DocumentBlobs.RemoveRange(documentToRemove.DocumentBlobs);

            var documentCategoryRelationsToRemove = await _database
                .DocumentSubcategoryRelations
                .Where(p => p.DocumentId == documentToRemove.Id)
                .ToArrayAsync(ct);

            _database.DocumentSubcategoryRelations.RemoveRange(documentCategoryRelationsToRemove);
            _database.Documents.Remove(documentToRemove);

            await _database.SaveChangesAsync(ct);

            return Ok(ApiResponse.Ok());
        }
    }
}
