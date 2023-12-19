using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qujat.Backoffice.Api.Models;
using Qujat.Core.Data;
using Qujat.Core.Data.Entities;
using Qujat.Core.DTOs.Shared;
using Qujat.Core.Exceptions;
using Qujat.Core.Services;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Qujat.Backoffice.Api.Controllers
{
    [ApiController]
    [Route("api/1/application-users")]
    [AuthorizeByAccessToken]
    public class ApplicationUserController(UserManager<ApplicationUserEntity> userManager,
        ApplicationDbContext database, IMapper mapper) : ControllerBase
    {
        private readonly UserManager<ApplicationUserEntity> _userManager = userManager;
        private readonly ApplicationDbContext _database = database;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<AdminDto[]>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetApplicationUsers(
            [FromQuery(Name = "pageIndex")] int pageIndex = 0,
            [FromQuery(Name = "pageSize")] int pageSize = 10,
            CancellationToken ct = default)
        {
            var applicationUsersFromDb = await _database
                .Users
                .Where(x => !x.IsDeleted)
                .OrderBy(p => p.Id)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToArrayAsync(ct);

            var applicationUsersFromDbCount = await _database
                .Users
                .Where(x => !x.IsDeleted)
                .CountAsync(ct);

            var applicationUsers = applicationUsersFromDb
                .Select(_mapper.Map<AdminDto>)
                .ToArray();

            var response = ApiResponse<AdminDto[]>.Ok(applicationUsers,
                pageIndex, pageSize, applicationUsersFromDbCount);
            return Ok(response);
        }


        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<AdminDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> CreateApplicationUser(
            [FromBody] SignUpAdminRq rq,
            [FromServices] ICurrentUserProvider currentUserProvider,
            CancellationToken ct)
        {
            var currentUser = currentUserProvider.GetCurrentUser();
            var user = await _database.Users.SingleOrDefaultAsync(p => p.Id == currentUser.UserId, ct);

            if (user.UserType != ApplicationUserType.RootAdmin)
                throw new BadRequestException("Только корневой администратор может создавать новых администраторов");

            if (rq == null) throw new EmptyRqBodyException();

            var emailAlreadyUsed = await _database.Users.AnyAsync(x => x.Email == rq.Email, ct);
            if (emailAlreadyUsed)
                throw new BadRequestException("Email уже используется");

            var password = rq.Password;
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                throw new BadRequestException("Длина пароля должна быть 8 или более символов!");

            bool meetCriteria = false;
            if (password.Any(char.IsUpper) &&
                password.Any(char.IsLower) &&
                password.Any(char.IsDigit))
            {
                meetCriteria = true;
            }

            if (!meetCriteria)
            {
                throw new BadRequestException("Пароль должен содержать как минимум 1 заглавный символ, 1 цифру и 1 специальный символ!");
            }

            var applicationUserToCreate = new ApplicationUserEntity
            {
                Email = rq.Email,
                UserName = rq.Email,
                FullName = rq.Email,
                UserType = ApplicationUserType.Admin,
            };

            var createApplicationUserResult = await _userManager.CreateAsync(applicationUserToCreate, rq.Password);

            if (!createApplicationUserResult.Succeeded)
                throw new BadRequestException("Что-то пошло не так!");

            applicationUserToCreate.PhoneNumberConfirmed = true;
            applicationUserToCreate.EmailConfirmed = true;
            applicationUserToCreate.IsAccountActivatedByUser = false;

            var response = _mapper.Map<AdminDto>(applicationUserToCreate);

            var apiResponse = ApiResponse<AdminDto>.Ok(response);
            return Ok(apiResponse);
        }


        [HttpPatch("{userId:long}/password")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<AdminDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> ResetPasswordForAdmin(
            [FromRoute] long userId,
            [FromBody] ResetPasswordForAdminRq rq,
            [FromServices] ICurrentUserProvider currentUserProvider,
            CancellationToken ct)
        {
            if (rq == null) throw new EmptyRqBodyException();

            var currentUser = currentUserProvider.GetCurrentUser();

            var currentUserFromDb = await _database.Users
                .SingleOrDefaultAsync(p => p.Id == currentUser.UserId, ct);

            if (currentUserFromDb.UserType != ApplicationUserType.RootAdmin)
                throw new BadRequestException("Операция доступна только корневому администратору!");

            var userToUpdate = await _database.Users.SingleOrDefaultAsync(p => p.Id == userId, ct) ??
                throw new ResourceNotFoundException();

            await _userManager.RemovePasswordAsync(userToUpdate);
            var opResult = await _userManager.AddPasswordAsync(userToUpdate, rq.Password);

            if (!opResult.Succeeded)
                throw new BadRequestException();

            userToUpdate.AccessFailedCount = 0;
            _database.Users.Update(userToUpdate);
            await _database.SaveChangesAsync(ct);

            var response = _mapper.Map<AdminDto>(userToUpdate);

            var apiResponse = ApiResponse<AdminDto>.Ok(response);
            return Ok(apiResponse);
        }


        [HttpDelete("{userId:long}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteUserAsync(
            [FromRoute] long userId,
            [FromServices] ICurrentUserProvider currentUserProvider,
            CancellationToken ct)
        {
            var currentUser = currentUserProvider.GetCurrentUser();

            var currentUserFromDb = await _database.Users
                .SingleOrDefaultAsync(p => p.Id == currentUser.UserId, ct);

            if (currentUserFromDb.UserType != ApplicationUserType.RootAdmin)
                throw new BadRequestException("Операция доступна только корневому администратору!");

            var userToRemove = await _database.Users.SingleOrDefaultAsync(p => p.Id == userId, ct) ??
                throw new ResourceNotFoundException();

            if (userToRemove.Id == currentUser.UserId || userToRemove.UserType == ApplicationUserType.RootAdmin)
                throw new BadRequestException("Нельзя удалить самого себя или другого корневого администратора!");

            var opResult = await _userManager.DeleteAsync(userToRemove);

            if (!opResult.Succeeded)
                throw new BadRequestException();

            var apiResponse = ApiResponse.Ok();
            return Ok(apiResponse);
        }
    }
}
