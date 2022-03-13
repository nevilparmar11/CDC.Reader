using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace CDC.Reader.Application.Extension
{
    public class SqlExtension : ISqlExtension
    {

        private readonly IConfiguration configuration;

        public SqlExtension(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private static SqlConnection GetSqlConnection(string connectionString)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            return sqlConnection;
        }

        public async Task<DataSet> ExecuteCommand(string connectionString, string commandName, SqlParameter[] parameters)
        {
            DataSet dataSet = new DataSet();

            SqlConnection sqlConnection = GetSqlConnection(connectionString);

            SqlCommand command = new SqlCommand(commandName, sqlConnection);
            command.Parameters.AddRange(parameters);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = Convert.ToInt32(configuration["CommandTimeOut"]);

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);

            sqlConnection.Open();
            await Task.Run(() => sqlDataAdapter.Fill(dataSet)).ConfigureAwait(false);
            sqlConnection.Close();

            return dataSet;
        }

        public async Task<SqlParameter[]> ExecuteNonQuery(string connectionString, string commandName, SqlParameter[] parameters)
        {
            SqlConnection sqlConnection = GetSqlConnection(connectionString);

            SqlCommand command = new SqlCommand(commandName, sqlConnection);
            command.Parameters.AddRange(parameters);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = Convert.ToInt32(configuration["CommandTimeOut"]);

            sqlConnection.Open();
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            sqlConnection.Close();

            return parameters;
        }

        public async Task<T> Get<T>(string query, object parameters = null, string mirrorConnectionString = "ConnectionStrings:CDCMirror", string connectionString = "ConnectionStrings:CDCMain")
        {
            T result;
            using (IDbConnection connection = new SqlConnection(configuration[mirrorConnectionString]))
            {
                connection.Open();
                result = await connection.QueryFirstOrDefaultAsync<T>(query, parameters, commandType: CommandType.Text).ConfigureAwait(false);
                connection.Close();
            }
            if (result == null)
            {
                using (IDbConnection connection = new SqlConnection(configuration[connectionString]))
                {
                    connection.Open();
                    result = await connection.QueryFirstOrDefaultAsync<T>(query, parameters, commandType: CommandType.Text).ConfigureAwait(false);
                    connection.Close();
                }
            }
            return result;
        }
        public async Task<IEnumerable<T>> GetList<T>(string sqlQuery, object parameters = null, string mirrorConnectionString = "ConnectionStrings:CDCMirror", string connectionString = "ConnectionStrings:CDCMain")
        {
            IEnumerable<T> result;
            using (IDbConnection connection = new SqlConnection(configuration[mirrorConnectionString]))
            {
                connection.Open();
                result = await connection.QueryAsync<T>(sqlQuery, parameters, commandType: CommandType.Text).ConfigureAwait(false);
                connection.Close();
            }
            if (result?.Any() == false)
            {
                using (IDbConnection connection = new SqlConnection(configuration[connectionString]))
                {
                    connection.Open();
                    result = await connection.QueryAsync<T>(sqlQuery, parameters, commandType: CommandType.Text).ConfigureAwait(false);
                    connection.Close();
                }
            }
            return result;
        }

    }
}
