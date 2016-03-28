using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
	                COLUMN_NAME As ColumnName,
	                TABLE_NAME As TableName,
	                TABLE_SCHEMA As TableSchema,
	                CASE WHEN IS_NULLABLE = 'NO' THEN 0 ELSE 1 END As IsNullable,
	                DATA_TYPE As DataType,
	                COLUMNPROPERTY(object_id(TABLE_SCHEMA +'.'+TABLE_NAME), COLUMN_NAME, 'IsIdentity') AS IsIdentity,
	                *
                from INFORMATION_SCHEMA.COLUMNS
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
