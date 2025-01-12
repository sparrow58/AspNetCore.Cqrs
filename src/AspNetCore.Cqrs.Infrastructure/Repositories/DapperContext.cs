using AspNetCore.Cqrs.Infrastructure.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace AspNetCore.Cqrs.Infrastructure.Repositories
{
    public sealed class DapperContext(IOptions<DatabaseSettings> options)
    {
        private readonly string? _connectionString = options.Value.SqlConnectionString;

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
