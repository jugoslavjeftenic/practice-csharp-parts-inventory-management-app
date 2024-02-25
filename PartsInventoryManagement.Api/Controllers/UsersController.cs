using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PartsInventoryManagement.Api.Data;
using PartsInventoryManagement.Api.Dtos;
using PartsInventoryManagement.Api.Helpers;
using PartsInventoryManagement.Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PartsInventoryManagement.Api.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
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
				return BadRequest("Potvrda lozinke se ne podudara.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@UserNameParam", userDto.UserName, DbType.String);
			sqlParameters.Add("@LocationIdParam", userDto.LocationId, DbType.Int32);

			// Query users
			string sqlQueryUsers = @$"
				SELECT *
				FROM [dbo].[Users]
				WHERE [UserName] = @UserNameParam
				";

			IEnumerable<GetUsersDTO> users =
				_dapper.QuerySql<GetUsersDTO>(sqlQueryUsers, sqlParameters);

			if (users.Any())
			{
				return BadRequest("Korisnik već postoji.");
			}

			// Query locations
			string sqlQueryLocations = @$"
				SELECT *
				FROM [dbo].[Locations]
				WHERE [LocationId] = @LocationIdParam
				";

			IEnumerable<LocationModel> locations =
				_dapper.QuerySql<LocationModel>(sqlQueryLocations, sqlParameters);

			if (locations.Any() is not true)
			{
				return BadRequest("Nema tražene lokacije.");
			}

			// Hash password
			var (passwordHash, passwordSalt) = _authHelper.HashPassword(userDto.Password);
			sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
			sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);

			// Insert user
			string sqlExecute = @$"
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

			if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom dodavanja korisnika.");
			}

			return Ok(userDto);
		}

		// Read
		[HttpGet]
		public IActionResult GetUsers()
		{
			string sqlQuery = @$"
				SELECT
					[UserId],
					[UserName],
					[LocationId]
				FROM [dbo].[Users]";

			IEnumerable<GetUsersDTO> users = _dapper.QuerySql<GetUsersDTO>(sqlQuery);

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
				return BadRequest("Potvrda lozinke se ne podudara.");
			}

			// Query users
			string sqlQueryUsers = @$"
				SELECT *
				FROM [dbo].[Users]
				WHERE [UserId] = @UserIdParam
				";

			IEnumerable<GetUsersDTO> users =
				_dapper.QuerySql<GetUsersDTO>(sqlQueryUsers, sqlParameters);

			if (users.Any() is not true)
			{
				return BadRequest("Nema traženog korisnika.");
			}

			// Query locations
			string sqlQueryLocations = @$"
				SELECT *
				FROM [dbo].[Locations]
				WHERE [LocationId] = @LocationIdParam
				";

			IEnumerable<LocationModel> locations =
				_dapper.QuerySql<LocationModel>(sqlQueryLocations, sqlParameters);

			if (locations.Any() is not true)
			{
				return BadRequest("Nema tražene lokacije.");
			}

			// Hash password
			var (passwordHash, passwordSalt) = _authHelper.HashPassword(editUserDTO.Password);
			sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
			sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);

			// Update user
			string sqlExecute = @$"
				UPDATE [dbo].[Users]
				SET
					[UserName] = @UserNameParam,
					[LocationId] = @LocationIdParam,
					[PasswordHash] = @PasswordHashParam,
					[PasswordSalt] = @PasswordSaltParam
				WHERE [UserId] = @UserIdParam
				";

			if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
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

			// Query users
			string sqlQueryUsers = @$"
				SELECT *
				FROM [dbo].[Users]
				WHERE [UserId] = @UserIdParam
				";

			IEnumerable<GetUsersDTO> users =
				_dapper.QuerySql<GetUsersDTO>(sqlQueryUsers, sqlParameters);

			if (users.Any() is not true)
			{
				return BadRequest("Nema traženog korisnika.");
			}

			// Delete user
			try
			{
				string sqlExecute = @$"
				DELETE
				FROM [dbo].[Users]
				WHERE [UserId] = @UserIdParam
				";

				if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
				{
					return BadRequest("Greška prilikom brisanja korisnika.");
				}
			}
			catch (SqlException ex)
			{
				if (ex.Number.Equals(547))
				{
					return BadRequest
						("Nije moguće izbrisati korisnika. Postoje povezani elementi u tabeli inventara.");
				}
			}

			return Ok(users);
		}

		// Read ById
		[HttpGet("{userId:int}")]
		public IActionResult GetUserById(int userId)
		{
			if (userId < 1)
			{
				return BadRequest($"Id korisnika mora da bude veći od 0.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@UserIdParam", userId, DbType.Int32);

			// Query users
			string sqlQuery = @$"
				SELECT [UserId], [UserName], [LocationId]
				FROM [dbo].[Users]
				WHERE [UserId] = @UserIdParam
				";

			IEnumerable<GetUsersDTO> user = _dapper.QuerySql<GetUsersDTO>(sqlQuery, sqlParameters);

			return Ok(user);
		}

		// Read LikeName
		[HttpGet("name/{userName}")]
		public IActionResult GetUsersLikeName(string userName)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@UserNameParam", userName, DbType.String);

			// Query users
			string sqlQuery = @$"
				SELECT [UserId], [UserName], [LocationId]
				FROM [dbo].[Users]
				WHERE [UserName] LIKE  '%' + @UserNameParam + '%'
				";

			IEnumerable<GetUsersDTO> users =
				_dapper.QuerySql<GetUsersDTO>(sqlQuery, sqlParameters);

			return Ok(users);
		}

		// Read LocationId
		[HttpGet("location/{locationId:int}")]
		public IActionResult GetUsersByLocationId(int locationId)
		{
			if (locationId < 1)
			{
				return BadRequest($"Id lokacije mora da bude veći od 0.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@LocationIdParam", locationId, DbType.Int32);

			// Query locations
			string sqlQueryLocations = @$"
				SELECT *
				FROM [dbo].[Locations]
				WHERE [LocationId] = @LocationIdParam
				";

			IEnumerable<LocationModel> locations =
				_dapper.QuerySql<LocationModel>(sqlQueryLocations, sqlParameters);

			if (locations.Any() is not true)
			{
				return BadRequest("Nema tražene lokacije.");
			}

			// Query users
			string sqlQuery = @$"
				SELECT [UserId], [UserName], [LocationId]
				FROM [dbo].[Users]
				WHERE [LocationId] = @LocationIdParam
				";

			IEnumerable<GetUsersDTO> users = _dapper.QuerySql<GetUsersDTO>(sqlQuery, sqlParameters);

			return Ok(users);
		}

		[HttpPost("login")]
		public IActionResult Login(LoginDTO login)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@UserNameParam", login.UserName, DbType.String);
			sqlParameters.Add("@PasswordParam", login.Password, DbType.String);

			// Query users
			string sqlQueryHashAndSalt = @$"
				SELECT [UserId], [PasswordHash], [PasswordSalt]
				FROM [dbo].[Users]
				WHERE [UserName] = @UserNameParam
				";

			IEnumerable<LoginHashAndSaltDTO> users =
				_dapper.QuerySql<LoginHashAndSaltDTO>(sqlQueryHashAndSalt, sqlParameters);

			if (users.Any() is not true)
			{
				return BadRequest("Nema traženog korisnika.");
			}

			if (users.First().PasswordHash is null || users.First().PasswordSalt is null)
			{
				return StatusCode(500,
					"Polja potrebna za dekripciju lozinke su prazna. Kontaktirajte administratora aplikacije.");
			}

			byte[] passwordHash =
				_authHelper.GetPasswordHash(login.Password, users.First().PasswordSalt!);

			for (int i = 0; i < passwordHash.Length; i++)
			{
				if (passwordHash[i].Equals(users.First().PasswordHash![i]) is not true)
				{
					return Unauthorized("Incorrect password!");
				}
			}

			return Ok(new Dictionary<string, string>
			{
				//{ "token", _authHelper.CreateToken(users.First().UserId) }
				{ "token", "Bearer " + _authHelper.CreateToken(users.First().UserId) }
			});
		}

		[HttpGet("RefreshToken")]
		public IActionResult RefreshToken()
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@UserIdParam", User.FindFirst("userId")?.Value, DbType.Int32);

			// Query Db for user
			string sqlUserQueryDb = @$"
				SELECT [UserId]
				FROM [dbo].[Users]
				WHERE [UserId] = '{User.FindFirst("userId")?.Value ?? "1"}'
				";

			int userId = _dapper.QuerySql<int>(sqlUserQueryDb).First();

			return Ok(new Dictionary<string, string>
			{
				//{ "token", _authHelper.CreateToken(users.First().UserId) }
				{ "token", "Bearer " + _authHelper.CreateToken(userId) }
			});
		}

		//Seed
		[HttpPost("seed")]
		public IActionResult SeedUsers()
		{
			int counter = 0;
			List<NewUserDTO> newUsersList =
			[
				new NewUserDTO {
					UserName = "Zeleni Zub", LocationId = 1, Password = "zz", PasswordConfirm = "zz"},
				new NewUserDTO {
					UserName = "Beavis", LocationId = 2, Password = "zz", PasswordConfirm = "zz"},
				new NewUserDTO {
					UserName = "Butthead", LocationId = 2, Password = "zz", PasswordConfirm = "zz"},
				new NewUserDTO {
					UserName = "Mirko", LocationId = 3, Password = "zz", PasswordConfirm = "zz"},
				new NewUserDTO {
					UserName = "Slavko", LocationId = 3, Password = "zz", PasswordConfirm = "zz"},
			];

			foreach (var newUser in newUsersList)
			{
				DynamicParameters sqlParameters = new();
				sqlParameters.Add("@UserNameParam", newUser.UserName, DbType.String);

				// Query users
				string sqlQueryUsers = @$"
				SELECT *
				FROM [dbo].[Users]
				WHERE [UserName] = @UserNameParam
				";

				IEnumerable<GetUsersDTO> users =
					_dapper.QuerySql<GetUsersDTO>(sqlQueryUsers, sqlParameters);

				if (users.Any())
				{
					continue;
				}

				// Query locations
				string locationName = newUser.LocationId switch
				{
					1 => "Subotica",
					2 => "Novi Sad",
					3 => "Beograd",
					_ => String.Empty,
				};

				sqlParameters.Add("@LocationNameParam", locationName, DbType.String);

				string sqlQueryLocations = @$"
				SELECT *
				FROM [dbo].[Locations]
				WHERE [LocationName] = @LocationNameParam
				";

				IEnumerable<LocationModel> locations =
					_dapper.QuerySql<LocationModel>(sqlQueryLocations, sqlParameters);

				if (locations.Any() is not true)
				{
					continue;
				}

				sqlParameters.Add("@LocationsIdParam", locations.First().LocationId, DbType.Int32);

				// Hash password
				var (passwordHash, passwordSalt) = _authHelper.HashPassword(newUser.Password);
				sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
				sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);

				// Insert user
				string sqlExecute = @$"
				INSERT INTO [dbo].[Users] (
					[UserName],
					[LocationId],
					[PasswordHash],
					[PasswordSalt]
				) VALUES (
					@UserNameParam,
					@LocationsIdParam,
					@PasswordHashParam,
					@PasswordSaltParam
				)";
				try
				{
					_dapper.ExecuteSql(sqlExecute, sqlParameters);
					counter++;
				}
				catch (Exception)
				{
					continue;
				}
			}

			return Ok($"Dodavanje podataka završeno. Ukupno dodato: {counter}");
		}
	}
}
