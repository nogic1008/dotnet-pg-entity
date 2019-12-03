using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
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

        public async IAsyncEnumerable<(string fileName, string text)> GenerateSourceFileAsync(string nameSpace)
        {
            await foreach (var table in _repository.ListAllAsync())
            {
                yield return
                (
                    $"{table.Name}.cs",
                    ((ITextTemplate)new TableEntity(_repository.TypeMapping, nameSpace, table)).TransformText()
                );
            }
        }

        public async ValueTask WriteFilesAsync(string nameSpace, string? outputFolder = null, Encoding? encoding = null, CancellationToken cancellationToken = default)
        {
            if (nameSpace is null)
                throw new ArgumentNullException(nameof(nameSpace));
            if (string.IsNullOrWhiteSpace(nameSpace))
                throw new ArgumentException(nameof(nameSpace) + " is required.", nameof(nameSpace));

            await foreach (var (fileName, text) in GenerateSourceFileAsync(nameSpace))
            {
                string filePath = string.IsNullOrEmpty(outputFolder)
                ? Path.Combine(Environment.CurrentDirectory, fileName)
                : Path.Combine(outputFolder, fileName);
                await File.AppendAllTextAsync(filePath, text, encoding ?? UTF8Encoding.Default, cancellationToken);
            }
        }
    }
}