using System;
using System.Threading.Tasks;
using MicroBatchFramework;

namespace TableEntityGenerator.Npgsql
{
    internal class Program
    {
        private static async Task Main(string[] args)
            => await BatchHost.CreateDefaultBuilder().RunBatchEngineAsync<CreateEntityBatch>(args);
    }
}