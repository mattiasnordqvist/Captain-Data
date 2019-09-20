using System;
using System.Collections.Generic;
using System.Linq;

namespace CaptainData
{
    public class RowInstruction
    {
        public CaptainContext CaptainContext { get; set; }

        public string TableName { get; set; }

        public object Overrides { get; set; }

        internal IDictionary<string, ColumnInstruction> NonEmptyColumnInstructions
        {
            get
            {
                return ColumnInstructions.Where(x => x.Key != null).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public Dictionary<string, ColumnInstruction> ColumnInstructions { get; private set; } = new Dictionary<string, ColumnInstruction>();

        public ColumnInstruction this[string index]
        {
            set
            {
                ColumnInstructions[index] = value;
            }
        }
        public bool IsDefinedFor(string columnName) => ColumnInstructions.ContainsKey(columnName) && !ColumnInstructions[columnName].IgnoreColumn;

        public bool RequiresIdentityInsert => ColumnInstructions.Any(x => IsDefinedFor(x.Key) && CaptainContext.SchemaInformation[TableName][x.Key].IsIdentity);

        public Delegate Callback { get; internal set; }
    }
}