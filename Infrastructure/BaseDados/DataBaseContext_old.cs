using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BaseDados
{
    public class DataBaseContext : IDBContext
    {
        private readonly string stringConnection;

        public DataBaseContext(IConfiguration config)
        {
            this.stringConnection = config.GetConnectionString("cartaofatura");
        }
        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(stringConnection);
        }
    }
}
