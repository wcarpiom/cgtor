using Cgtor.Lib.Models;
using System.Collections.Generic;

namespace Cgtor.Lib.DbServices
{
    public interface IDbReadingService
    {
        string ConnectionString { get; set; }
        string DbObjectName { get; set; }

        IEnumerable<ModelProperty> ReadFromDb();
    }
}