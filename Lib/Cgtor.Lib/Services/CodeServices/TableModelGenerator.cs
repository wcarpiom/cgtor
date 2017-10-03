using Cgtor.Lib.Controllers;
using Cgtor.Lib.Models;
using Gtor.Utils.DbUtilities;
using Gtor.Utils.Models;
using Gtor.Utils.StringUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace Cgtor.Lib.Services.CodeServices
{
    public class TableModelGenerator : ICodeGeneratorService
    {
        private readonly IDbSysUtils _dbSysUtils;
        private readonly string _connectionString;
        private readonly IModelPropertyController _modelPropertyController;

        public TableModelGenerator() : this(null, null, null) { }

        public TableModelGenerator(string connectionString, IModelPropertyController modelPropertyController, IDbSysUtils dbSysUtils)
        {
            _dbSysUtils = dbSysUtils ?? new DbSysUtils();
            _modelPropertyController = modelPropertyController ?? new ModelPropertyController();
            _connectionString = connectionString ?? WebConfigurationManager.AppSettings["connectionString"];
        }

        public string DbDataContext { get; set; }

        public StringBuilder GenerateCode(IEnumerable<ModelProperty> properties, string connectionString, string dbObjectName)
        {
            var modelProperties = properties.ToList();

            var tableName = _dbSysUtils.GetTablesFromSP(connectionString, dbObjectName)
                .ToList()
                .FirstOrDefault()
                .TransformTo(CaseType.SentenceCase);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"public class Table{tableName.TransformTo(CaseType.SentenceCase)}");
            stringBuilder.AppendLine("{");

            foreach (var modelProperty in modelProperties)
            {
                stringBuilder.AppendLine("  public "
                                         + modelProperty.PropertyDataType
                                         + _modelPropertyController.GetNullableMarkIfApplicable(modelProperty) + " "
                                         + modelProperty.PropertyName.TransformTo(CaseType.SentenceCase)
                                         + " { get; set; }"
                );
            }

            stringBuilder.AppendLine("}");

            return stringBuilder;
        }
    }
}