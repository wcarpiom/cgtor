using Cgtor.Lib.Models;
using Gtor.Utils.DbUtilities;
using Gtor.Utils.Models;
using Gtor.Utils.StringUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace Cgtor.Lib.Services.CodeServices
{
    public class RepoGenerator : ICodeGeneratorService
    {
        private readonly IDbSysUtils _dbSysUtils;
        private string _connectionString;

        public RepoGenerator() : this(null, null) { }

        public RepoGenerator(string connectionString, IDbSysUtils dbSysUtils)
        {
            _dbSysUtils = dbSysUtils ?? new DbSysUtils();
            _connectionString = connectionString ?? WebConfigurationManager.AppSettings["connectionString"];
        }

        public string DbDataContext { get; set; }

        public StringBuilder GenerateCode(IEnumerable<ModelProperty> properties, string connectionString, string dbObjectName)
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                _connectionString = connectionString;
            }
            var modelProperties = properties.ToList();
            var tableName = _dbSysUtils.GetTablesFromSP(_connectionString, dbObjectName).ToList().FirstOrDefault();
            var firstProperty = modelProperties.FirstOrDefault();
            var lastProperty = modelProperties.Last();

            var parametersExist = _dbSysUtils.GetParametersFromSP(_connectionString, dbObjectName).Any();

            if (tableName == null || firstProperty == null)
            {
                throw new ArgumentException($"It's not possible to retrieve the first table name from this SP: {dbObjectName}");
            }
            var spName = tableName;
            tableName = tableName.TransformTo(CaseType.SentenceCase).Replace("_", "");

            var stringBuilder = new StringBuilder();
            if (!parametersExist)
            {
                stringBuilder.AppendLine($"public interface I{tableName}Repo");
                stringBuilder.AppendLine("{");
            }
            stringBuilder.AppendLine(parametersExist
                    ? $"  public Table{tableName} GetBy{firstProperty.PropertyName.TransformTo(CaseType.SentenceCase)}({firstProperty.PropertyDataType.ToLower()} {firstProperty.PropertyName.ToLower()});"
                    : $"  IEnumerable<Table{tableName}> GetAll();");
            stringBuilder.AppendLine($"  void Save(Table{tableName} table{tableName});");
            if (!parametersExist)
                stringBuilder.AppendLine("}");
            stringBuilder.AppendLine();
            if (!parametersExist)
            {
                stringBuilder.AppendLine($"public class {tableName}Repo : DataAccessBaseRepo, I{tableName}Repo");
                stringBuilder.AppendLine("{");
            }
            stringBuilder.AppendLine(parametersExist
                ? $"  public Table{tableName} GetBy{firstProperty.PropertyName.TransformTo(CaseType.SentenceCase)}({firstProperty.PropertyDataType.ToLower()} {firstProperty.PropertyName.ToLower()})"
                : $"  public IEnumerable<Table{tableName}> GetAll()");
            stringBuilder.AppendLine("  {");
            stringBuilder.AppendLine(parametersExist
                ? $"      Table{tableName} table{tableName};"
                : $"      var table{tableName} = new List<Table{tableName}>();");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"      using (var db = new {DbDataContext}(ConnectionString))");
            stringBuilder.AppendLine("      {");
            stringBuilder.AppendLine(parametersExist
                ? $"          var dataList = db.{dbObjectName.ToUpper()}({firstProperty.PropertyName.ToLower()}).FirstOrDefault();"
                : $"          var dataList = db.{dbObjectName.ToUpper()}().ToList();");
            stringBuilder.AppendLine(parametersExist
                ? $"          table{tableName} = new Table{tableName}"
                : $"          table{tableName}.AddRange(dataList.Select(data{tableName} => new Table{tableName}");
            stringBuilder.AppendLine("          {");

            foreach (var property in modelProperties)
            {
                stringBuilder.Append("              " + property.PropertyName.TransformTo(CaseType.SentenceCase) + $" = data{tableName}." +
                                         property.PropertyName.TransformTo(CaseType.SentenceCase));

                if (property.PropertyName.Contains("TIMESTAMP"))
                {
                    stringBuilder.Append(".UtcDateTime");
                }

                if (!property.Equals(lastProperty))
                {
                    stringBuilder.AppendLine(",");
                }
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine(parametersExist ? "          };" : "          }));");
            stringBuilder.AppendLine("      }");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"      return table{tableName};");
            stringBuilder.AppendLine("  }");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"  public void Save(Table{tableName} table{tableName})");
            stringBuilder.AppendLine("  {");
            stringBuilder.AppendLine($"      using (var db = new {DbDataContext}(ConnectionString))");
            stringBuilder.AppendLine("      {");
            stringBuilder.Append($"          db.SAVE_{spName}(");

            foreach (var property in modelProperties)
            {
                stringBuilder.Append($"table{tableName}.{property.PropertyName.TransformTo(CaseType.SentenceCase)}");
                if (!property.Equals(lastProperty))
                {
                    stringBuilder.Append(", ");
                }
            }
            stringBuilder.AppendLine(");");
            stringBuilder.AppendLine("      }");
            stringBuilder.AppendLine("  }");

            if (!parametersExist)
                stringBuilder.AppendLine("}");

            return stringBuilder;
        }
    }
}