using CaptainData.Schema;

namespace CaptainData.CustomRules
{
    public abstract class SingleColumnValueRule : SingleColumnRule
    {
        public bool OverwriteExistingInstruction { get; set; } = false;

        public override void Apply(RowInstruction rowInstruction, ColumnSchema column, InstructionContext instructionContext)
        {
            rowInstruction[column.ColumnName] = Value(column, instructionContext);
        }

        public override bool Match(RowInstruction rowInstruction, ColumnSchema column, InstructionContext instructionContext)
        {
            return (!rowInstruction.IsDefinedFor(column.ColumnName) || OverwriteExistingInstruction)
                   && Match(column, instructionContext);
        }

        public abstract bool Match(ColumnSchema column, InstructionContext instructionContext);

        public abstract ColumnInstruction Value(ColumnSchema column, InstructionContext instructionContext);
    }
}