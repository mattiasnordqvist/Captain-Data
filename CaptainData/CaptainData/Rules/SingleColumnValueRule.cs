using CaptainData.Schema;

namespace CaptainData.Rules
{
    public abstract class SingleColumnValueRule : SingleColumnRule
    {
        public bool OverwriteExistingInstruction { get; set; } = false;

        public override void Apply(RowInstruction rowInstruction, ColumnSchema column)
        {
            rowInstruction[column.ColumnName] = Value(column, rowInstruction);
        }

        public override bool Match(RowInstruction rowInstruction, ColumnSchema column)
        {
            return (!rowInstruction.IsDefinedFor(column.ColumnName) || OverwriteExistingInstruction)
                   && Match(column, rowInstruction);
        }

        public abstract bool Match(ColumnSchema column, RowInstruction rowInstruction);

        public abstract ColumnInstruction Value(ColumnSchema column, RowInstruction rowInstruction);
    }
}