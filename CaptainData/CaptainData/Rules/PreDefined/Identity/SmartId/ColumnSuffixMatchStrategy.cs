using System;
using System.Linq;
using CaptainData.Schema;

namespace CaptainData.Rules.PreDefined.Identity
{
    public class ColumnSuffixMatchStrategy : IForeignKeyMatchingStrategy
    {
        public virtual bool IsForeignKey(ColumnSchema c, RowInstruction rowInstruction)
        {
            return c.ColumnName.EndsWith(EndsWith) && rowInstruction.CaptainContext.SchemaInformation.Any(x => x.TableName == ColumnToTableName(c.ColumnName));
        }

        protected virtual string EndsWith { private get; set; } = "_Id";

        public virtual string GetReferencedTable(ColumnSchema c, RowInstruction rowInstruction)
        {
            var tables = rowInstruction.CaptainContext.SchemaInformation.GroupBy(x => (x.TableName, x.TableSchema)).ToList();
            var count = tables.Count(x => x.Key.TableName == ColumnToTableName(c.ColumnName));
            if (count == 1)
            {
                var t = tables.Single(x => x.Key.TableName == ColumnToTableName(c.ColumnName)).Key;
                return SchemaInformation.FTN(t.TableSchema + "." + t.TableName);
            }
            else
            {
                throw new Exception("Can't find table reference for foreign key " + c.TableSchema + "." + c.TableName + "." + c.ColumnName);
            }
        }

        protected virtual string ColumnToTableName(string columnName)
        {
            return columnName.Substring(0, columnName.Length - EndsWith.Length);
        }

    }
}