using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace CaptainData
{
    public class SqlExecutor : ISqlExecutor
    {
        public async Task<object> Execute(IDbConnection connection, string sql, DynamicParameters values, IDbTransaction transaction)
        {
            return await connection.ExecuteScalarAsync(sql, values, transaction);
        }
    }
}