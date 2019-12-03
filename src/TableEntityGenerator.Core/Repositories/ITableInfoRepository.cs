using System.Collections.Generic;
using TableEntityGenerator.Core.Models;

namespace TableEntityGenerator.Core.Repositories
{
    public interface ITableInfoRepository
    {
        IDictionary<string, string> TypeMapping { get; }
        IAsyncEnumerable<TableInfo> ListAllAsync();
    }
}