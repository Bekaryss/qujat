using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qujat.Core.Data;
using Qujat.Core.Exceptions;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Qujat.Core.DTOs.Shared;
using Qujat.Core.Data.Entities;
using System.Linq;
using Qujat.Backoffice.Api.Models;
using AutoMapper;

namespace Qujat.Backoffice.Api.Controllers
{
    [ApiController]
    [Route("api/1/links")]
    [AuthorizeByAccessToken]
    public class LinkController(ApplicationDbContext database,
        IMapper mapper) : 
        ControllerBase
    {
        private readonly ApplicationDbContext _database = database;
        private readonly IMapper _mapper = mapper;

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<LinkDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> AddLink(
            [FromBody] CreateLinkRq rq, CancellationToken ct)
        {
            if (rq == null) throw new EmptyRqBodyException();

            var linkToCreate = _mapper.Map<LinkEntity>(rq);

            _database.Links.Add(linkToCreate);

            await _database.SaveChangesAsync(ct);

            var linkDto = _mapper.Map<LinkDto>(linkToCreate);

            var response = ApiResponse<LinkDto>.Ok(linkDto);
            return Ok(response);
        }


        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<LinkDto[]>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetLinks(
            [FromQuery(Name = "pageIndex")] int pageIndex = 0,
            [FromQuery(Name = "pageSize")] int pageSize = 10,
            CancellationToken ct = default)
        {
            var linksFromDb = await _database
                .Links
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.NameKz)
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

        [HttpGet("{id:long?}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<LinkDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetLinkById(
            [FromRoute] long id, CancellationToken ct)
        {
            var linkFromDb = await _database
                .Links
                .SingleOrDefaultAsync(p => p.Id == id, ct) ??
                throw new ResourceNotFoundException();

            var linkDto = _mapper.Map<LinkDto>(linkFromDb);

            var response = ApiResponse<LinkDto>.Ok(linkDto);
            return Ok(response);
        }

        [HttpPatch("{id:long?}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<LinkDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> UpdateLink(
            [FromRoute] long id,
            [FromBody] UpdateLinkRq rq, CancellationToken ct)
        {
            if (rq == null) throw new EmptyRqBodyException();

            var linkToUpdate = await _database
                .Links
                .SingleOrDefaultAsync(p => p.Id == id, ct) ??
                throw new ResourceNotFoundException();

            linkToUpdate.NameKz = rq.NameKz;
            linkToUpdate.NameRu = rq.NameRu;
            linkToUpdate.Uri = rq.Uri;

            _database.Links.Update(linkToUpdate);
            await _database.SaveChangesAsync(ct);

            var linkDto = _mapper.Map<LinkDto>(linkToUpdate);

            var response = ApiResponse<LinkDto>.Ok(linkDto);
            return Ok(response);
        }


        [HttpDelete("{id:long?}")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> DeleteLink(
            [FromRoute] long id, CancellationToken ct)
        {
            var linkToDelete = await _database
                .Links
                .SingleOrDefaultAsync(p => p.Id == id, ct) ??
                throw new ResourceNotFoundException("LinkNotFound");

            _database.Links.Remove(linkToDelete);
            await _database.SaveChangesAsync(ct);

            var response = ApiResponse.Ok();
            return Ok(response);
        }
    }
}
