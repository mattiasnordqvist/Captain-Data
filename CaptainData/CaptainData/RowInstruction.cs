using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

namespace CaptainData
{
    public class RowInstruction
    {
        private readonly Dictionary<string, ColumnInstruction> _columnInstructions = new Dictionary<string, ColumnInstruction>();

        private InstructionContext _instructionContext;

        internal readonly List<Action<IDbConnection, IDbTransaction>> After = new List<Action<IDbConnection, IDbTransaction>>();

        internal readonly List<Action<IDbConnection, IDbTransaction>> Before = new List<Action<IDbConnection, IDbTransaction>>();

        public void SetContext(InstructionContext instructionContext)
        {
            _instructionContext = instructionContext;
        }

        internal IDictionary<string, ColumnInstruction> NonEmptyColumnInstructions
        {
            get
            {
                return _columnInstructions.Where(x => x.Key != null).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public object this[string index]
        {
            set
            {
                _columnInstructions[index] = new ColumnInstruction { Value = value };
            }
        }

        internal Dictionary<string, ColumnInstruction> ColumnInstructions => _columnInstructions;

        internal InstructionContext InstructionContext => _instructionContext;

        public bool IsDefinedFor(string columnName) => _columnInstructions.ContainsKey(columnName);

        public void AddBefore(string before)
        {
            AddBefore((c, t) => c.Execute(before, transaction: t));
        }

        public void AddAfter(string after)
        {
            AddAfter((c, t) => c.Execute(after, transaction: t));
        }

        public void AddBefore(Action<IDbConnection, IDbTransaction> before)
        {
            Before.Add(before);
        }

        public void AddAfter(Action<IDbConnection, IDbTransaction> after)
        {
            After.Insert(0, after);
        }
    }
}