using Microsoft.AspNetCore.Mvc;
using Qujat.Core.Data.Entities;
using Qujat.Core.Data;
using Qujat.Core.DTOs.Shared;
using Qujat.Core.Exceptions;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Qujat.Backoffice.Api.Models;
using AutoMapper;

namespace Qujat.Backoffice.Api.Controllers
{
    [ApiController]
    [Route("api/1/categories")]
    [AuthorizeByAccessToken]
    public class CategoryController(ApplicationDbContext database, IMapper mapper) : ControllerBase
    {
        private readonly ApplicationDbContext _database = database;
        private readonly IMapper _mapper = mapper;

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> AddCategory([FromBody] CreateCategoryRq rq, CancellationToken ct)
        {
            if (rq == null) throw new EmptyRqBodyException();

            var lastDisplayOrder = await _database.Categories
                .Select(p => p.DisplayOrder)
                .Concat(new[] { 0 })
                .MaxAsync(p => p, ct);

            var categoryToCreate = _mapper.Map<CategoryEntity>(rq);
            categoryToCreate.DisplayOrder = ++lastDisplayOrder;

            var iconBlob = await _database.Icons
                .SingleOrDefaultAsync(p => p.Id == rq.IconBlobId, ct);

            categoryToCreate.IconBlobId = iconBlob?.Id ?? null;
            _database.Categories.Add(categoryToCreate);

            await _database.SaveChangesAsync(ct);

            var category = await _database.Categories
                .AsNoTracking()
                .Include(p => p.IconBlob)
                .SingleOrDefaultAsync(p => p.Id == categoryToCreate.Id, ct);

            var categoryDto = _mapper.Map<CategoryDto>(category);

            var response = ApiResponse<CategoryDto>.Ok(categoryDto);
            return Ok(response);
        }


        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto[]>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetCategories(
            [FromQuery(Name = "pageIndex")] int pageIndex = 0,
            [FromQuery(Name = "pageSize")] int pageSize = 10,
            CancellationToken ct = default)
        {
            if(pageSize == 0) { pageSize = 10000; }

            var categoriesFromDb = await _database.Categories
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

            var response = ApiResponse<CategoryDto[]>.Ok(categories, pageIndex, pageSize, totalCategoriesCount);
            return Ok(response);
        }


        [HttpGet("{categoryId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetCategoryById(
            [FromRoute] long categoryId,
            CancellationToken ct = default)
        {
            var category = await _database.Categories
                .Include(p => p.IconBlob)
                .SingleOrDefaultAsync(p => !p.IsDeleted && p.Id == categoryId, ct) ?? 
                throw new ResourceNotFoundException();

            var response = _mapper.Map<CategoryDto>(category);

            var apiResponse = ApiResponse<CategoryDto>.Ok(response);
            return Ok(apiResponse);
        }

        [HttpPatch("display-order")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse), (int) HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateCategoryDisplayOrders(
            [FromBody] UpdateCategoryDisplayOrdersRq rq, CancellationToken ct)
        {
            if (rq == null) throw new EmptyRqBodyException();

            var categoriesFromDb = await _database.Categories
                .Where(p => !p.IsDeleted)
                .ToArrayAsync(ct);

            var displayOrderCounter = 1;
            foreach (var id in rq.CategoryIdsByDisplayOrder)
            {
                var categoryFromDb = categoriesFromDb.SingleOrDefault(p => p.Id == id) ?? 
                    throw new ResourceNotFoundException();

                categoryFromDb.DisplayOrder = displayOrderCounter;
                displayOrderCounter++;
            }

            var unspecifiedCategories = categoriesFromDb
                .Where(p => !rq.CategoryIdsByDisplayOrder.Contains(p.Id))
                .OrderBy(p => p.DisplayOrder)
                .ToArray();

            foreach (var item in unspecifiedCategories)
            {
                item.DisplayOrder = displayOrderCounter;
                displayOrderCounter++;
            }

            _database.Categories.UpdateRange(categoriesFromDb);
            await _database.SaveChangesAsync(ct);

            var apiResposne = ApiResponse.Ok();
            return Ok(apiResposne);
        }


        [HttpPatch("{categoryId:long?}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> UpdateCategory(
            [FromRoute] long categoryId,
            [FromBody] UpdateCategoryRq rq, CancellationToken ct)
        {
            if (rq == null) throw new EmptyRqBodyException();

            var categoryExist = await _database.Categories
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .AnyAsync(p => p.Id == categoryId, ct);

            if (!categoryExist)
                throw new ResourceNotFoundException();

            if(rq.IconBlobId != null)
            {
                var iconBlobExist = await _database.Icons
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted)
                    .AnyAsync(p => p.Id == rq.IconBlobId, ct);

                if (!iconBlobExist)
                    throw new ResourceNotFoundException();
            }

            var categoryToUpdate = _mapper.Map<CategoryEntity>(rq);
            categoryToUpdate.Id = categoryId;

            _database.Categories.Update(categoryToUpdate);
            await _database.SaveChangesAsync(ct);

            var category = await _database.Categories
                .AsNoTracking()
                .Include(p => p.IconBlob)
                .SingleOrDefaultAsync(p => p.Id == categoryToUpdate.Id, ct);

            var categoryDto = _mapper.Map<CategoryDto>(category);
            var response = ApiResponse<CategoryDto>.Ok(categoryDto);

            return Ok(response);
        }


        [HttpDelete("{categoryId:long?}")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> DeleteCategory(
            [FromRoute] long categoryId, CancellationToken ct)
        {
            var categoryToDelete = await _database
                .Categories
                .SingleOrDefaultAsync(p => p.Id == categoryId, ct) ??
                throw new ResourceNotFoundException();

            var anySubcategoryExist = await _database.Subcategories.AnyAsync(
                p => p.ParentCategoryId == categoryId, ct);

            if (anySubcategoryExist)
                throw new BadRequestException(
                    "Нельзя удалить категорию, пока существует хотя бы 1 подкатегория внутри!");

            _database.Categories.Remove(categoryToDelete);
            await _database.SaveChangesAsync(ct);

            var response = ApiResponse.Ok();
            return Ok(response);
        }
    }
}
