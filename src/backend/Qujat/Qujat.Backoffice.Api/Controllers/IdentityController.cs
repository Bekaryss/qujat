using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Qujat.Core.Data;
using Qujat.Core.Data.Entities;
using Qujat.Core.Exceptions;
using Qujat.Core.Services;
using Qujat.Core.DTOs.Shared;
using Qujat.Backoffice.Api.Models;
using AutoMapper;

namespace Qujat.Backoffice.Api.Controllers
{
    [ApiController]
    [Route("api/1/identity")]
    [AuthorizeByAccessToken]
    public class IdentityController(ApplicationDbContext database, IMapper mapper) : ControllerBase
    {
        private readonly ApplicationDbContext _database = database;
        private readonly IMapper _mapper = mapper;

        [AuthorizeByAccessToken]
        [HttpGet("me")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<AdminDto>), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> GetMe(
            [FromServices] ICurrentUserProvider currentUserProvider,
            CancellationToken ct)
        {
            var currentUser = currentUserProvider.GetCurrentUser();
            var user = await _database.Users.SingleOrDefaultAsync(p => p.Id == currentUser.UserId, ct);

            var response = _mapper.Map<AdminDto>(user);
            return Ok(ApiResponse<AdminDto>.Ok(response));
        }


        [AllowAnonymous]
        [HttpPost("sign-in")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<SignInAdminUserRp>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> SignInAdminUser(
            [FromBody] SignInAdminUserRq rq,
            [FromServices] UserManager<ApplicationUserEntity> userManager,
            [FromServices] IAccessTokenGenerator accessTokenGenerator,
            [FromServices] ApplicationDbContext database,
            CancellationToken ct)
        {
            if (rq == null) throw new EmptyRqBodyException();

            var user = await database.Users
                .SingleOrDefaultAsync(p => p.Email == rq.Email, ct)
                ?? throw new BadRequestException("Указанная вами комбинация email и пароля не найдена.");

            //if (user.AccessFailedCount > 5)
            //    throw new BadRequestException("Слишком много попыток входа, обратитесь в службу ТП");

            var checkPasswordResult = await userManager.CheckPasswordAsync(user, rq.Password);

            if (!checkPasswordResult)
            {
                user.AccessFailedCount++;
                _database.Users.Update(user);
                await _database.SaveChangesAsync(ct);

                throw new BadRequestException("Указанная вами комбинация email и пароля не найдена.");
            }

            var accessToken = await accessTokenGenerator.GenerateAccessToken(user.Id);

            var wrappedResponse = ApiResponse<SignInAdminUserRp>.Ok(
                new SignInAdminUserRp { AccessToken = accessToken.Token });

            return Ok(wrappedResponse);
        }
    }
}
