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

        public override void Apply(RowInstruction rowInstruction, ColumnSchema column, InstructionContext instructionContext)
        {
            var context = GetContext(instructionContext);
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

        private Dictionary<string, T> GetContext(InstructionContext instructionContext)
        {
            if (!instructionContext.CaptainContext.ContainsKey("SmartIdContext"))
            {
                instructionContext.CaptainContext["SmartIdContext"] = new Dictionary<string, T>();
            }
            return (Dictionary<string, T>)instructionContext.CaptainContext["SmartIdContext"];
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

        public override bool Match(RowInstruction rowInstruction, ColumnSchema column, InstructionContext instructionContext)
        {
            return column.IsIdentity || (foreignKeysEnabled && foreignKeyResolver.Is(column, GetContext(instructionContext).Select(x => x.Key).ToArray()));
        }

        public SmartIdInsertRule<T> EnableForeignKeys(Func<FKDefaults, ISmartIdInsertForeignKeyResolver> foreignKeyResolver)
        {
            this.foreignKeyResolver = foreignKeyResolver(new FKDefaults());
            foreignKeysEnabled = true;
            return this;
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