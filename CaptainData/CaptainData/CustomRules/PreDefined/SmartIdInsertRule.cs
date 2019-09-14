using System.Collections.Generic;
using CaptainData.Schema;

namespace CaptainData.CustomRules.PreDefined
{
    public abstract class SmartIdInsertRule<T> : SingleColumnRule
    {
        private Dictionary<string, T> lastId = new Dictionary<string, T>();
        public override void Apply(RowInstruction rowInstruction, ColumnSchema column, InstructionContext instructionContext)
        {
            rowInstruction.ColumnInstructions[column.ColumnName].Value = NextId(column.TableName);
        }

        private T NextId(string tableName)
        {
            if (!lastId.ContainsKey(tableName))
            {
                lastId.Add(tableName, GetNextId(default(T)));
            }
            lastId[tableName] = GetNextId(lastId[tableName]);
            return lastId[tableName];
        }

        protected abstract T GetNextId(T lastId);

        public override bool Match(RowInstruction rowInstruction, ColumnSchema column, InstructionContext instructionContext)
        {
            return column.IsIdentity;
        }
    }
}