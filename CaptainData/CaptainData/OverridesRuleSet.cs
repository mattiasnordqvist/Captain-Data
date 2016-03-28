using System.Linq;

namespace CaptainData
{
    public class OverridesRuleSet : RuleSet
    {
        public override void Apply(RowInstruction rowInstruction, InstructionContext instructionContext)
        {
            var overrides = instructionContext.Overrides;

            var overridesDictionary = overrides?.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(overrides, null));

            var columns = instructionContext.CaptainContext.SchemaInformation[instructionContext.TableName];
            foreach (var column in columns)
            {
                if (overridesDictionary?.ContainsKey(column.ColumnName) ?? false)
                {
                    rowInstruction[column.ColumnName] = overridesDictionary[column.ColumnName];
                }
            }
        }
    }
}