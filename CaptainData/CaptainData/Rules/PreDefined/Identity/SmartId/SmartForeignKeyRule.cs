using System;
using System.Linq;
using CaptainData.Schema;

namespace CaptainData.Rules.PreDefined.Identity
{
    public class SmartForeignKeyRule : SingleColumnRule
    {
        protected ISmartIdInsertForeignKeyResolver ForeignKeyResolver = new FKDefaults.Table_Id();

        public override void Apply(RowInstruction rowInstruction, ColumnSchema column)
        {
            if (!rowInstruction.IsDefinedFor(column.ColumnName))
            {
                var context = rowInstruction.CaptainContext.LastIds();
                if (ForeignKeyResolver.Is(column, context.Select(x => x.Key).ToArray()))
                {
                    var referencedTable = ForeignKeyResolver.Get(column);
                    if (context.ContainsKey(referencedTable))
                    {
                        rowInstruction.ColumnInstructions[column.ColumnName] = new ColumnInstruction(context[referencedTable]);
                    }
                }
            }
        }

        public override bool Match(RowInstruction rowInstruction, ColumnSchema column)
        {
            return (ForeignKeyResolver.Is(column, rowInstruction.CaptainContext.LastIds().Select(x => x.Key).ToArray()));
        }
    }
}