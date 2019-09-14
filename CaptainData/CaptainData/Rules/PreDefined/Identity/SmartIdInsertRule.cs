using System;
using System.Collections.Generic;
using System.Linq;
using CaptainData.Schema;

namespace CaptainData.Rules.PreDefined.Identity
{
    public abstract class SmartIdInsertRule<T> : SingleColumnRule
    {
        private Dictionary<string, T> lastId = new Dictionary<string, T>();
        private Func<ColumnSchema, string> getReferencedTable;
        private Func<ColumnSchema, string[], bool> isForeignKey;
        private bool foreignKeysEnabled;

        public override void Apply(RowInstruction rowInstruction, ColumnSchema column, InstructionContext instructionContext)
        {
            if (column.IsIdentity)
            {
                rowInstruction.ColumnInstructions[column.ColumnName] = new ColumnInstruction(NextId(column.TableName));
            }
            else if (foreignKeysEnabled && isForeignKey(column, lastId.Select(x => x.Key).ToArray()))
            {
                var referencedTable = getReferencedTable(column);
                if (lastId.ContainsKey(referencedTable))
                {
                    rowInstruction.ColumnInstructions[column.ColumnName] = new ColumnInstruction(lastId[referencedTable]);
                }
            }
        }

        private T NextId(string tableName)
        {
            if (!lastId.ContainsKey(tableName))
            {
                lastId.Add(tableName, GetNextId(default(T)));
            }
            else
            {
                lastId[tableName] = GetNextId(lastId[tableName]);
            }
            return lastId[tableName];
        }

        protected abstract T GetNextId(T lastId);

        public override bool Match(RowInstruction rowInstruction, ColumnSchema column, InstructionContext instructionContext)
        {
            return column.IsIdentity || (foreignKeysEnabled && isForeignKey(column, lastId.Select(x => x.Key).ToArray()));
        }

        public SmartIdInsertRule<T> EnableForeignKeys(Func<ColumnSchema, string[], bool> isForeignKey, Func<ColumnSchema, string> getReferencedTable)
        {
            this.getReferencedTable = getReferencedTable;
            this.isForeignKey = isForeignKey;
            foreignKeysEnabled = true;
            return this;
        }
    }
}