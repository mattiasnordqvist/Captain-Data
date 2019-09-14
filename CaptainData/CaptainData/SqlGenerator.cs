using System.Linq;

namespace CaptainData
{
    public class SqlGenerator : ISqlGenerator
    {
        public virtual string CreateInsertStatement(RowInstruction rowInstruction)
        {
            return $"INSERT INTO {rowInstruction.InstructionContext.TableName} ({string.Join(", ", rowInstruction.ColumnInstructions.Keys.Select(x => $"[{x}]"))}) VALUES ({string.Join(", ", rowInstruction.ColumnInstructions.Keys.Select(x => $"@{x}"))});";
        }

        public virtual string CreateGetScopeIdentityQuery(RowInstruction rowInstruction)
        {
            return "SELECT SCOPE_IDENTITY();";
        }

    }
}