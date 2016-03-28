using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Dapper;

namespace CaptainData
{
    public class RowInstruction
    {
        private readonly Dictionary<string, ColumnInstruction> _columnInstructions = new Dictionary<string, ColumnInstruction>();

        private InstructionContext _instructionContext;

        private readonly List<Action<IDbConnection, IDbTransaction>> _after = new List<Action<IDbConnection, IDbTransaction>>();
        private readonly List<Action<IDbConnection, IDbTransaction>> _before = new List<Action<IDbConnection, IDbTransaction>>();

        public void SetContext(InstructionContext instructionContext)
        {
            _instructionContext = instructionContext;
        }

        public object this[string index]
        {
            set
            {
                _columnInstructions[index] = new ColumnInstruction { Value = value };
            }
        }

        public void Apply(IDbConnection connection, IDbTransaction transaction)
        {
            foreach (var action in _before)
            {
                action(connection, transaction);
            }
            var sql = new StringBuilder();
            var nonEmptyColumnInstructions = _columnInstructions.Where(x => x.Key != null).ToDictionary(x => x.Key, x => x.Value);
            var v = nonEmptyColumnInstructions.ToDictionary(x => x.Key, x => x.Value.Value).AsEnumerable();
            var values = new DynamicParameters(v);
            sql.AppendLine($"INSERT INTO {_instructionContext.TableName} ({string.Join(", ", nonEmptyColumnInstructions.Keys)}) VALUES ({string.Join(", ", nonEmptyColumnInstructions.Keys.Select(x => $"@{x}"))});");
            sql.AppendLine("SELECT SCOPE_IDENTITY()");
            _instructionContext.CaptainContext.ScopeIdentity = connection.ExecuteScalar(sql.ToString(), values, transaction);
            foreach (var action in _after)
            {
                action(connection, transaction);
            }
        }

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
            _before.Add(before);
        }

        public void AddAfter(Action<IDbConnection, IDbTransaction> after)
        {
            _after.Insert(0, after);
        }
    }

    public class ColumnInstruction
    {
        public object Value { get; set; }
    }
}