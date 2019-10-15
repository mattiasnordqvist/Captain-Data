using CaptainData.Schema;

namespace CaptainData.Rules.PreDefined.Identity
{
    public interface IForeignKeyMatchingStrategy
    {
        bool IsForeignKey(ColumnSchema column, RowInstruction rowInstruction);
        string GetReferencedTable(ColumnSchema column, RowInstruction rowInstruction);
    }
}