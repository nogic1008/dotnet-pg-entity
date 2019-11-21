using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MicroBatchFramework;
using TableEntityGenerator.Core;

namespace TableEntityGenerator.Npgsql
{
    public class CreateEntityBatch : BatchBase
    {
        public async ValueTask GenerateFiles(string connectionString, string nameSpace, string? outputFolder = null, string? schema = null)
        {
            var repository = new PgTableInfoRepository(connectionString, schema);
            var generator = new GeneratorCore(repository);
            var sources = await generator.GenerateSourceFileAsync(nameSpace);
            foreach (var (fileName, text) in sources)
            {
                string filePath = string.IsNullOrEmpty(outputFolder)
                ? Path.Combine(Environment.CurrentDirectory, fileName)
                : Path.Combine(outputFolder, fileName);
                await File.AppendAllTextAsync(filePath, text, UTF8Encoding.Default, Context.CancellationToken);
            }

        }
    }
}