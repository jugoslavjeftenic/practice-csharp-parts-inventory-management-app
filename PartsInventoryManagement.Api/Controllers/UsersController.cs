using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartsInventoryManagement.Api.Data;
using PartsInventoryManagement.Api.Dtos;
using PartsInventoryManagement.Api.Helpers;
using PartsInventoryManagement.Api.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;

namespace PartsInventoryManagement.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class UsersController(IConfiguration config) : ControllerBase
	{
		private readonly DbContextDapper _dapper = new(config);
		private readonly AuthHelper _authHelper = new(config);

		// Create
		[HttpPost]
		public IActionResult AddPart(NewUserDTO userDto)
		{
			if (userDto.Password.Equals(userDto.PasswordConfirm) is not true)
			{
				return BadRequest("Lozinke se ne podudaraju.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@UserNameParam", userDto.UserName, DbType.String);
			sqlParameters.Add("@LocationIdParam", userDto.LocationId, DbType.Int32);
			sqlParameters.Add("@PasswordParam", userDto.Password, DbType.String);
			sqlParameters.Add("@PasswordConfirmParam", userDto.PasswordConfirm, DbType.String);

			// Query Db for user
			string sqlUsersQueryDb = @$"
				SELECT *
				FROM [dbo].[Users]
				WHERE [UserName] = @UserNameParam
				";

			IEnumerable<UserModel> usersQueryDb =
				_dapper.QuerySql<UserModel>(sqlUsersQueryDb, sqlParameters);

			if (usersQueryDb.Any())
			{
				return BadRequest("Korisnik već postoji.");
			}

			// Query Db for location
			if (userDto.LocationId.Equals(0) is not true)
			{
				string sqlLocationsQueryDb = @$"
				SELECT *
				FROM [dbo].[Locations]
				WHERE [LocationId] = @LocationIdParam
				";

				IEnumerable<LocationModel> locationsQueryDb =
					_dapper.QuerySql<LocationModel>(sqlLocationsQueryDb, sqlParameters);

				if (locationsQueryDb.Any() is not true)
				{
					return BadRequest("Nema tražene lokacije.");
				}
			}

			// Hash password
			byte[] passwordSalt = [128 / 8];

			using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
			{
				rng.GetNonZeroBytes(passwordSalt);
			}

			byte[] passwordHash = _authHelper.GetPasswordHash(userDto.Password, passwordSalt);

			sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
			sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);

			// Insert user
			string sql = @$"
				INSERT INTO [dbo].[Users] (
					[UserName],
					[LocationId],
					[PasswordHash],
					[PasswordSalt]
				) VALUES (
					@UserNameParam,
					@LocationIdParam,
					@PasswordHashParam,
					@PasswordSaltParam
				)";

			if (_dapper.ExecuteSql(sql, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom dodavanja korisnika.");
			}

			return Ok(userDto);
		}
	}
}
