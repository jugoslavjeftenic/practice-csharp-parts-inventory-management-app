﻿using Microsoft.AspNetCore.Mvc;
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
		public IActionResult TestApplication()
		{
			return Ok("Application is up && running.");
		}

		[HttpGet("/DbConnection")]
		public IActionResult TestConnection()
		{
			return Ok(_dapper.QuerySql<DateTime>("SELECT GETDATE()"));
		}
	}
}
