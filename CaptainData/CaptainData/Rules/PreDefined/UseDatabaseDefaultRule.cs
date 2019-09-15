using CaptainData.Schema;

namespace CaptainData.Rules.PreDefined
{
    /// <summary>
    /// Allows for columns with default specified to get it's default value set by the database
    /// </summary>
    public class UseDatabaseDefaultRule : SingleColumnRule
    {
        public bool ApplyOnNullableColumns { get; set; } = false;

        public override void Apply(RowInstruction rowInstruction, ColumnSchema column)
        {
            rowInstruction.ColumnInstructions[column.ColumnName] = ColumnInstruction.Ignore();            
        }

        public override bool Match(RowInstruction rowInstruction, ColumnSchema column)
        {
            if (rowInstruction.IsDefinedFor(column.ColumnName) || !column.HasDefault)
            {
                return false;
            }

            return ApplyOnNullableColumns || !column.IsNullable;
        }
    }
}