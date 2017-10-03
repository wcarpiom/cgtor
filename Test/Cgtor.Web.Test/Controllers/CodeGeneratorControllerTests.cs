using Cgtor.Lib.DbServices;
using Cgtor.Lib.Models;
using Cgtor.Web.Controllers;
using Cgtor.Web.ViewModels;
using Gtor.Utils.DbUtilities;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace Cgtor.Web.Test.Controllers
{
    [Category("Integration")]
    [TestFixture(TestOf = typeof(CodeGeneratorController))]
    public class CodeGeneratorControllerTests
    {
        private Mock<IDbReadingService> _dbReadingServiceMock;
        private Mock<IDbSysUtils> _dbSysUtilsMock;

        private CodeGeneratorController _target;

        private CodeServiceType _codeServiceType;
        private List<ModelProperty> _testModelProperties;
        private string _connectionString;
        private string _dbObjectName;
        private IEnumerable<string> _testTables;

        [SetUp]
        public void SetUp()
        {
            _dbReadingServiceMock = new Mock<IDbReadingService>(MockBehavior.Strict);
            _dbSysUtilsMock = new Mock<IDbSysUtils>(MockBehavior.Strict);

            _target = new CodeGeneratorController(_dbReadingServiceMock.Object, _dbSysUtilsMock.Object);

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

            _testTables = new List<string>(
                new[]
                {
                    "RATE_GROUP_DEF"
                }
            );
            _connectionString =
                "Data Source=ky1-vrt-msqld1.ky.cafepress.com;Initial Catalog=transit;User ID=cpdba;Password=ithinkgreen; ";

            _dbObjectName = "GET_RATE_GROUP_BY_GROUP_CODE";
            _codeServiceType = CodeServiceType.TableModel;
        }

        [Test]
        public void Test_Index_PreloadDefaultValues()
        {
            // Arrange
            var expectedConnectionString = ConfigurationManager.AppSettings["connectionString"];
            var expectedStoredProcedure = ConfigurationManager.AppSettings["dbObjectName"];

            // Act
            var result = _target.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(((GeneratedModelCodeViewModel) result.ViewData.Model).DefaultConnectionString,
                expectedConnectionString);
            Assert.AreEqual(((GeneratedModelCodeViewModel) result.ViewData.Model).DefaultStoredProcedure,
                expectedStoredProcedure);
        }

        [Test]
        public void Test_GenerateModel()
        {
            //Arrange   
            _dbReadingServiceMock.Setup(r => r.ReadColumnsFromDbObject())
                .Returns(_testModelProperties);
            _dbReadingServiceMock.SetupSet(r => r.ConnectionString = _connectionString);
            _dbReadingServiceMock.SetupSet(r => r.DbObjectName = _dbObjectName);
            _dbSysUtilsMock.Setup(d => d.GetTablesFromSP(_connectionString, _dbObjectName)).Returns(_testTables);


            //Act
            var result = _target.GenerateModel(_connectionString, _dbObjectName, _codeServiceType) as ViewResult;

            //Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(((GeneratedModelCodeViewModel) result.ViewData.Model).ModelCode);
            Assert.IsNotNull(((GeneratedModelCodeViewModel) result.ViewData.Model).ModelName);
            Assert.IsNotNull(((GeneratedModelCodeViewModel) result.ViewData.Model).CodeServiceType);
            Assert.AreEqual(((GeneratedModelCodeViewModel) result.ViewData.Model).CodeServiceType, _codeServiceType);
        }

        [Test]
        public void Test_GenerateModel_IsNull_ActionMethod()
        {
            // Act
            var result = _target.GenerateModel(null, null, _codeServiceType) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(((GeneratedModelCodeViewModel) result.ViewData.Model).ModelCode);
        }
    }
}
