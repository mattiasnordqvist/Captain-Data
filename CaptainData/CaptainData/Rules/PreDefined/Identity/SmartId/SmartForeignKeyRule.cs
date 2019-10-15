using CaptainData.Schema;

namespace CaptainData.Rules.PreDefined.Identity
{
    public class SmartForeignKeyRule : SingleColumnRule
    {
        private readonly IForeignKeyMatchingStrategy strategy;

        public SmartForeignKeyRule(IForeignKeyMatchingStrategy strategy)
        {
            this.strategy = strategy;
        }

        public override void Apply(RowInstruction rowInstruction, ColumnSchema column)
        {
            if (!rowInstruction.IsDefinedFor(column.ColumnName))
            {
                var context = rowInstruction.CaptainContext.LastIds();
                if (strategy.IsForeignKey(column, rowInstruction))
                {
                    var referencedTable = strategy.GetReferencedTable(column, rowInstruction);
                    if (context.ContainsKey(referencedTable))
                    {
                        rowInstruction.ColumnInstructions[column.ColumnName] = new ColumnInstruction(context[referencedTable]);
                    }
                }
            }
        }

        public override bool Match(RowInstruction rowInstruction, ColumnSchema column)
        {
            return strategy.IsForeignKey(column, rowInstruction);
        }


        
    }
}