using System;
using Cgtor.Lib.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Gtor.Utils.DbUtilities;
using Gtor.Utils.TypeUtilities;

namespace Cgtor.Lib.DbServices
{
    public interface IDbReadingService
    {
        string ConnectionString { get; set; }
        string DbObjectName { get; set; }

        IEnumerable<ModelProperty> ReadColumnsFromDbObject();
    }

    public class SpReadingService : IDbReadingService
    {
        public string ConnectionString { get; set; }
        public string DbObjectName { get; set; }
        private readonly ITypeUtils _typeUtils;
        private readonly IDbSysUtils _dbSysUtils;

        public SpReadingService() : this(null, null, null, null) { }

        public SpReadingService(string connectionString, string dbObjectName, ITypeUtils typeUtils, IDbSysUtils dbSysUtils)
        {
            ConnectionString = connectionString ?? ConfigurationManager.AppSettings["connectionString"];
            DbObjectName = dbObjectName ?? ConfigurationManager.AppSettings["dbObjectName"];
            _typeUtils = typeUtils ?? new TypeUtils();
            _dbSysUtils = dbSysUtils ?? new DbSysUtils();
        }

        public IEnumerable<ModelProperty> ReadColumnsFromDbObject()
        {
            try
            {
                if (string.IsNullOrEmpty(ConnectionString))
                {
                    throw new ArgumentException("Connection string is null or empty");
                }

                var properties = new List<ModelProperty>();

                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(DbObjectName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var parameterList = _dbSysUtils.GetParametersFromSP(ConnectionString, DbObjectName).ToArray();

                    command.Parameters.Clear();
                    command.Parameters.AddRange(parameterList);

                    _dbSysUtils.SetParametersToNull(command.Parameters);

                    connection.Open();

                    var sqlDataAdapter = new SqlDataAdapter(command);
                    var dataSet = new DataSet();
                    sqlDataAdapter.FillSchema(dataSet, SchemaType.Source);

                    properties.AddRange(
                        from DataColumn dataColumn in dataSet.Tables[0].Columns
                        select new ModelProperty
                        {
                            PropertyName = dataColumn.ColumnName,
                            PropertyDataType = _typeUtils.GetFriendlyNameByType(dataColumn.DataType),
                            IsNullable = dataColumn.AllowDBNull
                        });

                    connection.Close();
                }
                return properties;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Debug.Flush();
                throw;
            }
        }
    }
}