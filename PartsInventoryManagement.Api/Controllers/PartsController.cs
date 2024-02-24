using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PartsInventoryManagement.Api.Data;
using PartsInventoryManagement.Api.Dtos;
using PartsInventoryManagement.Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PartsInventoryManagement.Api.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
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

			// Query parts
			string sqlQueryParts = @$"
				SELECT *
				FROM [dbo].[Parts]
				WHERE [PartName] = @PartNameParam
				";

			IEnumerable<PartModel> parts =
				_dapper.QuerySql<PartModel>(sqlQueryParts, sqlParameters);

			if (parts.Any())
			{
				return BadRequest("Deo već postoji.");
			}

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

			// Insert part
			string sqlExecute = @$"
				INSERT INTO [dbo].[Parts] (
					[PartCategoryId],
					[PartName]
				) VALUES (
					@PartCategoryIdParam,
					@PartNameParam
				)";

			if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom dodavanja dela.");
			}

			return Ok(partDto);
		}

		// Read
		[HttpGet]
		public IActionResult GetParts()
		{
			string sqlQuery = "SELECT [PartId], [PartCategoryId], [PartName] FROM [dbo].[Parts]";

			IEnumerable<PartModel> parts = _dapper.QuerySql<PartModel>(sqlQuery);

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
			string sqlQueryParts = @$"
				SELECT *
				FROM [dbo].[Parts]
				WHERE [PartId] = @PartIdParam
				";

			IEnumerable<PartModel> parts =
				_dapper.QuerySql<PartModel>(sqlQueryParts, sqlParameters);

			if (parts.Any() is not true)
			{
				return BadRequest("Nema traženog dela.");
			}

			// Query Db for part category
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

			// Update part
			string sqlExecute = @$"
				UPDATE [dbo].[Parts]
				SET
					[PartCategoryId] = @PartCategoryIdParam,
					[PartName] = @PartNameParam
				WHERE [PartId] = @PartIdParam
				";

			if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
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
			string sqlQueryParts = @$"
				SELECT *
				FROM [dbo].[Parts]
				WHERE [PartId] = @PartIdParam
				";

			IEnumerable<PartModel> parts =
				_dapper.QuerySql<PartModel>(sqlQueryParts, sqlParameters);

			if (parts.Any() is not true)
			{
				return BadRequest("Nema traženog dela.");
			}

			// Delete part
			try
			{
				string sqlExecute = @$"
				DELETE
				FROM [dbo].[Parts]
				WHERE [PartId] = @PartIdParam
				";

				if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
				{
					return BadRequest("Greška prilikom brisanja dela.");
				}
			}
			catch (SqlException ex)
			{
				if (ex.Number.Equals(547))
				{
					return BadRequest
						("Nije moguće izbrisati deo. Postoje povezani elementi u tabeli inventara.");
				}
			}

			return Ok(parts);
		}

		// Read ById
		[HttpGet("{partId:int}")]
		public IActionResult GetPartById(int partId)
		{
			if (partId < 1)
			{
				return BadRequest($"Id dela mora da bude veći od 0.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartIdParam", partId, DbType.Int32);

			// Query Db by part id
			string sqlQuery = @$"
				SELECT [PartId], [PartCategoryId], [PartName]
				FROM [dbo].[Parts]
				WHERE [PartId] = @PartIdParam
				";

			IEnumerable<PartModel> parts = _dapper.QuerySql<PartModel>(sqlQuery, sqlParameters);

			return Ok(parts);
		}

		// Read ByPartCategoryId
		[HttpGet("category/{partCategoryId:int}")]
		public IActionResult GetPartsByPartCategoryId(int partCategoryId)
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

			// Query parts
			string sqlQuery = @$"
				SELECT [PartId], [PartCategoryId], [PartName]
				FROM [dbo].[Parts]
				WHERE [PartCategoryId] = @PartCategoryIdParam
				";

			IEnumerable<PartModel> parts = _dapper.QuerySql<PartModel>(sqlQuery, sqlParameters);

			return Ok(parts);
		}

		// Read LikeName
		[HttpGet("name/{partName}")]
		public IActionResult GetPartsLikeName(string partName)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartNameParam", partName, DbType.String);

			// Query parts
			string sqlQuery = @$"
				SELECT [PartId], [PartCategoryId], [PartName]
				FROM [dbo].[Parts]
				WHERE [PartName] LIKE  '%' + @PartNameParam + '%'
				";

			IEnumerable<PartModel> parts =
				_dapper.QuerySql<PartModel>(sqlQuery, sqlParameters);

			return Ok(parts);
		}

		//Seed
		[HttpPost("seed")]
		public IActionResult SeedParts()
		{
			int counter = 0;
			List<NewPartDTO> newPartsList =
			[
				new NewPartDTO { PartCategoryId = 1, PartName = "HDD 3.5in 500GB"},
				new NewPartDTO { PartCategoryId = 1, PartName = "HDD 3.5in 1TB"},
				new NewPartDTO { PartCategoryId = 1, PartName = "HDD 2.5in 500GB"},
				new NewPartDTO { PartCategoryId = 1, PartName = "HDD 2.5in 1TB"},
				new NewPartDTO { PartCategoryId = 1, PartName = "SSD 2.5in 256GB"},
				new NewPartDTO { PartCategoryId = 1, PartName = "SSD 2.5in 512GB"},
				new NewPartDTO { PartCategoryId = 1, PartName = "SSD NVME 2280 M.2 256GB"},
				new NewPartDTO { PartCategoryId = 1, PartName = "SSD NVME 2280 M.2 512GB"},
				new NewPartDTO { PartCategoryId = 2, PartName = "DIMM DDR3 4GB"},
				new NewPartDTO { PartCategoryId = 2, PartName = "DIMM DDR3 8GB"},
				new NewPartDTO { PartCategoryId = 2, PartName = "DIMM DDR4 4GB"},
				new NewPartDTO { PartCategoryId = 2, PartName = "DIMM DDR4 8GB"},
				new NewPartDTO { PartCategoryId = 2, PartName = "SODIMM DDR3 4GB"},
				new NewPartDTO { PartCategoryId = 2, PartName = "SODIMM DDR3 8GB"},
				new NewPartDTO { PartCategoryId = 2, PartName = "SODIMM DDR4 4GB"},
				new NewPartDTO { PartCategoryId = 2, PartName = "SODIMM DDR4 8GB"},
				new NewPartDTO { PartCategoryId = 3, PartName = "Miš USB optički crni"},
				new NewPartDTO { PartCategoryId = 3, PartName = "Tastatura YU USB crna"},
				new NewPartDTO { PartCategoryId = 3, PartName = "Ethernet adapter USB3 Gb"},
				new NewPartDTO { PartCategoryId = 4, PartName = "Svič neupravljivi 8 portni"},
				new NewPartDTO { PartCategoryId = 4, PartName = "Svič neupravljivi 16 portni"},
				new NewPartDTO { PartCategoryId = 5, PartName = "U/UTP peč kabel Cat5e 1m"},
				new NewPartDTO { PartCategoryId = 5, PartName = "U/UTP peč kabel Cat5e 2m"},
				new NewPartDTO { PartCategoryId = 5, PartName = "U/UTP peč kabel Cat5e 3m"},
				new NewPartDTO { PartCategoryId = 5, PartName = "U/UTP kotur kabel Cat5e (metar)"},
				new NewPartDTO { PartCategoryId = 5, PartName = "Konektor RJ45"},
				new NewPartDTO { PartCategoryId = 5, PartName = "Kapica za konektor RJ45"}
			];

			foreach (var newPart in newPartsList)
			{
				DynamicParameters sqlParameters = new();
				sqlParameters.Add("@PartNameParam", newPart.PartName, DbType.String);

				// Query parts
				string sqlQueryParts = @$"
				SELECT *
				FROM [dbo].[Parts]
				WHERE [PartName] = @PartNameParam
				";

				IEnumerable<PartModel> parts =
					_dapper.QuerySql<PartModel>(sqlQueryParts, sqlParameters);

				if (parts.Any())
				{
					continue;
				}

				// Query part categories
				string partCategoryName = newPart.PartCategoryId switch
				{
					1 => "Diskovi",
					2 => "Memorije",
					3 => "Periferije",
					4 => "Svičevi",
					5 => "Kablovi",
					_ => String.Empty,
				};

				sqlParameters.Add("@PartCategoryNameParam", partCategoryName, DbType.String);

				string sqlQueryPartCategories = @$"
				SELECT *
				FROM [dbo].[PartCategories]
				WHERE [PartCategoryName] = @PartCategoryNameParam
				";

				IEnumerable<PartCategoryModel> partCategories =
					_dapper.QuerySql<PartCategoryModel>(sqlQueryPartCategories, sqlParameters);

				if (partCategories.Any() is not true)
				{
					continue;
				}

				sqlParameters.Add("@PartCategoryIdParam", partCategories.First().PartCategoryId, DbType.Int32);

				// Insert part category
				string sqlExecute = @$"
				INSERT INTO [dbo].[Parts] (
					[PartCategoryId],
					[PartName]
				) VALUES (
					@PartCategoryIdParam,
					@PartNameParam
				)";
				try
				{
					_dapper.ExecuteSql(sqlExecute, sqlParameters);
					counter++;
				}
				catch (Exception)
				{
					throw;
					//continue;
				}
			}

			return Ok($"Dodavanje podataka završeno. Ukupno dodato: {counter}");
		}
	}
}
