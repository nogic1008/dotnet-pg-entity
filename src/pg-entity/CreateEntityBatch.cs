using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MicroBatchFramework;
using Microsoft.Extensions.Logging;
using TableEntityGenerator.Core;

namespace TableEntityGenerator.Npgsql
{
    public class CreateEntityBatch : BatchBase
    {
        /// <summary>
        /// Generate Entity Class Files from Postgresql.
        /// </summary>
        /// <param name="connectionString">Connection String to Postgresql Database.</param>
        /// <param name="nameSpace">Namespace of generated Entity class.</param>
        /// <param name="outputFolder">Output folder (optional)</param>
        /// <param name="schema">Schema of Table to generate Entity class (optional)</param>
        public async Task GenerateFileAsync(
            [Option("c", "Connection String to Postgresql Database.")]string connectionString,
            [Option("n", "Namespace of generated Entity class.")]string nameSpace,
            [Option("o", "Output folder (optional)")]string? outputFolder = null,
            [Option("s", "Schema of Table to generate Entity class (optional)")]string? schema = null)
        {
            if (nameSpace is null)
                throw new ArgumentNullException(nameof(nameSpace));
            if (string.IsNullOrWhiteSpace(nameSpace))
                throw new ArgumentException(nameof(nameSpace) + " is required.", nameof(nameSpace));

            var repository = new PgTableInfoRepository(connectionString, schema);
            var generator = new GeneratorCore(repository);
            foreach (var (fileName, text) in await generator.GenerateSourceFileAsync(nameSpace))
            {
                Context.Logger.LogInformation($"Write {fileName}");
                string filePath = string.IsNullOrEmpty(outputFolder)
                ? Path.Combine(Environment.CurrentDirectory, fileName)
                : Path.Combine(outputFolder, fileName);
                await File.AppendAllTextAsync(filePath, text, UTF8Encoding.Default, Context.CancellationToken);
            }
        }

        [Command("version", "show version.")]
        public void ShowVersion()
        {
            string version = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyFileVersionAttribute>()!
                .Version;
            Console.WriteLine(version);
        }
    }
}