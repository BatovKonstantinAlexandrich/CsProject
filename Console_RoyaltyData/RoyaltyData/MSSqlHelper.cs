using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace WebApi.Data
{
    public class MSSqlHelper
    {
        // Установить соединение, команду, а затем выполнить команду без запроса.
        //public static int ExecuteNonQuery(String connectionString, String commandText,
        //    CommandType commandType, params SqlParameter[] parameters)
        public async Task<int> ExecuteNonQueryAsync(String connectionString, String commandText,
            CommandType commandType, Action<SqlParameterCollection> addParameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    // There're three command types: StoredProcedure, Text, TableDirect. The TableDirect   
                    // type is only for OLE DB.    
                    cmd.CommandType = commandType;
                    addParameters(cmd.Parameters);

                    await conn.OpenAsync();
                    return (int)await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // Установить соединение, команду, а затем выполнить команду и вернуть только одно значение.
        public async Task<object> ExecuteScalarAsync(String connectionString, String commandText,
            CommandType commandType, Action<SqlParameterCollection> addParameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    addParameters(cmd.Parameters);

                    await conn.OpenAsync();
                    return await cmd.ExecuteScalarAsync();
                }
            }
        }

        // Установить соединение, команду, а затем выполнить команду с запросом и вернуть Reader.
        public async Task<SqlDataReader> ExecuteReaderAsync(String connectionString, String commandText,
            CommandType commandType, Action<SqlParameterCollection> addParameters)
        {
            SqlConnection conn = new SqlConnection(connectionString);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = commandType;
                cmd.CommandTimeout = cmd.CommandTimeout * 10;
                addParameters(cmd.Parameters);

                await conn.OpenAsync();
                return (SqlDataReader)await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
        }
    }
}