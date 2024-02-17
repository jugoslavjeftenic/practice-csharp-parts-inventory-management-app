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
	public class PartCategoriesController(IConfiguration config) : ControllerBase
	{
		private readonly DbContextDapper _dapper = new(config);

		//Create
		[HttpPost]
		public IActionResult AddPartCategory(NewPartCategoryDTO partCategoryDto)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartCategoryNameParam", partCategoryDto.PartCategoryName, DbType.String);

			string sqlQueryDb = @$"
				SELECT *
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryName] = @PartCategoryNameParam
				";

			IEnumerable<PartCategoryModel> partCategoriesQueryDb =
				_dapper.QuerySql<PartCategoryModel>(sqlQueryDb, sqlParameters);

			if (partCategoriesQueryDb.Any())
			{
				return Conflict("Kategorija već postoji.");
			}

			string sql = @$"
				INSERT INTO [dbo].[PartCategories] (
					[PartCategoryName]
				) VALUES (
					@PartCategoryNameParam
				)";

			if (_dapper.ExecuteSql(sql, sqlParameters) == false)
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

			if (partCategories == null || !partCategories.Any())
			{
				return NotFound("Tabela kategorija delova je prazna.");
			}

			return Ok(partCategories);
		}

		// Update
		[HttpPut]
		public IActionResult EditPartCategory(PartCategoryModel partCategory)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartCategoryIdParam", partCategory.PartCategoryId, DbType.Int32);
			sqlParameters.Add("@PartCategoryNameParam", partCategory.PartCategoryName, DbType.String);

			string sqlQueryDb = @$"
				SELECT *
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryId] = @PartCategoryIdParam
				";

			IEnumerable<PartCategoryModel> partCategoriesQueryDb =
				_dapper.QuerySql<PartCategoryModel>(sqlQueryDb, sqlParameters);

			if (partCategoriesQueryDb.Any() == false)
			{
				return Conflict("Nema tražene kategorije.");
			}

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
		[HttpDelete("/{partCategoryId}")]
		public IActionResult DeletePartCategory(int partCategoryId)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartCategoryIdParam", partCategoryId, DbType.Int32);

			string sqlQueryDb = @$"
				SELECT *
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryId] = @PartCategoryIdParam
				";

			IEnumerable<PartCategoryModel> partCategoriesQueryDb =
				_dapper.QuerySql<PartCategoryModel>(sqlQueryDb, sqlParameters);

			if (partCategoriesQueryDb.Any() == false)
			{
				return Conflict("Nema tražene kategorije.");
			}

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
	}
}
