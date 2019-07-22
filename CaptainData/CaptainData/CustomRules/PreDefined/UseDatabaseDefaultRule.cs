using CaptainData.Schema;

namespace CaptainData.CustomRules.PreDefined
{
    /// <summary>
    /// Allows for columns with default specified to get it's default value set by the database
    /// </summary>
    public class UseDatabaseDefaultRule : SingleColumnRule
    {
        public enum Mode
        {
            ApplyOnlyOnNotNullableDefaultColumns = 0,
            ApplyOnAllDefaultColumns = 1
        }

        public UseDatabaseDefaultRule(Mode mode = Mode.ApplyOnlyOnNotNullableDefaultColumns)
        {
            ApplicationMode = mode;
        }

        protected Mode ApplicationMode { get; set; }

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

            return ApplicationMode == Mode.ApplyOnAllDefaultColumns || !column.IsNullable;
        }
    }
}