using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.BaseDados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AdicionarFaturaRepositorio : IAdicionarFaturaEntidade
    {
        private readonly IDBContext _dbContext;
        public AdicionarFaturaRepositorio(IDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void AdicionarFaturaDomain(FaturaDomain faturaDomain)
        {
            var sql = "INSERT INTO fatura(descricao, data, categoria, valor) VALUES(@descricao, @data, @categoria, @valor)";

            var parameters = new DynamicParameters();
            parameters.Add("descricao", faturaDomain.descricao, System.Data.DbType.String);
            parameters.Add("data", faturaDomain.data, System.Data.DbType.Date);
            parameters.Add("categoria", faturaDomain.categoria, System.Data.DbType.String);
            parameters.Add("valor", faturaDomain.valor, System.Data.DbType.Double);

            using var connection = _dbContext.CreateConnection();
            connection.Open();

            connection.Execute(sql, parameters);
            connection.Close();
        }
    }
}
