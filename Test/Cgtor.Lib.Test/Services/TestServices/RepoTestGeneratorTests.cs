using Cgtor.Lib.Models;
using Cgtor.Lib.Services.TestServices;
using Gtor.Utils.DbUtilities;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Cgtor.Lib.Test.Services.TestServices
{
    [TestFixture(TestOf = typeof(RepoTestGenerator)), Category("Unit")]
    public class RepoTestGeneratorTests
    {
        private Mock<IDbSysUtils> _dbSysUtilsMock;

        private ITestGeneratorService _target;

        private IEnumerable<ModelProperty> _testProperties;
        private string _dbObjectName;
        private string _connectionString;
        private List<string> _testTables;
        private IEnumerable<SqlParameter> _testSqlParameters;

        [SetUp]
        public void SetUp()
        {
            _connectionString = "Data Source=ky1-vrt-msqld1.ky.cafepress.com;" +
                                "Initial Catalog=transit;User ID=cpdba;Password=ithinkgreen";
            _dbSysUtilsMock = new Mock<IDbSysUtils>(MockBehavior.Strict);

            _target = new RepoTestGenerator(_connectionString, _dbSysUtilsMock.Object);

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

            _testTables = new List<string>(
                new[]
                {
                    "RATE_GROUP_DEF"
                }
            );
        }

        [Test]
        public void GenerateTestCode_Test()
        {
            //Arrange
            _dbObjectName = "GET_RATE_GROUP_DEF";
            _testSqlParameters = new List<SqlParameter>();
            _dbSysUtilsMock.Setup(d => d.GetTablesFromSP(_connectionString, _dbObjectName)).Returns(_testTables);
            _dbSysUtilsMock.Setup(d => d.GetParametersFromSP(_connectionString, _dbObjectName)).Returns(_testSqlParameters);

            var testClass = new StringBuilder();

            testClass.AppendLine("[TestFixture, Category(\"Integration\")]");
            testClass.AppendLine("public class RategroupdefRepoTests");
            testClass.AppendLine("{");
            testClass.AppendLine("  private TransactionScope _scope;");
            testClass.AppendLine("  private IRategroupdefRepo _target;");
            testClass.AppendLine();
            testClass.AppendLine("  [OneTimeSetUp]");
            testClass.AppendLine("  public void Setup()");
            testClass.AppendLine("  {");
            testClass.AppendLine("      _scope = new TransactionScope();");
            testClass.AppendLine("      _target = new RategroupdefRepo();");
            testClass.AppendLine("  }");
            testClass.AppendLine();
            testClass.AppendLine("  [Test]");
            testClass.AppendLine("  public void Test_RategroupdefRepo_Methods()");
            testClass.AppendLine("  {");
            testClass.AppendLine("      //Arrange");
            testClass.AppendLine("      const int testRate_group_code = int.MaxValue;");
            testClass.AppendLine("      var rategroupdefRepo = new RategroupdefRepo();");
            testClass.AppendLine("      rategroupdefRepo.Save(new TableRategroupdef { Rate_group_code = testRate_group_code });");
            testClass.AppendLine();
            testClass.AppendLine("      //Act");
            testClass.AppendLine("      _target.Save(new TableRategroupdef");
            testClass.AppendLine("      {");
            testClass.AppendLine("          Rate_group_code = testRate_group_code");
            testClass.AppendLine("      });");
            testClass.AppendLine("      var rategroupdefs = _target.GetAll();");
            testClass.AppendLine();
            testClass.AppendLine("      //Assert");
            testClass.AppendLine("      Assert.IsNotNull(rategroupdefs);");
            testClass.AppendLine("      Assert.AreEqual(rategroupdefs.FirstOrDefault()?.Rate_group_code, testRate_group_code);");
            testClass.AppendLine("  }");
            testClass.AppendLine("}");

            // Act
            var result = _target.GenerateCode(_testProperties, _connectionString, _dbObjectName);

            // Assert
            Assert.IsTrue(testClass.Equals(result));
        }

        [Test]
        public void GenerateTestCode_WithParameter_Test()
        {
            //Arrange
            _dbObjectName = "GET_RATE_GROUP_BY_RATE_GROUP_CODE";
            _testSqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@inORDER_NO", SqlDbType.Int)
            };
            _dbSysUtilsMock.Setup(d => d.GetTablesFromSP(_connectionString, _dbObjectName)).Returns(_testTables);
            _dbSysUtilsMock.Setup(d => d.GetParametersFromSP(_connectionString, _dbObjectName)).Returns(_testSqlParameters);

            var testClass = new StringBuilder();

            testClass.AppendLine("[TestFixture, Category(\"Integration\")]");
            testClass.AppendLine("public class RategroupdefRepoTests");
            testClass.AppendLine("{");
            testClass.AppendLine("  private TransactionScope _scope;");
            testClass.AppendLine("  private IRategroupdefRepo _target;");
            testClass.AppendLine();
            testClass.AppendLine("  [OneTimeSetUp]");
            testClass.AppendLine("  public void Setup()");
            testClass.AppendLine("  {");
            testClass.AppendLine("      _scope = new TransactionScope();");
            testClass.AppendLine("      _target = new RategroupdefRepo();");
            testClass.AppendLine("  }");
            testClass.AppendLine();
            testClass.AppendLine("  [Test]");
            testClass.AppendLine("  public void Test_RategroupdefRepo_Methods()");
            testClass.AppendLine("  {");
            testClass.AppendLine("      //Arrange");
            testClass.AppendLine("      const int testRate_group_code = int.MaxValue;");
            testClass.AppendLine("      var rategroupdefRepo = new RategroupdefRepo();");
            testClass.AppendLine("      rategroupdefRepo.Save(new TableRategroupdef { Rate_group_code = testRate_group_code });");
            testClass.AppendLine();
            testClass.AppendLine("      //Act");
            testClass.AppendLine("      _target.Save(new TableRategroupdef");
            testClass.AppendLine("      {");
            testClass.AppendLine("          Rate_group_code = testRate_group_code");
            testClass.AppendLine("      });");
            testClass.AppendLine("      var rategroupdefs = _target.GetByRate_group_code(testRate_group_code);");
            testClass.AppendLine();
            testClass.AppendLine("      //Assert");
            testClass.AppendLine("      Assert.IsNotNull(rategroupdefs);");
            testClass.AppendLine("      Assert.AreEqual(rategroupdefs.FirstOrDefault()?.Rate_group_code, testRate_group_code);");
            testClass.AppendLine("  }");
            testClass.AppendLine("}");

            // Act
            var result = _target.GenerateCode(_testProperties, _connectionString, _dbObjectName);

            // Assert
            Assert.IsTrue(testClass.Equals(result));
        }  
    }
}
