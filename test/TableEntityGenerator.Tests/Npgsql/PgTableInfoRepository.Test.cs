using System.Threading.Tasks;
using Xunit;
using TableEntityGenerator.Npgsql;
using Microsoft.Extensions.Configuration;

namespace TableEntityGenerator.Tests.Npgsql
{
    public class PgTableInfoRepositoryTest
    {
        private readonly string _connectionString;
        public PgTableInfoRepositoryTest()
        {
            var config = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .AddEnvironmentVariables();
            _connectionString = config.Build().GetConnectionString("postgres");
        }

        [SkippableFact]
        public async Task ListAllAsync_Returns_TableInfolist()
        {
            Skip.If(string.IsNullOrWhiteSpace(_connectionString), "ConnectionString[\"postgres\"] is null or empty.");
            var repo = new PgTableInfoRepository(_connectionString);
            var tables = await repo.ListAllAsync();
            Assert.NotNull(tables);
        }
    }
}
