using System.Data;
using System.Linq;
using CaptainData.Schema;
using Dapper;

namespace CaptainData.Schema
{
    public class SchemaInformationFactory : ISchemaInformationFactory
    {
        public SchemaInformation Create(IDbConnection connection, IDbTransaction transaction)
        {
                var columns = connection.Query<ColumnSchema>(@"
                select 
	                c.name as ColumnName, 
	                t.name as TableName, 
	                s.name as TableSchema, 
	                c.is_nullable As IsNullable,
	                y.name as DataType,
	                c.is_identity as IsIdentity,
	                c.is_computed as IsComputed,
	                CASE WHEN c.default_object_id > 0 THEN 1 ELSE 0 END as HasDefault,
	                *
                from sys.tables t
                inner join sys.columns c on c.object_id = t.object_id
                inner join sys.types y ON c.user_type_id = y.user_type_id
                inner join sys.schemas s ON s.schema_id = t.schema_id
            ", transaction: transaction).ToList();
                return new SchemaInformation(columns);
            }
    }
}