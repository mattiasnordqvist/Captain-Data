using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace CaptainData
{
    public interface ISqlExecutor
    {
        Task<object> Execute(IDbConnection connection, string sql, DynamicParameters values, IDbTransaction transaction);
    }
}