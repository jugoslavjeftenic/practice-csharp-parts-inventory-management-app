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
		public IActionResult AddUser(NewUserDTO userDto)
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

			IEnumerable<GetUsersDTO> usersQueryDb =
				_dapper.QuerySql<GetUsersDTO>(sqlUsersQueryDb, sqlParameters);

			if (usersQueryDb.Any())
			{
				return BadRequest("Korisnik već postoji.");
			}

			// Query Db for location
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

			// Hash password
			var (passwordHash, passwordSalt) = _authHelper.HashPassword(userDto.Password);
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

		// Read
		[HttpGet]
		public IActionResult GetUsers()
		{
			string sql = @$"
				SELECT
					[UserId],
					[UserName],
					[LocationId]
				FROM [dbo].[Users]";

			IEnumerable<GetUsersDTO> users = _dapper.QuerySql<GetUsersDTO>(sql);

			return Ok(users);
		}

		// Update
		[HttpPut]
		public IActionResult EditUser(EditUserDTO editUserDTO)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@UserIdParam", editUserDTO.UserId, DbType.Int32);
			sqlParameters.Add("@UserNameParam", editUserDTO.UserName, DbType.String);
			sqlParameters.Add("@LocationIdParam", editUserDTO.LocationId, DbType.Int32);

			if (editUserDTO.Password.Equals(editUserDTO.PasswordConfirm) is not true)
			{
				return BadRequest("Lozinke se ne podudaraju.");
			}

			// Query Db for user
			string sqlUsersQueryDb = @$"
				SELECT *
				FROM [dbo].[Users]
				WHERE [UserId] = @UserIdParam
				";

			IEnumerable<GetUsersDTO> usersQueryDb =
				_dapper.QuerySql<GetUsersDTO>(sqlUsersQueryDb, sqlParameters);

			if (usersQueryDb.Any() is not true)
			{
				return BadRequest("Nema traženog korisnika.");
			}

			// Query Db for location
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

			// Update user
			var (passwordHash, passwordSalt) = _authHelper.HashPassword(editUserDTO.Password);
			sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
			sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);

			string sql = @$"
				UPDATE [dbo].[Users]
				SET
					[UserName] = @UserNameParam,
					[LocationId] = @LocationIdParam,
					[PasswordHash] = @PasswordHashParam,
					[PasswordSalt] = @PasswordSaltParam
				WHERE [UserId] = @UserIdParam
				";

			if (_dapper.ExecuteSql(sql, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom izmene korisnika.");
			}

			return Ok(editUserDTO);
		}

		// Delete
		[HttpDelete("{userId:int}")]
		public IActionResult DeleteUser(int userId)
		{
			if (userId < 1)
			{
				return BadRequest($"Id korisnika mora da bude veći od 0.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@UserIdParam", userId, DbType.Int32);

			// Query Db for part
			string sqlUserQueryDb = @$"
				SELECT *
				FROM [dbo].[Users]
				WHERE [UserId] = @UserIdParam
				";

			IEnumerable<GetUsersDTO> usersQueryDb =
				_dapper.QuerySql<GetUsersDTO>(sqlUserQueryDb, sqlParameters);

			if (usersQueryDb.Any() is not true)
			{
				return BadRequest("Nema traženog korisnika.");
			}

			// Delete user
			string sql = @$"
				DELETE
				FROM [dbo].[Users]
				WHERE [UserId] = @UserIdParam
				";

			if (_dapper.ExecuteSql(sql, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom brisanja korisnika.");
			}

			return Ok(usersQueryDb);
		}
	}
}
