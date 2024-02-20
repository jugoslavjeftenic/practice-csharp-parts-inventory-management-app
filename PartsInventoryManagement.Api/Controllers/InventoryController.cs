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

			// Query Db for inventory item
			string sqlInventoryItemQueryDb = @$"
				SELECT *
				FROM [dbo].[Inventory]
				WHERE [PartId] = @PartIdParam AND [LocationId] = @LocationIdParam
				";

			IEnumerable<InventoryModel> inventoryItemQueryDb =
				_dapper.QuerySql<InventoryModel>(sqlInventoryItemQueryDb, sqlParameters);

			if (inventoryItemQueryDb.Any())
			{
				return BadRequest("Inventarska stavka već postoji.");
			}

			// Query Db for part
			string sqlPartsQueryDb = @$"
				SELECT *
				FROM [dbo].[Parts]
				WHERE [PartId] = @PartIdParam
				";

			IEnumerable<PartModel> partsQueryDb =
				_dapper.QuerySql<PartModel>(sqlPartsQueryDb, sqlParameters);

			if (partsQueryDb.Any() is not true)
			{
				return BadRequest("Nema traženog dela.");
			}

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
				return BadRequest("Nema tražene lokacije.");
			}

			// Insert inventory item
			string sql = @$"
				INSERT INTO [dbo].[Inventory] (
					[PartId],
					[LocationId],
					[PartQuantity]
				) VALUES (
					@PartIdParam,
					@LocationIdParam,
					@PartQuantityParam
				)";

			if (_dapper.ExecuteSql(sql, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom dodavanja inventarske stavke.");
			}

			return Ok(inventoryItemDTODto);
		}

		// Read
		[HttpGet]
		public IActionResult GetInventoryItems()
		{
			string sql = @$"
				SELECT
					[InventoryId],
					[PartId],
					[LocationId],
					[PartQuantity]
				FROM [dbo].[Inventory]
				";

			IEnumerable<InventoryModel> inventoryItems = _dapper.QuerySql<InventoryModel>(sql);

			return Ok(inventoryItems);
		}

		// Update
		[HttpPut]
		public IActionResult EditInventoryItem(InventoryModel inventoryItem)
		{
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@InventoryIdParam", inventoryItem.InventoryId, DbType.Int32);
			sqlParameters.Add("@PartIdParam", inventoryItem.PartId, DbType.Int32);
			sqlParameters.Add("@LocationIdParam", inventoryItem.LocationId, DbType.Int32);
			sqlParameters.Add("@PartQuantityParam", inventoryItem.PartQuantity, DbType.Int32);

			// Query Db for inventory item
			string sqlInventoryItemQueryDb = @$"
				SELECT *
				FROM [dbo].[Inventory]
				WHERE [InventoryId] = @InventoryIdParam
				";

			IEnumerable<InventoryModel> inventoryItemQueryDb =
				_dapper.QuerySql<InventoryModel>(sqlInventoryItemQueryDb, sqlParameters);

			if (inventoryItemQueryDb.Any() is not true)
			{
				return BadRequest("Nema tražene inventarske stavke.");
			}

			// Query Db for part
			string sqlPartsQueryDb = @$"
				SELECT *
				FROM [dbo].[Parts]
				WHERE [PartId] = @PartIdParam
				";

			IEnumerable<PartModel> partsQueryDb =
				_dapper.QuerySql<PartModel>(sqlPartsQueryDb, sqlParameters);

			if (partsQueryDb.Any() is not true)
			{
				return BadRequest("Nema traženog dela.");
			}

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
				return BadRequest("Nema tražene lokacije.");
			}

			// Update inventory item
			string sql = @$"
				UPDATE [dbo].[Inventory]
				SET
					[PartId] = @PartIdParam,
					[LocationId] = @LocationIdParam,
					[PartQuantity] = @PartQuantityParam
				WHERE [InventoryId] = @InventoryIdParam
				";

			if (_dapper.ExecuteSql(sql, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom izmene dela.");
			}

			return Ok(inventoryItem);
		}

		// Delete
		[HttpDelete("{inventoryId:int}")]
		public IActionResult DeleteInventoryId(int inventoryId)
		{
			if (inventoryId < 1)
			{
				return BadRequest($"Id inventarske stavke mora da bude veći od 0.");
			}

			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@InventoryIdParam", inventoryId, DbType.Int32);

			// Query Db for inventory item
			string sqlInventoryItemQueryDb = @$"
				SELECT *
				FROM [dbo].[Inventory]
				WHERE [InventoryId] = @InventoryIdParam
				";

			IEnumerable<InventoryModel> inventoryItemQueryDb =
				_dapper.QuerySql<InventoryModel>(sqlInventoryItemQueryDb, sqlParameters);

			if (inventoryItemQueryDb.Any() is not true)
			{
				return BadRequest("Nema tražene inventarske stavke.");
			}

			// Delete inventory item
			string sql = @$"
				DELETE
				FROM [dbo].[Inventory]
				WHERE [InventoryId] = @InventoryIdParam
				";

			if (_dapper.ExecuteSql(sql, sqlParameters) is not true)
			{
				return BadRequest("Greška prilikom brisanja inventarske stavke.");
			}

			return Ok(inventoryItemQueryDb);
		}
	}
}
