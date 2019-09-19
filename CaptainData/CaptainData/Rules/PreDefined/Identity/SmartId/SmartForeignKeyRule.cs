using System;
using System.Linq;
using CaptainData.Schema;

namespace CaptainData.Rules.PreDefined.Identity
{
    public class SmartForeignKeyRule : SingleColumnRule
    {

        public override void Apply(RowInstruction rowInstruction, ColumnSchema column)
        {
            if (!rowInstruction.IsDefinedFor(column.ColumnName))
            {
                var context = rowInstruction.CaptainContext.LastIds();
                if (IsForeignKey(column, rowInstruction))
                {
                    var referencedTable = GetReferencedTable(column, rowInstruction);
                    if (context.ContainsKey(referencedTable))
                    {
                        rowInstruction.ColumnInstructions[column.ColumnName] = new ColumnInstruction(context[referencedTable]);
                    }
                }
            }
        }

        public override bool Match(RowInstruction rowInstruction, ColumnSchema column)
        {
            return IsForeignKey(column, rowInstruction);
        }

        public virtual bool IsForeignKey(ColumnSchema c, RowInstruction rowInstruction) => c.ColumnName.EndsWith("_Id");

        public virtual string GetReferencedTable(ColumnSchema c, RowInstruction rowInstruction)
        {
            if (rowInstruction.CaptainContext.SchemaInformation.Count(x => x.TableName == c.ColumnName.Substring(0, c.ColumnName.Length - 3)) == 1)
            {
                var t = rowInstruction.CaptainContext.SchemaInformation.Single(x => x.TableName == c.ColumnName.Substring(0, c.ColumnName.Length - 3));
                return SchemaInformation.FTN(t.TableSchema + "." + t.TableName);
            }
            else
            {
                throw new Exception("Can't find table reference for foreign key " + c.TableSchema + "." + c.TableName + "." + c.ColumnName);
            }
        }
    }
}