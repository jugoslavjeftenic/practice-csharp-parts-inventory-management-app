using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartsInventoryManagement.Api.Data;
using System;

namespace PartsInventoryManagement.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TestController(IConfiguration config) : ControllerBase
	{
		private readonly DbContextDapper _dapper = new(config);

		[HttpGet]
		public string TestApplication()
		{
			return "Application is up && running.";
		}

		[HttpGet("TestDbConnection")]
		public DateTime TestConnection()
		{
			return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
		}
	}
}
