using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableEntityGenerator.Core.Repositories;
using TableEntityGenerator.Core.Templates;

namespace TableEntityGenerator.Core
{
    public class GeneratorCore
    {
        private readonly ITableInfoRepository _repository;

        public GeneratorCore(ITableInfoRepository repository)
            => _repository = repository;

        public async ValueTask<IEnumerable<(string fileName, string text)>> GenerateSourceFileAsync(string nameSpace)
            => (await _repository.ListAllAsync())
                .Select(t => ($"{t.Name}.cs", ((ITextTemplate)new TableEntity(_repository.TypeMapping, nameSpace, t)).TransformText()));
    }
}