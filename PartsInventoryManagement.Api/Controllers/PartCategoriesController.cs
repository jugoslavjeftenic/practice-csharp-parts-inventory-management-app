using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartsInventoryManagement.Api.Data;
using PartsInventoryManagement.Api.Dtos;
using PartsInventoryManagement.Api.Models;
using System.Collections.Generic;
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
			string sqlQueryDb = @$"
				SELECT *
				FROM [dbo].[PartCategories]
				WHERE LOWER([PartCategoryName]) = LOWER('{partCategoryDto.PartCategoryName}')
				";

			IEnumerable<PartCategoryModel> partCategories = _dapper.LoadData<PartCategoryModel>(sqlQueryDb);

			if (partCategories.Any())
			{
				return Conflict("Kategorija već postoji.");
			}

			string sql = @$"
				INSERT INTO [dbo].[PartCategories] (
					[PartCategoryName]
				) VALUES (
					'{partCategoryDto.PartCategoryName}'
				)";

			if (_dapper.ExecuteSql(sql) == false)
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

			IEnumerable<PartCategoryModel> partCategories = _dapper.LoadData<PartCategoryModel>(sql);

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
			string sqlQueryDb = @$"
				SELECT *
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryId] = {partCategory.PartCategoryId}
				";

			if (_dapper.LoadData<PartCategoryModel>(sqlQueryDb).Any() == false)
			{
				return Conflict("Nema tražene kategorije.");
			}

			string sql = @$"
				UPDATE [dbo].[PartCategories]
				SET
					[PartCategoryName] = '{partCategory.PartCategoryName}'
				WHERE [PartCategoryId] = {partCategory.PartCategoryId}
				";

			if (_dapper.ExecuteSql(sql) == false)
			{
				return BadRequest("Greška prilikom izmene kategorije.");
			}

			return Ok(partCategory);
		}

		// Delete
		[HttpDelete("/{partCategoryId}")]
		public IActionResult DeletePartCategory(int partCategoryId)
		{
			string sqlQueryDb = @$"
				SELECT *
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryId] = {partCategoryId}
				";

			IEnumerable<PartCategoryModel> partCategories = _dapper.LoadData<PartCategoryModel>(sqlQueryDb);

			if (partCategories.Any() == false)
			{
				return Conflict("Nema tražene kategorije.");
			}

			string sql = @$"
				DELETE
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryId] = {partCategoryId}
				";

			if (_dapper.ExecuteSql(sql) == false)
			{
				return BadRequest("Greška prilikom brisanja kategorije.");
			}

			return Ok(partCategories);
		}
	}
}
