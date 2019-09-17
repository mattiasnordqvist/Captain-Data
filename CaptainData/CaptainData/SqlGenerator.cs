using System.Linq;

namespace CaptainData
{
    public class SqlGenerator : ISqlGenerator
    {
        public virtual string CreateInsertStatement(RowInstruction rowInstruction)
        {
            return $"INSERT INTO {rowInstruction.TableName} ({string.Join(", ", rowInstruction.InsertableColumns.Keys.Select(x => $"[{x}]"))}) VALUES ({string.Join(", ", rowInstruction.InsertableColumns.Keys.Select(x => $"@{x}"))});";
        }

        public virtual string CreateGetScopeIdentityQuery(RowInstruction rowInstruction)
        {
            return "SELECT SCOPE_IDENTITY();";
        }

    }
}