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
	[Route("api/[controller]")]
	public class LocationsController(IConfiguration config) : ControllerBase
	{
		private readonly DbContextDapper _dapper = new(config);

		//Create
		[HttpPost]
		public IActionResult AddLocation(NewLocationDTO locationDto)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@LocationAlphaParam", locationDto.LocationAlpha, DbType.String);
			sqlParameters.Add("@LocationNameParam", locationDto.LocationName, DbType.String);
			sqlParameters.Add("@LocationColorParam", locationDto.LocationColor, DbType.String);

			// Query Db for location
			string sqlPartCategoriesQueryDb = @$"
				SELECT *
				FROM [dbo].[Locations]
				WHERE [LocationAlpha] = @LocationAlphaParam AND [LocationName] = @LocationNameParam
				";

			IEnumerable<LocationModel> locationQueryDb =
				_dapper.QuerySql<LocationModel>(sqlPartCategoriesQueryDb, sqlParameters);

			if (locationQueryDb.Any())
			{
				return BadRequest("Lokacija već postoji.");
			}

			// Insert location
			string sql = @$"
				INSERT INTO [dbo].[Locations] (
					[LocationAlpha],
					[LocationName],
					[LocationColor]
				) VALUES (
					@LocationAlphaParam,
					@LocationNameParam,
					@LocationColorParam
				)";

			if (_dapper.ExecuteSql(sql, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom dodavanja lokacije.");
			}

			return Ok(locationDto);
		}

		// Read
		[HttpGet]
		public IActionResult GetLocations()
		{
			string sql = @$"
				SELECT
					[LocationId],
					[LocationAlpha],
					[LocationName],
					[LocationColor]
				FROM [dbo].[Locations]";

			IEnumerable<LocationModel> locations = _dapper.QuerySql<LocationModel>(sql);

			return Ok(locations);
		}

		// Update
		[HttpPut]
		public IActionResult EditLocation(LocationModel location)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@LocationIdParam", location.LocationId, DbType.Int32);
			sqlParameters.Add("@LocationAlphaParam", location.LocationAlpha, DbType.String);
			sqlParameters.Add("@LocationNameParam", location.LocationName, DbType.String);
			sqlParameters.Add("@LocationColorParam", location.LocationColor, DbType.String);

			// Query Db for location
			string sqlPartCategoriesQueryDb = @$"
				SELECT *
				FROM [dbo].[Locations]
				WHERE [LocationId] = @LocationIdParam
				";

			IEnumerable<LocationModel> locationsQueryDb =
				_dapper.QuerySql<LocationModel>(sqlPartCategoriesQueryDb, sqlParameters);

			if (locationsQueryDb.Any() is not true)
			{
				return BadRequest("Nema tražene lokacije.");
			}

			// Update location
			string sql = @$"
				UPDATE [dbo].[Locations]
				SET
					[LocationAlpha] = @LocationAlphaParam,
					[LocationName] = @LocationNameParam,
					[LocationColor] = @LocationColorParam
				WHERE [LocationId] = @LocationIdParam
				";

			if (_dapper.ExecuteSql(sql, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom izmene lokacije.");
			}

			return Ok(location);
		}

		// Delete
		[HttpDelete("{locationId:int}")]
		public IActionResult DeleteLocation(int locationId)
		{
			if (locationId < 1)
			{
				return BadRequest($"Id lokacije mora da bude veći od 0.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@LocationIdParam", locationId, DbType.Int32);

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
				return Conflict("Nema tražene lokacije.");
			}

			// Delete location
			string sql = @$"
				DELETE
				FROM [dbo].[Locations]
				WHERE [LocationId] = @LocationIdParam
				";

			if (_dapper.ExecuteSql(sql, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom brisanja lokacije.");
			}

			return Ok(locationsQueryDb);
		}
	}
}
