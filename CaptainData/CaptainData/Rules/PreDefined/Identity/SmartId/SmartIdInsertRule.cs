using System;
using System.Collections.Generic;
using System.Linq;
using CaptainData.Schema;

namespace CaptainData.Rules.PreDefined.Identity
{
    public abstract partial class SmartIdInsertRule<T> : SingleColumnRule
    {
        private ISmartIdInsertForeignKeyResolver foreignKeyResolver;
        private bool foreignKeysEnabled;

        public override void Apply(RowInstruction rowInstruction, ColumnSchema column)
        {
            var context = GetSmartIdContext(rowInstruction);
            if (column.IsIdentity)
            {
                rowInstruction.ColumnInstructions[column.ColumnName] = new ColumnInstruction(NextId(column.TableName, context));
            }
            else if (foreignKeysEnabled && foreignKeyResolver.Is(column, context.Select(x => x.Key).ToArray()))
            {
                var referencedTable = foreignKeyResolver.Get(column);
                if (context.ContainsKey(referencedTable))
                {
                    rowInstruction.ColumnInstructions[column.ColumnName] = new ColumnInstruction(context[referencedTable]);
                }
            }
        }

        private Dictionary<string, T> GetSmartIdContext(RowInstruction rowInstruction)
        {
            if (!rowInstruction.CaptainContext.ContainsKey("SmartIdContext"))
            {
                rowInstruction.CaptainContext["SmartIdContext"] = new Dictionary<string, T>();
            }
            return (Dictionary<string, T>)rowInstruction.CaptainContext["SmartIdContext"];
        }

        private T NextId(string tableName, Dictionary<string, T> context)
        {
            if (!context.ContainsKey(tableName))
            {
                context.Add(tableName, GetNextId(default(T)));
            }
            else
            {
                context[tableName] = GetNextId(context[tableName]);
            }
            return context[tableName];
        }

        protected abstract T GetNextId(T lastId);

        public override bool Match(RowInstruction rowInstruction, ColumnSchema column)
        {
            return column.IsIdentity || (foreignKeysEnabled && foreignKeyResolver.Is(column, GetSmartIdContext(rowInstruction).Select(x => x.Key).ToArray()));
        }

        public SmartIdInsertRule<T> EnableForeignKeys(Func<FKDefaults, ISmartIdInsertForeignKeyResolver> foreignKeyResolver)
        {
            return EnableForeignKeys(foreignKeyResolver(new FKDefaults()));
        }

        public SmartIdInsertRule<T> EnableForeignKeys(ISmartIdInsertForeignKeyResolver foreignKeyResolver)
        {
            this.foreignKeyResolver = foreignKeyResolver;
            foreignKeysEnabled = true;
            return this;
        }
    }

    public interface ISmartIdInsertForeignKeyResolver
    {
        Func<ColumnSchema, string[], bool> Is { get; }
        Func<ColumnSchema, string> Get { get; }
    }
}