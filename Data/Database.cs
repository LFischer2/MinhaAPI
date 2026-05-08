using Npgsql;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace MinhaAPI.Data
{
    public class Database
    {
        private readonly IConfiguration _config;

        public Database(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection ObterConexao()
        {
            return new NpgsqlConnection(_config.GetConnectionString("ConexaoPadrao"));
        }
    }
}
