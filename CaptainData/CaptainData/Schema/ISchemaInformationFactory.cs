using System.Data;
using CaptainData.Schema;

namespace CaptainData.Schema
{
    public interface ISchemaInformationFactory
    {
        SchemaInformation Create(IDbConnection connection, IDbTransaction transaction);
    }
}