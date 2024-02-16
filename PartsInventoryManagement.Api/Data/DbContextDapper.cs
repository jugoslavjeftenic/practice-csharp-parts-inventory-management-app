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

		public IEnumerable<T> LoadData<T>(string sql)
		{
			IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			return dbConnection.Query<T>(sql);
		}

		public T LoadDataSingle<T>(string sql)
		{
			IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			return dbConnection.QuerySingle<T>(sql);
		}

		public IEnumerable<T> LoadDataWithParameters<T>(string sql, DynamicParameters parameters)
		{
			IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			return dbConnection.Query<T>(sql, parameters);
		}

		public T LoadDataSingleWithParameters<T>(string sql, DynamicParameters parameters)
		{
			IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			return dbConnection.QuerySingle<T>(sql, parameters);
		}

		public bool ExecuteSql(string sql)
		{
			IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			return dbConnection.Execute(sql) > 0;
		}

		public int ExecuteSqlWithRowCount(string sql)
		{
			IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			return dbConnection.Execute(sql);
		}

		public bool ExecuteSqlWithParameters(string sql, DynamicParameters parameters)
		{
			IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			return dbConnection.Execute(sql, parameters) > 0;
		}
	}
}
