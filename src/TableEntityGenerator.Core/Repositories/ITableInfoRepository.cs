using System.Collections.Generic;
using System.Threading.Tasks;
using TableEntityGenerator.Core.Models;

namespace TableEntityGenerator.Core.Repositories
{
    public interface ITableInfoRepository
    {
        IDictionary<string, string> TypeMapping { get; }

        Task<IEnumerable<TableInfo>> ListAllAsync();
    }
}