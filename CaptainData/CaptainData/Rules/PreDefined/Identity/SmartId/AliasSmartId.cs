using CaptainData.Schema;
using System;

namespace CaptainData.Rules.PreDefined.Identity.SmartId
{
    /// <summary>
    /// Alias when column names don't match convention.
    /// </summary>
    public class AliasSmartId : SingleColumnValueRule
    {
        private readonly Func<ColumnSchema, RowInstruction, bool> _predicate;
        private readonly string _useIdFromTable;
        
        /// <param name="forTableColumn">Table column in the format {table}.{column}</param>
        /// <param name="useIdFromTable">Table name to reference foreign key id from</param>
        public AliasSmartId(string forTableColumn, string useIdFromTable)
        {
            _useIdFromTable = useIdFromTable;
            if (!string.IsNullOrEmpty(forTableColumn) && forTableColumn.Split('.') is { Length: 2 } segments)
            {
                _predicate = (column, _) => column.ColumnName == segments[1] && column.TableName == segments[0];
            }
            else
            {
                throw new ArgumentException("Table and column needs to be specified like {table}.{column}", forTableColumn);
            }
        }

        public AliasSmartId(Func<ColumnSchema, bool> predicate, string useIdFromTable)
            : this((column, _) => predicate(column), useIdFromTable)
        {
        }

        public AliasSmartId(Func<ColumnSchema, RowInstruction, bool> predicate, string useIdFromTable)
        {
            _predicate = predicate;
            _useIdFromTable = useIdFromTable;
        }

        public override ColumnInstruction Value(ColumnSchema column, RowInstruction rowInstruction)
        {
            return new ColumnInstruction(rowInstruction.CaptainContext.LastId($"dbo.{_useIdFromTable}"));
        }

        public override bool Match(ColumnSchema column, RowInstruction rowInstruction) => _predicate(column, rowInstruction);
    }
}
