using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
	[Route("api/v1/[controller]")]
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
			sqlParameters.Add("@LocationHexColorParam", locationDto.LocationHexColor, DbType.String);

			// Query locations
			string sqlQueryLocations = @$"
				SELECT *
				FROM [dbo].[Locations]
				WHERE [LocationAlpha] = @LocationAlphaParam OR [LocationName] = @LocationNameParam
				";

			IEnumerable<LocationModel> locations =
				_dapper.QuerySql<LocationModel>(sqlQueryLocations, sqlParameters);

			if (locations.Any())
			{
				return BadRequest("Lokacija sa istim nazivom ili alfom već postoji.");
			}

			// Insert location
			string sqlExecute = @$"
				INSERT INTO [dbo].[Locations] (
					[LocationAlpha],
					[LocationName],
					[LocationHexColor]
				) VALUES (
					@LocationAlphaParam,
					@LocationNameParam,
					@LocationHexColorParam
				)";

			if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom dodavanja lokacije.");
			}

			return Ok(locationDto);
		}

		// Read
		[HttpGet]
		public IActionResult GetLocations()
		{
			string sqlQuery = @$"
				SELECT
					[LocationId],
					[LocationAlpha],
					[LocationName],
					[LocationHexColor]
				FROM [dbo].[Locations]";

			IEnumerable<LocationModel> locations = _dapper.QuerySql<LocationModel>(sqlQuery);

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
			sqlParameters.Add("@LocationHexColorParam", location.LocationHexColor, DbType.String);

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

			// Update location
			string sqlExecute = @$"
				UPDATE [dbo].[Locations]
				SET
					[LocationAlpha] = @LocationAlphaParam,
					[LocationName] = @LocationNameParam,
					[LocationHexColor] = @LocationHexColorParam
				WHERE [LocationId] = @LocationIdParam
				";

			if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
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

			// Delete location
			try
			{
				string sqlExecute = @$"
				DELETE
				FROM [dbo].[Locations]
				WHERE [LocationId] = @LocationIdParam
				";

				if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
				{
					return BadRequest("Greška prilikom brisanja lokacije.");
				}
			}
			catch (SqlException ex)
			{
				if (ex.Number.Equals(547))
				{
					return BadRequest
						("Nije moguće izbrisati lokaciju. Postoje povezani elementi u tabeli korisnika.");
				}
			}

			return Ok(locations);
		}

		// Read ById
		[HttpGet("{locationId:int}")]
		public IActionResult GetLocationById(int locationId)
		{
			if (locationId < 1)
			{
				return BadRequest($"Id lokacije mora da bude veći od 0.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@LocationIdParam", locationId, DbType.Int32);

			// Query locations
			string sqlQuery = @$"
				SELECT
					[LocationId],
					[LocationAlpha],
					[LocationName],
					[LocationHexColor]
				FROM [dbo].[Locations]
				WHERE [LocationId] = @LocationIdParam
				";

			IEnumerable<LocationModel> locations =
				_dapper.QuerySql<LocationModel>(sqlQuery, sqlParameters);

			return Ok(locations);
		}

		// Read LikeAlpha
		[HttpGet("alpha/{locationAlpha}")]
		public IActionResult GetLocationsLikeAlpha(string locationAlpha)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@LocationAlphaParam", locationAlpha, DbType.String);

			// Query locations
			string sqlQuery = @$"
				SELECT
					[LocationId],
					[LocationAlpha],
					[LocationName],
					[LocationHexColor]
				FROM [dbo].[Locations]
				WHERE [LocationAlpha] LIKE '%' + @LocationAlphaParam + '%'
				";

			IEnumerable<LocationModel> locations =
				_dapper.QuerySql<LocationModel>(sqlQuery, sqlParameters);

			return Ok(locations);
		}

		// Read LikeName
		[HttpGet("name/{locationName}")]
		public IActionResult GetLocationsLikeName(string locationName)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@LocationNameParam", locationName, DbType.String);

			// Query locations
			string sqlQuery = @$"
				SELECT
					[LocationId],
					[LocationAlpha],
					[LocationName],
					[LocationHexColor]
				FROM [dbo].[Locations]
				WHERE [LocationName] LIKE '%' + @LocationNameParam + '%'
				";

			IEnumerable<LocationModel> locations =
				_dapper.QuerySql<LocationModel>(sqlQuery, sqlParameters);

			return Ok(locations);
		}

		// Read ByHexColor
		[HttpGet("color/{locationHexColor}")]
		public IActionResult GetLocationsByColor(string locationHexColor)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@LocationHexColorParam", locationHexColor, DbType.String);

			// Query locations
			string sqlQuery = @$"
				SELECT
					[LocationId],
					[LocationAlpha],
					[LocationName],
					[LocationHexColor]
				FROM [dbo].[Locations]
				WHERE [LocationHexColor] = @LocationHexColorParam
				";

			IEnumerable<LocationModel> locations =
				_dapper.QuerySql<LocationModel>(sqlQuery, sqlParameters);

			return Ok(locations);
		}
	}
}
