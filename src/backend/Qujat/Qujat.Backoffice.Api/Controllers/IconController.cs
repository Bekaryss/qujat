using Microsoft.AspNetCore.Mvc;
using Qujat.Core.Data;
using Qujat.Core.Exceptions;
using Qujat.Core.Services;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Qujat.Core.DTOs.Shared;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Qujat.Backoffice.Api.Models;
using AutoMapper;

namespace Qujat.Backoffice.Api.Controllers
{
    [ApiController]
    [Route("api/1/icons")]
    [AuthorizeByAccessToken]
    public class IconController(ExternalS3BlobService iconService, ApplicationDbContext database,
        IMapper mapper) : ControllerBase
    {
        private readonly ExternalS3BlobService _blobService = iconService;
        private readonly ApplicationDbContext _database = database;
        private readonly IMapper _mapper = mapper;

        [HttpGet("{iconId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<BlobDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetIconById([FromRoute] long iconId, CancellationToken ct)
        {
            var iconFromDb = await _database.Icons
                .SingleOrDefaultAsync(p => p.Id == iconId, ct) ??
                throw new ResourceNotFoundException();

            var icon = _mapper.Map<BlobDto>(iconFromDb);
            var response = ApiResponse<BlobDto>.Ok(icon);

            return Ok(response);
        }


        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<BlobDto[]>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<BlobDto[]>), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetIcons(
            [FromQuery(Name = "pageIndex")] int pageIndex = 0,
            [FromQuery(Name = "pageSize")] int pageSize = 10,
            CancellationToken ct = default)
        {
            if (pageSize == 0) { pageSize = 10000; }

            var icons = await _database.Icons
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToArrayAsync(ct);

            var iconsCount = await _database.Icons
                .Where(p => !p.IsDeleted)
                .CountAsync(ct);

            var response = icons
                .Select(_mapper.Map<BlobDto>)
                .ToArray();

            var apiResponse = ApiResponse<BlobDto[]>.Ok(response, pageIndex, pageSize, iconsCount);
            return Ok(apiResponse);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{iconId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<BlobDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<BlobDto>), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetIconContentAsStream([FromRoute] long iconId, CancellationToken ct)
        {
            var Icon = await _database.Icons.SingleOrDefaultAsync(p => p.Id == iconId, ct) ??
                throw new ResourceNotFoundException();

            using var stream = new MemoryStream(Icon.Content);
            return File(stream, "application/octet-stream", "{{filename.ext}}");
        }
    }
}
