using Cgtor.Lib.DbServices;
using Gtor.Utils.DbUtilities;
using Gtor.Utils.TypeUtilities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Cgtor.Lib.Test.DbServices
{
    [TestFixture(TestOf = typeof(SpReadingService)), Category("Unit")]
    public class SpReadingServiceTests
    {
        private IDbReadingService _target;
        private Mock<IDbSysUtils> _dbSysUtilsMock;
        private string _connectionString;
        private string _dbObjectName;
        private IEnumerable<SqlParameter> _testSqlParameters;
        private SqlParameterCollection _testSqlParameterCollection;
        private Mock<ITypeUtils> _typeUtilsMock;

        [SetUp]
        public void SetUp()
        {
            _connectionString = "Data Source=ky1-vrt-msqld1.ky.cafepress.com;Initial Catalog=transit;User ID=cpdba;Password=ithinkgreen; ";
            _dbObjectName = "GET_RATE_GROUP_DEF";
            _dbSysUtilsMock = new Mock<IDbSysUtils>(MockBehavior.Strict);
            _typeUtilsMock = new Mock<ITypeUtils>(MockBehavior.Strict);
            _target = new SpReadingService(_connectionString, _dbObjectName, _typeUtilsMock.Object, _dbSysUtilsMock.Object);
            _testSqlParameters = new List<SqlParameter>();
        }

        [Test]
        public void Test_ReadColumnsFromDbObject_Empty_Constructor()
        {
            // Arrange
            _target = new SpReadingService(null, null, null, null);

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(_target.ConnectionString));
            Assert.IsFalse(string.IsNullOrWhiteSpace(_target.DbObjectName));
        }

        [Test]
        public void Test_ReadColumnsFromDbObject()
        {
            //Arrange
            var command = new SqlCommand();
            _testSqlParameterCollection = command.Parameters;

            _dbSysUtilsMock.Setup(d => d.GetParametersFromSP(_connectionString, _dbObjectName)).Returns(_testSqlParameters);
            _dbSysUtilsMock.Setup(d => d.SetParametersToNull(_testSqlParameterCollection));
            _typeUtilsMock.SetupSequence(m => m.GetFriendlyNameByType(It.IsAny<Type>()))
                .Returns("int")
                .Returns("int")
                .Returns("string")
                .Returns("string")
                .Returns("DateTime");

            // Act
            var result = _target.ReadColumnsFromDbObject().ToList();

            // Assert
            Assert.AreEqual("int", result[0].PropertyDataType);
            Assert.AreEqual("int", result[1].PropertyDataType);
            Assert.AreEqual("string", result[2].PropertyDataType);
            Assert.AreEqual("string", result[3].PropertyDataType);
            Assert.AreEqual("DateTime", result[4].PropertyDataType);
            Assert.IsFalse(result[0].IsNullable);
            Assert.IsFalse(result[1].IsNullable);
            Assert.IsFalse(result[2].IsNullable);
            Assert.IsFalse(result[3].IsNullable);
            Assert.IsFalse(result[4].IsNullable);
        }

        [Test]
        public void Test_ReadColumnsFromDbObject_ArgumentException()
        {
            // Arrange
            _target = new SpReadingService { ConnectionString = null };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _target.ReadColumnsFromDbObject());
        }
    }
}