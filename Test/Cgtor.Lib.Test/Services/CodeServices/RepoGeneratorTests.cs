using Cgtor.Lib.Models;
using Cgtor.Lib.Services.CodeServices;
using Gtor.Utils.DbUtilities;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Cgtor.Lib.Test.Services.CodeServices
{
    [TestFixture(TestOf = typeof(RepoGenerator)), Category("Unit")]
    public class RepoGeneratorTests
    {
        private Mock<IDbSysUtils> _dbSysUtilsMock;

        private ICodeGeneratorService _target;

        private IEnumerable<ModelProperty> _testProperties;
        private string _dbObjectName;
        private string _dbDataContext;
        private string _connectionString;
        private IEnumerable<string> _testParameters;
        private IEnumerable<SqlParameter> _testSqlParameters;

        [SetUp]
        public void SetUp()
        {
            _connectionString = "Data Source=ky1-vrt-msqld1.ky.cafepress.com;" +
                                "Initial Catalog=transit;User ID=cpdba;Password=ithinkgreen";
            _dbSysUtilsMock = new Mock<IDbSysUtils>(MockBehavior.Strict);

            _target = new RepoGenerator(_connectionString, _dbSysUtilsMock.Object);

            _testProperties = new List<ModelProperty>
            {
                new ModelProperty
                {
                    PropertyDataType = "int",
                    PropertyName = "RATE_GROUP_CODE",
                    IsNullable = false
                },
                new ModelProperty
                {
                    PropertyDataType = "int",
                    PropertyName = "SERVICE_LEVEL_NO",
                    IsNullable = false
                },
                new ModelProperty
                {
                    PropertyDataType = "string",
                    PropertyName = "RATE_GROUP_DESCRIPTION",
                    IsNullable = false
                },
                new ModelProperty
                {
                    PropertyDataType = "string",
                    PropertyName = "USER_ID",
                    IsNullable = false
                },
                new ModelProperty
                {
                    PropertyDataType = "DateTimeOffset",
                    PropertyName = "RECORD_INSERT_TIMESTAMP",
                    IsNullable = false
                }
            };

            _testParameters = new List<string>(
                new[]
                {
                    "RATE_GROUP_DEF"
                }
            );
            _dbDataContext = "TransitDbDataContext";
        }

        [Test]
        public void Test_GenerateCode()
        {
            // Arrange
            _dbObjectName = "GET_RATE_GROUP_DEF";
            _target.DbDataContext = _dbDataContext;
            _testSqlParameters = new List<SqlParameter>();
            _dbSysUtilsMock.Setup(d => d.GetTablesFromSP(_connectionString, _dbObjectName)).Returns(_testParameters);
            _dbSysUtilsMock.Setup(d => d.GetParametersFromSP(_connectionString, _dbObjectName)).Returns(_testSqlParameters);

            var testClass = new StringBuilder();

            testClass.AppendLine("public interface IRategroupdefRepo");
            testClass.AppendLine("{");
            testClass.AppendLine("  IEnumerable<TableRategroupdef> GetAll();");
            testClass.AppendLine("  void Save(TableRategroupdef tableRategroupdef);");
            testClass.AppendLine("}");
            testClass.AppendLine();
            testClass.AppendLine("public class RategroupdefRepo : DataAccessBaseRepo, IRategroupdefRepo");
            testClass.AppendLine("{");
            testClass.AppendLine("  public IEnumerable<TableRategroupdef> GetAll()");
            testClass.AppendLine("  {");
            testClass.AppendLine("      var tableRategroupdef = new List<TableRategroupdef>();");
            testClass.AppendLine();
            testClass.AppendLine("      using (var db = new TransitDbDataContext(ConnectionString))");
            testClass.AppendLine("      {");
            testClass.AppendLine("          var dataList = db.GET_RATE_GROUP_DEF().ToList();");
            testClass.AppendLine("          tableRategroupdef.AddRange(dataList.Select(dataRategroupdef => new TableRategroupdef");
            testClass.AppendLine("          {");
            testClass.AppendLine("              Rate_group_code = dataRategroupdef.Rate_group_code,");
            testClass.AppendLine("              Service_level_no = dataRategroupdef.Service_level_no,");
            testClass.AppendLine("              Rate_group_description = dataRategroupdef.Rate_group_description,");
            testClass.AppendLine("              User_id = dataRategroupdef.User_id,");
            testClass.AppendLine("              Record_insert_timestamp = dataRategroupdef.Record_insert_timestamp.UtcDateTime");
            testClass.AppendLine("          }));");
            testClass.AppendLine("      }");
            testClass.AppendLine();
            testClass.AppendLine("      return tableRategroupdef;");
            testClass.AppendLine("  }");
            testClass.AppendLine();
            testClass.AppendLine("  public void Save(TableRategroupdef tableRategroupdef)");
            testClass.AppendLine("  {");
            testClass.AppendLine("      using (var db = new TransitDbDataContext(ConnectionString))");
            testClass.AppendLine("      {");
            testClass.AppendLine("          db.SAVE_RATE_GROUP_DEF(tableRategroupdef.Rate_group_code, tableRategroupdef.Service_level_no, tableRategroupdef.Rate_group_description, tableRategroupdef.User_id, tableRategroupdef.Record_insert_timestamp);");
            testClass.AppendLine("      }");
            testClass.AppendLine("  }");
            testClass.AppendLine("}");

            // Act
            var result = _target.GenerateCode(_testProperties, _connectionString, _dbObjectName);

            // Assert
            Assert.IsTrue(testClass.Equals(result));
        }

        [Test]
        public void Test_GenerateCode_WithParameters()
        {
            // Arrange
            _dbObjectName = "GET_RATE_GROUP_BY_RATE_GROUP_CODE";
            _target.DbDataContext = _dbDataContext;
            _testSqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@inRATE_GROUP_CODE", SqlDbType.Int)
            };
            _dbSysUtilsMock.Setup(d => d.GetTablesFromSP(_connectionString, _dbObjectName)).Returns(_testParameters);
            _dbSysUtilsMock.Setup(d => d.GetParametersFromSP(_connectionString, _dbObjectName)).Returns(_testSqlParameters);

            var testClass = new StringBuilder();

            testClass.AppendLine("  public TableRategroupdef GetByRate_group_code(int rate_group_code);");
            testClass.AppendLine("  void Save(TableRategroupdef tableRategroupdef);");
            testClass.AppendLine();
            testClass.AppendLine("  public TableRategroupdef GetByRate_group_code(int rate_group_code)");
            testClass.AppendLine("  {");
            testClass.AppendLine("      TableRategroupdef tableRategroupdef;");
            testClass.AppendLine();
            testClass.AppendLine("      using (var db = new TransitDbDataContext(ConnectionString))");
            testClass.AppendLine("      {");
            testClass.AppendLine("          var dataList = db.GET_RATE_GROUP_BY_RATE_GROUP_CODE(rate_group_code).FirstOrDefault();");
            testClass.AppendLine("          tableRategroupdef = new TableRategroupdef");
            testClass.AppendLine("          {");
            testClass.AppendLine("              Rate_group_code = dataRategroupdef.Rate_group_code,");
            testClass.AppendLine("              Service_level_no = dataRategroupdef.Service_level_no,");
            testClass.AppendLine("              Rate_group_description = dataRategroupdef.Rate_group_description,");
            testClass.AppendLine("              User_id = dataRategroupdef.User_id,");
            testClass.AppendLine("              Record_insert_timestamp = dataRategroupdef.Record_insert_timestamp.UtcDateTime");
            testClass.AppendLine("          };");
            testClass.AppendLine("      }");
            testClass.AppendLine();
            testClass.AppendLine("      return tableRategroupdef;");
            testClass.AppendLine("  }");
            testClass.AppendLine();
            testClass.AppendLine("  public void Save(TableRategroupdef tableRategroupdef)");
            testClass.AppendLine("  {");
            testClass.AppendLine("      using (var db = new TransitDbDataContext(ConnectionString))");
            testClass.AppendLine("      {");
            testClass.AppendLine("          db.SAVE_RATE_GROUP_DEF(tableRategroupdef.Rate_group_code, tableRategroupdef.Service_level_no, tableRategroupdef.Rate_group_description, tableRategroupdef.User_id, tableRategroupdef.Record_insert_timestamp);");
            testClass.AppendLine("      }");
            testClass.AppendLine("  }");

            // Act
            var result = _target.GenerateCode(_testProperties, _connectionString, _dbObjectName);

            // Assert
            Assert.IsTrue(testClass.Equals(result));
        }
    }
}