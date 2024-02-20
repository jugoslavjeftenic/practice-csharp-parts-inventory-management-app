using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartsInventoryManagement.Api.Data;
using PartsInventoryManagement.Api.Dtos;
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

		// Create
		[HttpPost]
		public IActionResult AddPart(NewUserDTO userDto)
		{
			if (userDto.Password.Equals(userDto.PasswordConfirm))
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

			// TODO: Nastaviti ovde
			// Query Db for part
			string sqlPartsQueryDb = @$"
				SELECT *
				FROM [dbo].[Parts]
				WHERE [PartName] = @PartNameParam
				";

			IEnumerable<PartModel> partsQueryDb =
				_dapper.QuerySql<PartModel>(sqlPartsQueryDb, sqlParameters);

			if (partsQueryDb.Any())
			{
				return BadRequest("Deo već postoji.");
			}

			// Insert part
			string sql = @$"
				INSERT INTO [dbo].[Parts] (
					[PartCategoryId],
					[PartName]
				) VALUES (
					@PartCategoryIdParam,
					@PartNameParam
				)";

			if (_dapper.ExecuteSql(sql, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom dodavanja dela.");
			}

			return Ok(userDto);
		}
	}
}
