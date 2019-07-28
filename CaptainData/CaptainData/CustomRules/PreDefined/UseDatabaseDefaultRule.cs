using CaptainData.Schema;

namespace CaptainData.CustomRules.PreDefined
{
    /// <summary>
    /// Allows for columns with default specified to get it's default value set by the database
    /// </summary>
    public class UseDatabaseDefaultRule : SingleColumnRule
    {
        public bool ApplyOnNullableColumns { get; set; } = false;

        public override void Apply(RowInstruction rowInstruction, ColumnSchema column, InstructionContext instructionContext)
        {
            rowInstruction.ColumnInstructions[column.ColumnName] = ColumnInstruction.Ignore();            
        }

        public override bool Match(RowInstruction rowInstruction, ColumnSchema column, InstructionContext instructionContext)
        {
            if (rowInstruction.IsDefinedFor(column.ColumnName) || !column.HasDefault)
            {
                return false;
            }

            return ApplyOnNullableColumns || !column.IsNullable;
        }
    }
}