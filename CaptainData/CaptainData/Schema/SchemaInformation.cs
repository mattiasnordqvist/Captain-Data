using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

namespace CaptainData.Schema
{
    public class SchemaInformation : List<ColumnSchema>
    {
        private SchemaInformation(List<ColumnSchema> columns)
        {
            AddRange(columns);
        }

        public static SchemaInformation Create(IDbConnection connection, IDbTransaction transaction)
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
                    IIF(c.default_object_id > 0, 1, 0) as HasDefault,
	                *
                from sys.tables t
                inner join sys.columns c on c.object_id = t.object_id
                inner join sys.types y ON c.user_type_id = y.user_type_id
                inner join sys.schemas s ON s.schema_id = t.schema_id
            ", transaction: transaction).ToList();
            return new SchemaInformation(columns);
        }

        public TableColumnList this[string tableName]
        {
            get
            {
                var schemaName = tableName.Contains(".") ? tableName.Split('.')[0].Trim('[', ']') : "dbo";
                tableName = (tableName.Contains(".") ? tableName.Split('.')[1] : tableName).Trim('[', ']');
                return new TableColumnList(this.Where(x => x.TableName == tableName && x.TableSchema == schemaName).ToList());

            }
        }
    }
}
