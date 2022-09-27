using PowerTrackEnterprise.Core.Handlers;
using PowerTrackEnterprise.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

namespace PowerTrackEnterprise.Core.Utility
{
    public class DatabaseDrivers : IDisposable
    {
        #region SQL Database Driver

        private SqlConnection _sqlConnection;

        public List<dynamic> SqlServerQuery(SourceDefinitionDatabaseDTO sourceDefinitionDatabase)
        {
            try
            {
                if (SqlServerConnection(sourceDefinitionDatabase) == null)
                    return new List<dynamic>(); ;

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = _sqlConnection;
                    LogHandler.LogInfo(DateTime.Now.ToString() + " Logged EcWin Connection string : " + cmd.Connection.ConnectionString);

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sourceDefinitionDatabase.Query;
                    cmd.CommandTimeout = 0;

                    var returnData = SqlBindDynamics(cmd.ExecuteReader()).ToList();
                    return returnData.Any() ? returnData : new List<dynamic>();
                }
            }
            catch (Exception e)
            {
                ErrorHandler.Log(e);
                return new List<dynamic>();
            }
        }

        private SqlConnection SqlServerConnection(SourceDefinitionDatabaseDTO sourceDefinitionDatabase)
        {
            try
            {
                var connectionString =
                    $"Data Source={sourceDefinitionDatabase.Server};Initial Catalog={sourceDefinitionDatabase.DatabaseName};User Id={sourceDefinitionDatabase.Username};password={sourceDefinitionDatabase.Password};MultipleActiveResultSets=True;";

                if (_sqlConnection == null)
                {
                    _sqlConnection = new SqlConnection(connectionString);
                    _sqlConnection.Open();
                }
                else
                {
                    if (_sqlConnection.State == ConnectionState.Closed)
                    {
                        _sqlConnection.ConnectionString = connectionString;
                        _sqlConnection.Open();
                    }
                }

                return _sqlConnection;
            }
            catch (Exception e)
            {
                ErrorHandler.Log(e);
                return null;
            }
        }

        private static IEnumerable<dynamic> SqlBindDynamics(SqlDataReader dataReader)
        {
            if (!dataReader.HasRows) yield break;
            while (dataReader.Read())
            {
                dynamic item = new ExpandoObject();
                for (var columnIndex = 0; columnIndex < dataReader.FieldCount; columnIndex++)
                {
                    var resultDictionary = (IDictionary<string, object>)item;

                    var dataValue = dataReader.GetValue(columnIndex);
                    resultDictionary.Add(dataReader.GetName(columnIndex),
                        DBNull.Value.Equals(dataValue) ? null : dataValue);
                }

                yield return item;
            }
        }

        #endregion

        public void Dispose()
        {
            _sqlConnection?.Dispose();

            //GC.Collect();
        }
    }
}
