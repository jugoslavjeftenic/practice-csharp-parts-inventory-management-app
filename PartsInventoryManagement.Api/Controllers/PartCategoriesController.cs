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
	public class PartCategoriesController(IConfiguration config) : ControllerBase
	{
		private readonly DbContextDapper _dapper = new(config);

		//Create
		[HttpPost]
		public IActionResult AddPartCategory(NewPartCategoryDTO partCategoryDto)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartCategoryNameParam", partCategoryDto.PartCategoryName, DbType.String);

			// Query Db for part category
			string sqlPartCategoriesQueryDb = @$"
				SELECT *
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryName] = @PartCategoryNameParam
				";

			IEnumerable<PartCategoryModel> partCategoriesQueryDb =
				_dapper.QuerySql<PartCategoryModel>(sqlPartCategoriesQueryDb, sqlParameters);

			if (partCategoriesQueryDb.Any())
			{
				return BadRequest("Kategorija već postoji.");
			}

			// Insert part category
			string sql = @$"
				INSERT INTO [dbo].[PartCategories] (
					[PartCategoryName]
				) VALUES (
					@PartCategoryNameParam
				)";

			if (_dapper.ExecuteSql(sql, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom dodavanja kategorije.");
			}

			return Ok(partCategoryDto);
		}

		// Read
		[HttpGet]
		public IActionResult GetPartCategories()
		{
			string sql = "SELECT [PartCategoryId], [PartCategoryName] FROM [dbo].[PartCategories]";

			IEnumerable<PartCategoryModel> partCategories = _dapper.QuerySql<PartCategoryModel>(sql);

			return Ok(partCategories);
		}

		// Update
		[HttpPut]
		public IActionResult EditPartCategory(PartCategoryModel partCategory)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartCategoryIdParam", partCategory.PartCategoryId, DbType.Int32);
			sqlParameters.Add("@PartCategoryNameParam", partCategory.PartCategoryName, DbType.String);

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

			// Update part category
			string sql = @$"
				UPDATE [dbo].[PartCategories]
				SET
					[PartCategoryName] = @PartCategoryNameParam
				WHERE [PartCategoryId] = @PartCategoryIdParam
				";

			if (_dapper.ExecuteSql(sql, sqlParameters) == false)
			{
				return BadRequest("Greška prilikom izmene kategorije.");
			}

			return Ok(partCategory);
		}

		// Delete
		[HttpDelete("{partCategoryId:int}")]
		public IActionResult DeletePartCategory(int partCategoryId)
		{
			if (partCategoryId < 1)
			{
				return BadRequest($"Id kategorije mora da bude veći od 0.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartCategoryIdParam", partCategoryId, DbType.Int32);

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
				return Conflict("Nema tražene kategorije.");
			}

			// Delete part category
			string sql = @$"
				DELETE
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryId] = @PartCategoryIdParam
				";

			if (_dapper.ExecuteSql(sql, sqlParameters) == false)
			{
				return BadRequest("Greška prilikom brisanja kategorije.");
			}

			return Ok(partCategoriesQueryDb);
		}

		// Read ById
		[HttpGet("{partCategoryId:int}")]
		public IActionResult GetPartCategoryById(int partCategoryId)
		{
			if (partCategoryId < 1)
			{
				return BadRequest($"Id kategorije mora da bude veći od 0.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartCategoryIdParam", partCategoryId, DbType.Int32);

			// Query Db by part category id
			string sql = @$"
				SELECT [PartCategoryId], [PartCategoryName]
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryId] = @PartCategoryIdParam
				";

			IEnumerable<PartCategoryModel> partCategories =
				_dapper.QuerySql<PartCategoryModel>(sql, sqlParameters);

			return Ok(partCategories);
		}

		// Read LikeName
		[HttpGet("{partCategoryName}")]
		public IActionResult GetPartCategoriesLikeName(string partCategoryName)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartCategoryNameParam", partCategoryName, DbType.String);

			// Query Db by part category partial name
			// If route parameter is int, method will be overload with GetPartCategoryById
			string sql = @$"
				SELECT [PartCategoryId], [PartCategoryName]
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryName] LIKE  '%' + @PartCategoryNameParam + '%'
				";

			IEnumerable<PartCategoryModel> partCategories =
				_dapper.QuerySql<PartCategoryModel>(sql, sqlParameters);

			return Ok(partCategories);
		}
	}
}
