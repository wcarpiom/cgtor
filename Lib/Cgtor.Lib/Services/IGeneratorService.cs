using System.Collections.Generic;
using System.Text;
using Cgtor.Lib.Models;

namespace Cgtor.Lib.Services
{
    public interface IGeneratorService
    {
        StringBuilder GenerateCode(IEnumerable<ModelProperty> properties, string connectionString, string dbObjectName);
    }
}