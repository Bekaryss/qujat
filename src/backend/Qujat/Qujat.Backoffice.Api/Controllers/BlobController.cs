using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qujat.Core.Data;
using Qujat.Core.DTOs.Shared;
using Qujat.Core.Exceptions;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Qujat.Core.Data.Entities;
using Qujat.Core.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Qujat.Backoffice.Api.Models;
using AutoMapper;

namespace Qujat.Backoffice.Api.Controllers
{
    [ApiController]
    [Route("api/1/blobs")]
    [AuthorizeByAccessToken]
    public class BlobController(ExternalS3BlobService blobService, ApplicationDbContext database,
        IMapper mapper) : ControllerBase
    {
        private readonly ExternalS3BlobService _blobService = blobService;
        private readonly ApplicationDbContext _database = database;
        private readonly IMapper _mapper = mapper;

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<BlobDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<BlobDto>), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateBlob(IFormFile rq, CancellationToken ct)
        {
            if (rq == null) throw new EmptyRqBodyException();

            var allowedBlobExtensions = new string[] { ".docx", ".pdf" };

            if (!allowedBlobExtensions.Contains(Path.GetExtension(rq.FileName)))
                throw new BadRequestException($"Allowed extensions are {string.Join(',', allowedBlobExtensions)} only");

            using var contentStream = new MemoryStream();
            await rq.CopyToAsync(contentStream, ct);
            contentStream.Position = 0;

            var blobUri = await _blobService.UploadBlob(contentStream, rq.FileName, ct);

            var blobToAdd = new BlobEntity 
            { 
                Uri = blobUri.ToString(), 
                Extension = Path.GetExtension(rq.FileName), 
                MimeType = MimeMapping.MimeUtility.GetMimeMapping(rq.FileName),
                FileName = rq.FileName,
            };

            _database.Blobs.Add(blobToAdd);
            await _database.SaveChangesAsync(ct);

            var blob = _mapper.Map<BlobDto>(blobToAdd);
            var response = ApiResponse<BlobDto>.Ok(blob);
            return Ok(response);
        }


        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<BlobDto[]>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<BlobDto[]>), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetBlobs(
            [FromQuery(Name = "pageIndex")] int pageIndex = 0,
            [FromQuery(Name = "pageSize")] int pageSize = 10,
            CancellationToken ct = default)
        {
            var blobs = await _database.Blobs
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToArrayAsync(ct);

            var blobsCount = await _database.Blobs
                .Where(p => !p.IsDeleted)
                .CountAsync(ct);

            var response = blobs
                .Select(_mapper.Map<BlobDto>)
                .ToArray();

            var apiResponse = ApiResponse<BlobDto[]>.Ok(response, pageIndex, pageSize, blobsCount);
            return Ok(apiResponse);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{blobId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<BlobDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<BlobDto>), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetBlobContentAsStream([FromRoute] long blobId, CancellationToken ct)
        {
            var blob = await _database.Blobs.SingleOrDefaultAsync(p => p.Id == blobId, ct) ?? 
                throw new ResourceNotFoundException();
            
            using var stream = new MemoryStream(blob.Content);
            return File(stream, "application/octet-stream", blob.FileName);
        }
    }
}
