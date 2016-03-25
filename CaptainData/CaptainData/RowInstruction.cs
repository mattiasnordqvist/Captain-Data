using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Dapper;

namespace CaptainData
{
    public class RowInstruction
    {
        private readonly Dictionary<string, ColumnInstruction> _columnInstructions = new Dictionary<string, ColumnInstruction>();

        private InstructionContext _instructionContext;

        private readonly List<Action<SqlConnection>> _after = new List<Action<SqlConnection>>();
        private readonly List<Action<SqlConnection>> _before = new List<Action<SqlConnection>>();

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

        public void Apply(SqlConnection connection)
        {
            foreach (var action in _before)
            {
                action(connection);
            }
            var sql = new StringBuilder();
            var columnValues = _columnInstructions.Where(x => x.Key != null).ToDictionary(x => x.Key, x => x.Value);
            sql.AppendLine($"INSERT INTO {_instructionContext.TableName} ({string.Join(", ", columnValues.Keys)}) VALUES ({string.Join(", ", columnValues.Values.Select(x => ToSqlValue(x.Value)))});");
            connection.Execute(sql.ToString());
            _instructionContext.CaptainContext.ScopeIdentity = connection.ExecuteScalar("SELECT SCOPE_IDENTITY()");
            foreach (var action in _after)
            {
                action(connection);
            }
        }

        private object ToSqlValue(object o)
        {
            if (o.GetType() == typeof(string))
            {
                return "'" + o + "'";
            }

            return o.ToString();
        }

        public bool IsDefinedFor(string columnName) => _columnInstructions.ContainsKey(columnName);

        public void AddBefore(string before)
        {
            AddBefore(c => c.Execute(before));
        }

        public void AddAfter(string after)
        {
            AddAfter(x => x.Execute(after));
        }

        public void AddBefore(Action<SqlConnection> before)
        {
            _before.Add(before);
        }

        public void AddAfter(Action<SqlConnection> after)
        {
            _after.Insert(0, after);
        }
    }

    public class ColumnInstruction
    {
        public object Value { get; set; }
    }
}