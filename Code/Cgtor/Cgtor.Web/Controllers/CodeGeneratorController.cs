using Cgtor.Lib.DbServices;
using Cgtor.Lib.Models;
using Cgtor.Lib.Services;
using Cgtor.Lib.Services.CodeServices;
using Cgtor.Lib.Services.TestServices;
using Cgtor.Web.ViewModels;
using Gtor.Utils.DbUtilities;
using System;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Cgtor.Web.Controllers
{
    public class CodeGeneratorController : Controller
    {
        private IGeneratorService _generatorService;
        private readonly IDbReadingService _spReadingService;
        private readonly IDbSysUtils _dbSysUtils;

        public CodeGeneratorController() : this(null, null) { }

        public CodeGeneratorController(IDbReadingService spReadingService, IDbSysUtils dbSysUtils)
        {
            _spReadingService = spReadingService ?? new SpReadingService();
            _dbSysUtils = dbSysUtils ?? new DbSysUtils();
        }
        public ActionResult Index()
        {
            var resultViewModel = new GeneratedModelCodeViewModel
            {
                DefaultConnectionString = WebConfigurationManager.AppSettings["connectionString"],
                DefaultStoredProcedure = WebConfigurationManager.AppSettings["dbObjectName"],
                CodeServiceType = CodeServiceType.TableModel
            };

            return View("Index", resultViewModel);
        }

        [HttpPost]
        public ActionResult GenerateModel(string connectionString, string dbObjectName, CodeServiceType codeServiceType)
        {
            if (string.IsNullOrEmpty(connectionString) ||
                string.IsNullOrEmpty(dbObjectName) ||
                codeServiceType == 0)
            {
                return View("Index", new GeneratedModelCodeViewModel());
            }

            _spReadingService.ConnectionString = connectionString;
            _spReadingService.DbObjectName = dbObjectName;
            var modelProperties = _spReadingService.ReadColumnsFromDbObject().ToList();

            switch (codeServiceType)
            {
                case CodeServiceType.TableModel:
                    _generatorService = new TableModelGenerator();
                    break;
                case CodeServiceType.Repo:
                    _generatorService = new RepoGenerator();
                    break;
                case CodeServiceType.RepoTest:
                    _generatorService = new RepoTestGenerator();
                    break;
                default:
                    throw new ArgumentException("That generator does not exists");
            }

            var generatedModelCodeViewModel = new GeneratedModelCodeViewModel
            {
                ModelName = _dbSysUtils.GetTablesFromSP(connectionString, dbObjectName).ToList().FirstOrDefault(),
                ModelCode = _generatorService.GenerateCode(modelProperties, connectionString, dbObjectName),
                CodeServiceType = codeServiceType
            };

            return View("Index", generatedModelCodeViewModel);
        }
    }
}