using System.Collections.Generic;
using System.Linq;

namespace CaptainData
{
    public class RowInstruction
    {

        public void SetContext(InstructionContext instructionContext)
        {
            InstructionContext = instructionContext;

        }

        internal IDictionary<string, ColumnInstruction> NonEmptyColumnInstructions
        {
            get
            {
                return ColumnInstructions.Where(x => x.Key != null).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public InstructionContext InstructionContext { get; private set; }

        public Dictionary<string, ColumnInstruction> ColumnInstructions { get; private set; } = new Dictionary<string, ColumnInstruction>();

        public ColumnInstruction this[string index]
        {
            set
            {
                ColumnInstructions[index] = value;
            }
        }
        public string TableName => InstructionContext.TableName;
        public bool IsDefinedFor(string columnName) => ColumnInstructions.ContainsKey(columnName) && !ColumnInstructions[columnName].IgnoreColumn;

        public bool RequiresIdentityInsert => ColumnInstructions.Any(x => IsDefinedFor(x.Key) && InstructionContext.CaptainContext.SchemaInformation[InstructionContext.TableName][x.Key].IsIdentity);
    }
}