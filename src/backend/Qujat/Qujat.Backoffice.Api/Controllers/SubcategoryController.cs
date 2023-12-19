using Microsoft.AspNetCore.Mvc;
using Qujat.Core.Data.Entities;
using Qujat.Core.Data;
using Qujat.Core.Exceptions;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Qujat.Core.DTOs.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Qujat.Backoffice.Api.Models;
using AutoMapper;

namespace Qujat.Backoffice.Api.Controllers
{
    [ApiController]
    [Route("api/1/categories/{categoryId:long}/subcategories")]
    [AuthorizeByAccessToken]
    public class SubcategoryController(ApplicationDbContext database, IMapper mapper) : ControllerBase
    {
        private readonly ApplicationDbContext _database = database;
        private readonly IMapper _mapper = mapper;

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<SubcategoryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> AddSubcategory(
            [FromRoute] long categoryId,
            [FromBody] CreateSubcategoryRq rq, CancellationToken ct)
        {
            if (rq == null) throw new EmptyRqBodyException();

            var parentCategoryExist = await _database.Categories
                .AnyAsync(p => p.Id == categoryId && !p.IsDeleted, ct);

            if (!parentCategoryExist) throw new ResourceNotFoundException();

            var lastDisplayOrder = await _database.Subcategories
                .Where(p => p.ParentCategoryId == categoryId && !p.IsDeleted)
                .Select(p => p.DisplayOrder)
                .Concat(new[] { 0 })
                .MaxAsync(p => p, ct);

            var subcategoryToCreate = _mapper.Map<SubcategoryEntity>(rq);
            subcategoryToCreate.DisplayOrder = ++lastDisplayOrder;
            subcategoryToCreate.ParentCategoryId = categoryId;

            _database.Subcategories.Add(subcategoryToCreate);

            await _database.SaveChangesAsync(ct);

            var subcategory = await _database.Subcategories
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == subcategoryToCreate.Id, ct);

            var subcategoryDto = _mapper.Map<SubcategoryDto>(subcategory);

            var response = ApiResponse<SubcategoryDto>.Ok(subcategoryDto);
            return Ok(response);
        }


        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<SubcategoryDto[]>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetSubcategories(
            [FromRoute] long categoryId,
            [FromQuery(Name = "pageIndex")] int pageIndex = 0,
            [FromQuery(Name = "pageSize")] int pageSize = 10,
            CancellationToken ct = default)
        {
            if (pageSize == 0) { pageSize = 10000; }

            var subcategoriesFromDb = await _database.Subcategories
                .Where(p => !p.IsDeleted && p.ParentCategoryId == categoryId)
                .OrderBy(p => p.DisplayOrder)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToArrayAsync(ct);

            var totalSubcategoriesCount = await _database
                .Subcategories
                .Where(p => !p.IsDeleted && p.ParentCategoryId == categoryId)
                .CountAsync(ct);

            var subcategories = subcategoriesFromDb
                .Select(_mapper.Map<SubcategoryDto>)
                .ToArray();

            var response = ApiResponse<SubcategoryDto[]>.Ok(subcategories, pageIndex, pageSize, totalSubcategoriesCount);
            return Ok(response);
        }


