using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace PartsInventoryManagement.Api.Data
{
	public class DbContextDapper(IConfiguration config)
	{
		private readonly IConfiguration _config = config;

		public IEnumerable<T> QuerySql<T>(string sql, DynamicParameters? parameters = null)
		{
			using IDbConnection dbConnection =
				new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			return dbConnection.Query<T>(sql, parameters);
		}

		public bool ExecuteSql(string sql, DynamicParameters? parameters = null)
		{
			using IDbConnection dbConnection =
				new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			return dbConnection.Execute(sql, parameters) > 0;
		}
	}
}
