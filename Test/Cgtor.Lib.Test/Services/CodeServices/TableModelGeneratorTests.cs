using Cgtor.Lib.Controllers;
using Cgtor.Lib.Models;
using Cgtor.Lib.Services.CodeServices;
using Gtor.Utils.DbUtilities;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace Cgtor.Lib.Test.Services.CodeServices
{
    [TestFixture(TestOf = typeof(TableModelGenerator)), Category("Unit")]
    public class TableModelGeneratorTests
    {
        private ICodeGeneratorService _target;
        private string _connectionString;
        private List<ModelProperty> _testModelProperties;
        private string _dbObjectName;
        private Mock<IModelPropertyController> _modelPropertyControllerMock;
        private Mock<IDbSysUtils> _dbSysUtilsMock;
        private IEnumerable<string> _testTables;

        [SetUp]
        public void SetUp()
        {
            _connectionString = "Data Source=ky1-vrt-msqld1.ky.cafepress.com;" +
                                "Initial Catalog=transit;User ID=cpdba;Password=ithinkgreen";

            _modelPropertyControllerMock = new Mock<IModelPropertyController>(MockBehavior.Strict);
            _dbSysUtilsMock = new Mock<IDbSysUtils>(MockBehavior.Strict);

            _target = new TableModelGenerator(_connectionString, _modelPropertyControllerMock.Object, _dbSysUtilsMock.Object);

            _testTables = new List<string>(
                new[]
                {
                    "RATE_GROUP_DEF"
                }
            );
            _testModelProperties = new List<ModelProperty>
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
            _dbObjectName = "GET_RATE_GROUP_BY_GROUP_CODE";
        }

        [Test]
        public void GenerateCode_Test()
        {
            // Arrange
            _modelPropertyControllerMock.Setup(m => m.GetNullableMarkIfApplicable(_testModelProperties[0])).Returns("");
            _modelPropertyControllerMock.Setup(m => m.GetNullableMarkIfApplicable(_testModelProperties[1])).Returns("");
            _modelPropertyControllerMock.Setup(m => m.GetNullableMarkIfApplicable(_testModelProperties[2])).Returns("");
            _modelPropertyControllerMock.Setup(m => m.GetNullableMarkIfApplicable(_testModelProperties[3])).Returns("");
            _modelPropertyControllerMock.Setup(m => m.GetNullableMarkIfApplicable(_testModelProperties[4])).Returns("");
            _dbSysUtilsMock.Setup(d => d.GetTablesFromSP(_connectionString, _dbObjectName)).Returns(_testTables);

            var testClass = new StringBuilder();
            testClass.AppendLine("public class TableRate_group_def");
            testClass.AppendLine("{");
            testClass.AppendLine("  public int Rate_group_code { get; set; }");
            testClass.AppendLine("  public int Service_level_no { get; set; }");
            testClass.AppendLine("  public string Rate_group_description { get; set; }");
            testClass.AppendLine("  public string User_id { get; set; }");
            testClass.AppendLine("  public DateTimeOffset Record_insert_timestamp { get; set; }");
            testClass.AppendLine("}");

            // Act
            var result = _target.GenerateCode(_testModelProperties, _connectionString, _dbObjectName);

            // Assert
            Assert.IsTrue(testClass.Equals(result));
        }
    }
}