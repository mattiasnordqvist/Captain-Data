using CaptainData.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CaptainData.Rules.PreDefined.Identity.SmartId
{
    /// <summary>
    /// Alias when column names don't match convention.
    /// </summary>
    public class AliasMatchStrategy : IForeignKeyMatchingStrategy
    {
        //private readonly Func<ColumnSchema, RowInstruction, bool> _predicate;
        //private readonly string _useIdFromTable;
        private AliasDefinitions _definitions;

        /// <param name="forTableColumn">Table column in the format {table}.{column}</param>
        /// <param name="useIdFromTable">Table name to reference foreign key id from</param>
        public AliasMatchStrategy(string forTableColumn, string useIdFromTable)
            : this (options => options.AddAlias(forTableColumn, useIdFromTable)) { }

        public AliasMatchStrategy(Func<ColumnSchema, bool> predicate, string useIdFromTable)
            : this((column, _) => predicate(column), useIdFromTable) { }

        public AliasMatchStrategy(Func<ColumnSchema, RowInstruction, bool> predicate, string useIdFromTable)
            : this(options => options.AddAlias(predicate, useIdFromTable)) { }

        public AliasMatchStrategy(Action<IAliasMatchStrategyOptions> build)
        {
            _definitions = new AliasDefinitions();
            build(_definitions);
        }

        public virtual bool IsForeignKey(ColumnSchema c, RowInstruction rowInstruction) => _definitions.HasMatch(c, rowInstruction);

        public virtual string GetReferencedTable(ColumnSchema c, RowInstruction rowInstruction) => _definitions.GetMatch(c, rowInstruction);

        public interface IAliasMatchStrategyOptions
        {
            IAliasMatchStrategyOptions AddAlias(string forTableColumn, string useIdFromTable);
            IAliasMatchStrategyOptions AddAlias(Func<ColumnSchema, bool> predicate, string useIdFromTable);
            IAliasMatchStrategyOptions AddAlias(Func<ColumnSchema, RowInstruction, bool> predicate, string useIdFromTable);
        }

        internal class AliasDefinitions : List<AliasDefinition>, IAliasMatchStrategyOptions
        {
            public IAliasMatchStrategyOptions AddAlias(string forTableColumn, string useIdFromTable)
            {
                if (!string.IsNullOrEmpty(forTableColumn) && forTableColumn.Split('.') is { Length: 2 } segments)
                {
                    return AddAlias((column, _) => column.ColumnName == segments[1] && column.TableName == segments[0], useIdFromTable);
                }
                else
                {
                    throw new ArgumentException("Table and column alias needs to be specified like {table}.{column}", forTableColumn);
                }
            }

            public IAliasMatchStrategyOptions AddAlias(Func<ColumnSchema, bool> predicate, string useIdFromTable)
            {
                return AddAlias((column, _) => predicate(column), useIdFromTable);
            }

            public IAliasMatchStrategyOptions AddAlias(Func<ColumnSchema, RowInstruction, bool> predicate, string useIdFromTable)
            {
                Add(new AliasDefinition(predicate, useIdFromTable));
                return this;
            }

            internal string GetMatch(ColumnSchema column, RowInstruction rowInstruction)
            {
                return this.FirstOrDefault(x => x.Predicate(column, rowInstruction))?.UseIdFromTable;
            }

            internal bool HasMatch(ColumnSchema column, RowInstruction rowInstruction) => this.Any(x => x.Predicate(column, rowInstruction));
        }

        internal class AliasDefinition
        {
            public AliasDefinition(Func<ColumnSchema, RowInstruction, bool> predicate, string useIdFromTable)
            {
                Predicate = predicate;
                UseIdFromTable = useIdFromTable;
            }

            public Func<ColumnSchema, RowInstruction, bool> Predicate { get; }
            public string UseIdFromTable { get; }
        }
    }
}
