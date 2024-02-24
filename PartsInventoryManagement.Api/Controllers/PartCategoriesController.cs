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
	public class PartCategoriesController(IConfiguration config) : ControllerBase
	{
		private readonly DbContextDapper _dapper = new(config);

		// Create
		[HttpPost]
		public IActionResult AddPartCategory(NewPartCategoryDTO newPartCategoryDto)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartCategoryNameParam", newPartCategoryDto.PartCategoryName, DbType.String);

			// Query part categories
			string sqlQueryPartCategories = @$"
				SELECT *
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryName] = @PartCategoryNameParam
				";

			IEnumerable<PartCategoryModel> partCategories =
				_dapper.QuerySql<PartCategoryModel>(sqlQueryPartCategories, sqlParameters);

			if (partCategories.Any())
			{
				return BadRequest("Kategorija već postoji.");
			}

			// Insert part category
			string sqlExecute = @$"
				INSERT INTO [dbo].[PartCategories] (
					[PartCategoryName]
				) VALUES (
					@PartCategoryNameParam
				)";

			if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom dodavanja kategorije.");
			}

			return Ok(newPartCategoryDto);
		}

		// Read
		[HttpGet]
		public IActionResult GetPartCategories()
		{
			// Query part categories
			string sqlQuery = "SELECT [PartCategoryId], [PartCategoryName] FROM [dbo].[PartCategories]";

			IEnumerable<PartCategoryModel> partCategories = _dapper.QuerySql<PartCategoryModel>(sqlQuery);

			return Ok(partCategories);
		}

		// Update
		[HttpPut]
		public IActionResult EditPartCategory(PartCategoryModel partCategory)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartCategoryIdParam", partCategory.PartCategoryId, DbType.Int32);
			sqlParameters.Add("@PartCategoryNameParam", partCategory.PartCategoryName, DbType.String);

			// Query part categories
			string sqlQueryPartCategories = @$"
				SELECT *
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryId] = @PartCategoryIdParam
				";

			IEnumerable<PartCategoryModel> partCategories =
				_dapper.QuerySql<PartCategoryModel>(sqlQueryPartCategories, sqlParameters);

			if (partCategories.Any() is not true)
			{
				return BadRequest("Nema tražene kategorije.");
			}

			// Update part category
			string sqlExecute = @$"
				UPDATE [dbo].[PartCategories]
				SET
					[PartCategoryName] = @PartCategoryNameParam
				WHERE [PartCategoryId] = @PartCategoryIdParam
				";

			if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
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

			// Query part categories
			string sqlQueryPartCategories = @$"
				SELECT *
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryId] = @PartCategoryIdParam
				";

			IEnumerable<PartCategoryModel> partCategories =
				_dapper.QuerySql<PartCategoryModel>(sqlQueryPartCategories, sqlParameters);

			if (partCategories.Any() is not true)
			{
				return BadRequest("Nema tražene kategorije.");
			}

			// Delete part category
			try
			{
				string sqlExecute = @$"
				DELETE
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryId] = @PartCategoryIdParam
				";

				if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
				{
					return BadRequest("Greška prilikom brisanja kategorije.");
				}
			}
			catch (SqlException ex)
			{
				if (ex.Number.Equals(547))
				{
					return BadRequest("Nije moguće izbrisati kategoriju jer postoje povezani elementi.");
				}
			}

			return Ok(partCategories);
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

			// Query part categories
			string sqlQueryPartCategories = @$"
				SELECT
					[PartCategoryId],
					[PartCategoryName]
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryId] = @PartCategoryIdParam
				";

			IEnumerable<PartCategoryModel> partCategories =
				_dapper.QuerySql<PartCategoryModel>(sqlQueryPartCategories, sqlParameters);

			return Ok(partCategories);
		}

		// Read LikeName
		[HttpGet("name/{partCategoryName}")]
		public IActionResult GetPartCategoriesLikeName(string partCategoryName)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartCategoryNameParam", partCategoryName, DbType.String);

			// Query part categories
			string sqlQueryPartCategories = @$"
				SELECT
					[PartCategoryId],
					[PartCategoryName]
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryName] LIKE '%' + @PartCategoryNameParam + '%'
				";

			IEnumerable<PartCategoryModel> partCategories =
				_dapper.QuerySql<PartCategoryModel>(sqlQueryPartCategories, sqlParameters);

			return Ok(partCategories);
		}

		//Seed
		[HttpPost("seed")]
		public IActionResult SeedPartCategories()
		{
			int counter = 0;
			List<NewPartCategoryDTO> newPartCategoriesList =
			[
				new NewPartCategoryDTO { PartCategoryName ="Diskovi"},
				new NewPartCategoryDTO { PartCategoryName ="Memorije"},
				new NewPartCategoryDTO { PartCategoryName ="Periferije"},
				new NewPartCategoryDTO { PartCategoryName ="Svičevi"},
				new NewPartCategoryDTO { PartCategoryName ="Kablovi"}
			];

			foreach (var newPartCategory in newPartCategoriesList)
			{
				DynamicParameters sqlParameters = new();
				sqlParameters.Add("@PartCategoryNameParam", newPartCategory.PartCategoryName, DbType.String);

				// Query part categories
				string sqlQueryPartCategories = @$"
				SELECT *
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryName] = @PartCategoryNameParam
				";

				IEnumerable<PartCategoryModel> partCategories =
					_dapper.QuerySql<PartCategoryModel>(sqlQueryPartCategories, sqlParameters);

				if (partCategories.Any())
				{
					continue;
				}

				// Insert part category
				string sqlExecute = @$"
				INSERT INTO [dbo].[PartCategories] (
					[PartCategoryName]
				) VALUES (
					@PartCategoryNameParam
				)
				";

				try
				{
					_dapper.ExecuteSql(sqlExecute, sqlParameters);
					counter++;
				}
				catch (System.Exception)
				{
					throw;
				}
			}

			return Ok($"Dodavanje podataka završeno. Ukupno dodato: {counter}");
		}
	}
}
