using Cgtor.Lib.Models;
using Gtor.Utils.DbUtilities;
using Gtor.Utils.Models;
using Gtor.Utils.StringUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace Cgtor.Lib.Services.TestServices
{
    public class RepoTestGenerator : ITestGeneratorService
    {
        private readonly IDbSysUtils _dbSysUtils;
        private string _connectionString;

        public RepoTestGenerator() : this(null, null) { }

        public RepoTestGenerator(string connectionString, IDbSysUtils dbSysUtils)
        {
            _dbSysUtils = dbSysUtils ?? new DbSysUtils();
            _connectionString = connectionString ?? WebConfigurationManager.AppSettings["connectionString"];
        }

        public StringBuilder GenerateCode(IEnumerable<ModelProperty> properties, string connectionString, string dbObjectName)
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                _connectionString = connectionString;
            }
            var modelProperties = properties.ToList();
            var tableName = _dbSysUtils.GetTablesFromSP(_connectionString, dbObjectName).ToList().FirstOrDefault();
            var firstProperty = modelProperties.FirstOrDefault();
            var parametersExist = _dbSysUtils.GetParametersFromSP(_connectionString, dbObjectName).Any();

            if (tableName == null || firstProperty == null)
            {
                throw new ArgumentException($"It's not possible to retrieve the first table name from this SP: {dbObjectName}");
            }
            tableName = tableName.TransformTo(CaseType.SentenceCase).Replace("_", "");
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("[TestFixture, Category(\"Integration\")]");
            stringBuilder.AppendLine($"public class {tableName}RepoTests");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("  private TransactionScope _scope;");
            stringBuilder.AppendLine($"  private I{tableName}Repo _target;");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("  [OneTimeSetUp]");
            stringBuilder.AppendLine("  public void Setup()");
            stringBuilder.AppendLine("  {");
            stringBuilder.AppendLine("      _scope = new TransactionScope();");
            stringBuilder.AppendLine($"      _target = new {tableName}Repo();");
            stringBuilder.AppendLine("  }");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("  [Test]");
            stringBuilder.AppendLine($"  public void Test_{tableName}Repo_Methods()");
            stringBuilder.AppendLine("  {");
            stringBuilder.AppendLine("      //Arrange");
            stringBuilder.AppendLine($"      const int test{firstProperty.PropertyName.TransformTo(CaseType.SentenceCase)} = int.MaxValue;");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("      //Act");
            stringBuilder.AppendLine($"      _target.Save(new Table{tableName}");
            stringBuilder.AppendLine("      {");
            stringBuilder.AppendLine($"          {firstProperty.PropertyName.TransformTo(CaseType.SentenceCase)} = test{firstProperty.PropertyName.TransformTo(CaseType.SentenceCase)}");
            stringBuilder.AppendLine("      });");
            stringBuilder.AppendLine(parametersExist
                ? $"      var {tableName.ToLower()}s = _target.GetBy{firstProperty.PropertyName.TransformTo(CaseType.SentenceCase)}(test{firstProperty.PropertyName.TransformTo(CaseType.SentenceCase)});"
                : $"      var {tableName.ToLower()}s = _target.GetAll();");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("      //Assert");
            stringBuilder.AppendLine($"      Assert.IsNotNull({tableName.ToLower()}s);");
            stringBuilder.AppendLine($"      Assert.AreEqual({tableName.ToLower()}s.FirstOrDefault()?.{firstProperty.PropertyName.TransformTo(CaseType.SentenceCase)}, test{firstProperty.PropertyName.TransformTo(CaseType.SentenceCase)});");
            stringBuilder.AppendLine("  }");
            stringBuilder.AppendLine("}");

            return stringBuilder;
        }
    }
}