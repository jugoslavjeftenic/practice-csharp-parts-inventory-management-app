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
	public class PartsController(IConfiguration config) : ControllerBase
	{
		private readonly DbContextDapper _dapper = new(config);

		// Create
		[HttpPost]
		public IActionResult AddPart(NewPartDTO partDto)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartCategoryIdParam", partDto.PartCategoryId, DbType.Int32);
			sqlParameters.Add("@PartNameParam", partDto.PartName, DbType.String);

			// Query Db for part category
			string sqlPartCategoriesQueryDb = @$"
				SELECT *
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryId] = @PartCategoryIdParam
				";

			IEnumerable<PartCategoryModel> partCategoriesQueryDb =
				_dapper.QuerySql<PartCategoryModel>(sqlPartCategoriesQueryDb, sqlParameters);

			if (partCategoriesQueryDb.Any() is not true)
			{
				return BadRequest("Nema tražene kategorije.");
			}

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

			return Ok(partDto);
		}

		// Read
		[HttpGet]
		public IActionResult GetParts()
		{
			string sql = "SELECT [PartId], [PartCategoryId], [PartName] FROM [dbo].[Parts]";

			IEnumerable<PartModel> parts = _dapper.QuerySql<PartModel>(sql);

			return Ok(parts);
		}

		// Update
		[HttpPut]
		public IActionResult EditPart(PartModel part)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartIdParam", part.PartId, DbType.Int32);
			sqlParameters.Add("@PartCategoryIdParam", part.PartCategoryId, DbType.Int32);
			sqlParameters.Add("@PartNameParam", part.PartName, DbType.String);

			// Query Db for part
			string sqlPartQueryDb = @$"
				SELECT *
				FROM [dbo].[Parts]
				WHERE [PartId] = @PartIdParam
				";

			IEnumerable<PartModel> partsQueryDb =
				_dapper.QuerySql<PartModel>(sqlPartQueryDb, sqlParameters);

			if (partsQueryDb.Any() is not true)
			{
				return BadRequest("Nema traženog dela.");
			}

			// Query Db for part category
			string sqlPartCategoriesQueryDb = @$"
				SELECT *
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryId] = @PartCategoryIdParam
				";

			IEnumerable<PartCategoryModel> partCategoriesQueryDb =
				_dapper.QuerySql<PartCategoryModel>(sqlPartCategoriesQueryDb, sqlParameters);

			if (partCategoriesQueryDb.Any() is not true)
			{
				return BadRequest("Nema tražene kategorije.");
			}

			// Update part
			string sql = @$"
				UPDATE [dbo].[Parts]
				SET
					[PartCategoryId] = @PartCategoryIdParam,
					[PartName] = @PartNameParam
				WHERE [PartId] = @PartIdParam
				";

			if (_dapper.ExecuteSql(sql, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom izmene dela.");
			}

			return Ok(part);
		}

		// Delete
		[HttpDelete("{partId:int}")]
		public IActionResult DeletePart(int partId)
		{
			if (partId < 1)
			{
				return BadRequest($"Id dela mora da bude veći od 0.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartIdParam", partId, DbType.Int32);

			// Query Db for part
			string sqlPartQueryDb = @$"
				SELECT *
				FROM [dbo].[Parts]
				WHERE [PartId] = @PartIdParam
				";

			IEnumerable<PartModel> partsQueryDb =
				_dapper.QuerySql<PartModel>(sqlPartQueryDb, sqlParameters);

			if (partsQueryDb.Any() is not true)
			{
				return BadRequest("Nema traženog dela.");
			}

			// Delete part
			string sql = @$"
				DELETE
				FROM [dbo].[Parts]
				WHERE [PartId] = @PartIdParam
				";

			if (_dapper.ExecuteSql(sql, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom brisanja dela.");
			}

			return Ok(partsQueryDb);
		}
	}
}
