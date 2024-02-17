using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartsInventoryManagement.Api.Data;
using PartsInventoryManagement.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PartsInventoryManagement.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PartsController(IConfiguration config) : ControllerBase
	{
		private readonly DbContextDapper _dapper = new(config);

		// Create
		//[HttpPost]
		//public IActionResult AddUser(UserToAddDto user)
		//{
		//	string sql = @$"
		//	INSERT INTO [TutorialAppSchema].[Users] (
		//		[FirstName],
		//		[LastName],
		//		[Email],
		//		[Gender],
		//		[Active]
		//	) VALUES (
		//		'{user.FirstName}',
		//		'{user.LastName}',
		//		'{user.Email}',
		//		'{user.Gender}',
		//		'{user.Active}'
		//	)";

		//	if (_dapper.ExecuteSql(sql))
		//	{
		//		return Ok();
		//	}

		//	throw new Exception("Failed to Add user");
		//}

		// Read
		[HttpGet]
		public ActionResult<IEnumerable<PartModel>> GetParts()
		{
			string sql = "SELECT [PartId], [PartCategory], [PartName] FROM dbo.Parts";

			IEnumerable<PartModel> parts = _dapper.QuerySql<PartModel>(sql);

			if (parts == null || !parts.Any())
			{
				return NotFound("Nisu pronađeni podaci.");
			}

			return Ok(parts);
		}
	}
}