        [HttpGet("{subcategoryId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<SubcategoryDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetSubcategoryById(
            [FromRoute] long categoryId,
            [FromRoute] long subcategoryId,
            CancellationToken ct = default)
        {
            var parentCategoryExist = await _database.Categories
                .AnyAsync(p => p.Id == categoryId && !p.IsDeleted, ct);

            if (!parentCategoryExist)
                throw new ResourceNotFoundException();

            var subcategoryFromDb = await _database
                .Subcategories
                .SingleOrDefaultAsync(p => p.Id == subcategoryId, ct);

            if (subcategoryFromDb.ParentCategoryId != categoryId)
                throw new ResourceNotFoundException();

            var subcategoryDto = _mapper.Map<SubcategoryDto>(subcategoryFromDb);

            var response = ApiResponse<SubcategoryDto>.Ok(subcategoryDto);
            return Ok(response);
        }


        [HttpPatch("display-order")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateSubcategoryDisplayOrders(
            [FromRoute] long categoryId,
            [FromBody] UpdateSubcategoryDisplayOrdersRq rq, CancellationToken ct)
        {
            if (rq == null) throw new EmptyRqBodyException();

            var subcategoriesFromDb = await _database.Subcategories
                .Where(p => !p.IsDeleted && p.ParentCategoryId == categoryId)
                .ToArrayAsync(ct);

            var displayOrderCounter = 1;
            foreach (var id in rq.SubcategoryIdsByDisplayOrder)
            {
                var subcategoryFromDb = subcategoriesFromDb.SingleOrDefault(p => p.Id == id) ??
                    throw new ResourceNotFoundException();

                subcategoryFromDb.DisplayOrder = displayOrderCounter;
                displayOrderCounter++;
            }

            var unspecifiedCategories = subcategoriesFromDb
                .Where(p => !rq.SubcategoryIdsByDisplayOrder.Contains(p.Id))
                .OrderBy(p => p.DisplayOrder)
                .ToArray();

            foreach (var item in unspecifiedCategories)
            {
                item.DisplayOrder = displayOrderCounter;
                displayOrderCounter++;
            }

            _database.Subcategories.UpdateRange(subcategoriesFromDb);
            await _database.SaveChangesAsync(ct);

            var apiResposne = ApiResponse.Ok();
            return Ok(apiResposne);
        }


        [HttpPatch("{subcategoryId:long?}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<SubcategoryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> UpdateSubcategory(
            [FromRoute] long categoryId,
            [FromRoute] long subcategoryId,
            [FromBody] UpdateSubcategoryRq rq, CancellationToken ct)
        {
            if (rq == null) throw new EmptyRqBodyException();

            var subcategoryExist = await _database.Subcategories
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .AnyAsync(p => p.Id == subcategoryId && p.ParentCategoryId == categoryId, ct);

            if (!subcategoryExist)
                throw new ResourceNotFoundException();

            var parentCategory = await _database.Categories
                .SingleOrDefaultAsync(p => p.Id == categoryId, ct) ?? 
                throw new ResourceNotFoundException();

            var subcategoryToUpdate = _mapper.Map<SubcategoryEntity>(rq);
            subcategoryToUpdate.Id = subcategoryId;
            subcategoryToUpdate.ParentCategoryId = parentCategory.Id;

            _database.Subcategories.Update(subcategoryToUpdate);
            await _database.SaveChangesAsync(ct);

            var subcategory = await _database.Subcategories
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == subcategoryToUpdate.Id, ct);

            var subcategoryDto = _mapper.Map<SubcategoryDto>(subcategory);
            var response = ApiResponse<SubcategoryDto>.Ok(subcategoryDto);

            return Ok(response);
        }


        [HttpDelete("{subcategoryId:long?}")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> DeleteSubcategory(
            [FromRoute] long categoryId,
            [FromRoute] long subcategoryId, 
            CancellationToken ct)
        {
            var parentCategoryExist = await _database.Categories
                .AnyAsync(p => p.Id == categoryId && !p.IsDeleted, ct);

            if (!parentCategoryExist)
                throw new ResourceNotFoundException();

            var subcategoryToDelete = await _database
                .Subcategories
                .SingleOrDefaultAsync(p => p.Id == subcategoryId, ct);

            if (subcategoryToDelete.ParentCategoryId != categoryId)
                throw new ResourceNotFoundException();

            var anyDocumentExist = await _database.DocumentSubcategoryRelations
                .AnyAsync(p => p.SubcategoryId == subcategoryId, ct);

            if (anyDocumentExist)
                throw new BadRequestException("Нельзя удалить подкатегорию, " +
                    "пока существует хотя бы 1 документ внутри");

            _database.Subcategories.Remove(subcategoryToDelete);
            await _database.SaveChangesAsync(ct);
            
            var response = ApiResponse.Ok();
            return Ok(response);
        }
    }
}
