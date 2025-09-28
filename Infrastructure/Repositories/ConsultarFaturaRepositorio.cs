using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.BaseDados;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class ConsultarFaturaRepositorio : IConsultarFaturaEntidade
    {
        private readonly IDBContext _dbContext;

        public ConsultarFaturaRepositorio(IDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<FaturaDomain> ObterTodasFaturas()
        {
            var sql = "SELECT descricao, data, valor, categoria FROM fatura";

            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var faturas = connection.Query<FaturaDomain>(sql).ToList();

            connection.Close();
            return faturas;
        }
    }
}
