using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cgtor.Lib.Models;

namespace Cgtor.Web.ViewModels
{
    public class GeneratedModelCodeViewModel
    {
        public string ModelName { get; set; }
        public StringBuilder ModelCode { get; set; }
        public string DefaultConnectionString { get; set; }
        public string DefaultStoredProcedure { get; set; }
        public CodeServiceType CodeServiceType { get; set; }
    }
}
