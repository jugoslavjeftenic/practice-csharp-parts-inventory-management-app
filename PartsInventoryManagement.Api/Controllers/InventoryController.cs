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
	public class InventoryController(IConfiguration config) : ControllerBase
	{
		private readonly DbContextDapper _dapper = new(config);

		//Create
		[HttpPost]
		public IActionResult AddInventoryItem(NewInventoryItemDTO inventoryItemDTODto)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartIdParam", inventoryItemDTODto.PartId, DbType.Int32);
			sqlParameters.Add("@LocationIdParam", inventoryItemDTODto.LocationId, DbType.Int32);
			sqlParameters.Add("@PartQuantityParam", inventoryItemDTODto.PartQuantity, DbType.Int32);

			// Query inventory
			string sqlQueryInventory = @$"
				SELECT *
				FROM [dbo].[Inventory]
				WHERE [PartId] = @PartIdParam AND [LocationId] = @LocationIdParam
				";

			IEnumerable<InventoryItemModel> inventory =
				_dapper.QuerySql<InventoryItemModel>(sqlQueryInventory, sqlParameters);

			if (inventory.Any())
			{
				return BadRequest("Inventarni predmet već postoji.");
			}

			// Query parts
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

			// Insert inventory item
			string sqlExecute = @$"
				INSERT INTO [dbo].[Inventory] (
					[PartId],
					[LocationId],
					[PartQuantity]
				) VALUES (
					@PartIdParam,
					@LocationIdParam,
					@PartQuantityParam
				)";

			if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom dodavanja inventarnog predmeta.");
			}

			return Ok(inventoryItemDTODto);
		}

		// Read
		[HttpGet]
		public IActionResult GetInventory()
		{
			string sqlQuery = @$"
				SELECT
					[InventoryId],
					[PartId],
					[LocationId],
					[PartQuantity]
				FROM [dbo].[Inventory]
				";

			IEnumerable<InventoryItemModel> inventory = _dapper.QuerySql<InventoryItemModel>(sqlQuery);

			return Ok(inventory);
		}

		// Update
		[HttpPut]
		public IActionResult EditInventoryItem(InventoryItemModel inventoryItem)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@InventoryIdParam", inventoryItem.InventoryId, DbType.Int32);
			sqlParameters.Add("@PartIdParam", inventoryItem.PartId, DbType.Int32);
			sqlParameters.Add("@LocationIdParam", inventoryItem.LocationId, DbType.Int32);
			sqlParameters.Add("@PartQuantityParam", inventoryItem.PartQuantity, DbType.Int32);

			// Query inventory
			string sqlQueryInventory = @$"
				SELECT *
				FROM [dbo].[Inventory]
				WHERE [InventoryId] = @InventoryIdParam
				";

			IEnumerable<InventoryItemModel> inventory =
				_dapper.QuerySql<InventoryItemModel>(sqlQueryInventory, sqlParameters);

			if (inventory.Any() is not true)
			{
				return BadRequest("Nema traženog inventarnog predmeta.");
			}

			// Query parts
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

			// Update inventory item
			string sqlExecute = @$"
				UPDATE [dbo].[Inventory]
				SET
					[PartId] = @PartIdParam,
					[LocationId] = @LocationIdParam,
					[PartQuantity] = @PartQuantityParam
				WHERE [InventoryId] = @InventoryIdParam
				";

			if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom izmene inventarnog predmeta.");
			}

			return Ok(inventoryItem);
		}

		// Delete
		[HttpDelete("{inventoryId:int}")]
		public IActionResult DeleteInventoryItem(int inventoryId)
		{
			if (inventoryId < 1)
			{
				return BadRequest($"Id inventarnog predmeta mora da bude veći od 0.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@InventoryIdParam", inventoryId, DbType.Int32);

			// Query inventory
			string sqlQueryInventory = @$"
				SELECT *
				FROM [dbo].[Inventory]
				WHERE [InventoryId] = @InventoryIdParam
				";

			IEnumerable<InventoryItemModel> inventory =
				_dapper.QuerySql<InventoryItemModel>(sqlQueryInventory, sqlParameters);

			if (inventory.Any() is not true)
			{
				return BadRequest("Nema traženog inventarnog predmeta.");
			}

			// Delete inventory item
			string sqlExecute = @$"
				DELETE
				FROM [dbo].[Inventory]
				WHERE [InventoryId] = @InventoryIdParam
				";

			if (_dapper.ExecuteSql(sqlExecute, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom brisanja inventarnog predmeta.");
			}

			return Ok(inventory);
		}

		// Read ById
		[HttpGet("{inventoryId:int}")]
		public IActionResult GetInventoryItemById(int inventoryId)
		{
			if (inventoryId < 1)
			{
				return BadRequest($"Id inventarnog predmeta mora da bude veći od 0.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@InventoryIdParam", inventoryId, DbType.Int32);

			// Query inventory
			string sqlQuery = @$"
				SELECT
					[InventoryId],
					[PartId],
					[LocationId],
					[PartQuantity]
				FROM [dbo].[Inventory]
				WHERE [InventoryId] = @InventoryIdParam
				";

			IEnumerable<InventoryItemModel> inventory =
				_dapper.QuerySql<InventoryItemModel>(sqlQuery, sqlParameters);

			return Ok(inventory);
		}

		// Read ByPartId
		[HttpGet("part/{partId:int}")]
		public IActionResult GetInventoryByPartId(int partId)
		{
			if (partId < 1)
			{
				return BadRequest($"Id dela mora da bude veći od 0.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PartIdParam", partId, DbType.Int32);

			// Query inventory
			string sqlQuery = @$"
				SELECT
					[InventoryId],
					[PartId],
					[LocationId],
					[PartQuantity]
				FROM [dbo].[Inventory]
				WHERE [PartId] = @PartIdParam
				";

			IEnumerable<InventoryItemModel> inventory =
				_dapper.QuerySql<InventoryItemModel>(sqlQuery, sqlParameters);

			return Ok(inventory);
		}

		// Read ByLocationId
		[HttpGet("location/{locationId:int}")]
		public IActionResult GetInventoryByLocationId(int locationId)
		{
			if (locationId < 1)
			{
				return BadRequest($"Id lokacije mora da bude veći od 0.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@LocationIdParam", locationId, DbType.Int32);

			// Query inventory
			string sqlQuery = @$"
				SELECT
					[InventoryId],
					[PartId],
					[LocationId],
					[PartQuantity]
				FROM [dbo].[Inventory]
				WHERE [LocationId] = @LocationIdParam
				";

			IEnumerable<InventoryItemModel> inventory =
				_dapper.QuerySql<InventoryItemModel>(sqlQuery, sqlParameters);

			return Ok(inventory);
		}

		//Seed
		[HttpPost("randomseed")]
		public IActionResult RandomSeedInventory()
		{
			int counter = 0;
			Random random = new();

			// Query parts
			string sqlQueryParts = @$"
				SELECT [PartId]
				FROM [dbo].[Parts]
				";
			int[] partsIdsArray = _dapper.QuerySql<int>(sqlQueryParts).ToArray();

			// Query locations
			string sqlQueryLocations = @$"
				SELECT [LocationId]
				FROM [dbo].[Locations]
				";
			int[] locationsIdsArray = _dapper.QuerySql<int>(sqlQueryLocations).ToArray();

			for (int i = 0; i < 10; i++)
			{
				int randomPart = partsIdsArray[random.Next(0, partsIdsArray.Length)];
				int randomLocation = locationsIdsArray[random.Next(0, locationsIdsArray.Length)];

				// Query inventory
				string sqlQueryInventory = @$"
				SELECT *
				FROM [dbo].[Inventory]
				WHERE [PartId] = {randomPart} AND [LocationId] = {randomLocation}
				";

				IEnumerable<NewInventoryItemDTO> inventory =
					_dapper.QuerySql<NewInventoryItemDTO>(sqlQueryInventory);

				if (inventory.Any())
				{
					continue;
				}

				// Insert inventory item
				string sqlExecute = @$"
				INSERT INTO [dbo].[Inventory] (
					[PartId],
					[LocationId],
					[PartQuantity]
				) VALUES (
					{randomPart},
					{randomLocation},
					{random.Next(1, 51)}
				)";

				try
				{
					_dapper.ExecuteSql(sqlExecute);
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
