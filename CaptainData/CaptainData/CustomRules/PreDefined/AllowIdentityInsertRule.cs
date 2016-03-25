using CaptainData.Schema;

namespace CaptainData.CustomRules.PreDefined
{
    /// <summary>
    /// This rule will turn on identity insert on identity columns for which there has been a value provided, typically through an override.
    /// </summary>
    public class AllowIdentityInsertRule : SingleColumnRule
    {
        public override void Apply(RowInstruction rowInstruction, ColumnSchema column, InstructionContext instructionContext)
        {
            rowInstruction.AddBefore($"SET IDENTITY_INSERT {column.TableName} ON");
            rowInstruction.AddAfter($"SET IDENTITY_INSERT {column.TableName} OFF");
        }

        public override bool Match(RowInstruction rowInstruction, ColumnSchema column, InstructionContext instructionContext)
        {
            return column.IsIdentity && rowInstruction.IsDefinedFor(column.ColumnName);
        }
        
    }
}