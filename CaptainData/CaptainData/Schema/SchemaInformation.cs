using System.Collections.Generic;
using System.Linq;

namespace CaptainData.Schema
{
    public class SchemaInformation : List<ColumnSchema>
    {
        public SchemaInformation(List<ColumnSchema> columns)
        {
            AddRange(columns);
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

        public static string FTN(string tableName)
        {
            var schemaName = tableName.Contains(".") ? tableName.Split('.')[0].Trim('[', ']') : "dbo";
            tableName = (tableName.Contains(".") ? tableName.Split('.')[1] : tableName).Trim('[', ']');
            return $"[{schemaName}].[{tableName}]";
        }
    }
}
