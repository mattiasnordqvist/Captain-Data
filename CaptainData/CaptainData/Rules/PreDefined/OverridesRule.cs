using CaptainData.Rules;
using System.Linq;

namespace CaptainData.Rules.PreDefined
{
    public class OverridesRule : IRule
    {
        public void Apply(RowInstruction rowInstruction)
        {
            var overrides = rowInstruction.Overrides;

            var overridesDictionary = overrides?.GetType().GetProperties().ToDictionary(x => x.Name, x => new ColumnInstruction(x.GetValue(overrides, null)));

            var columns = rowInstruction.CaptainContext.SchemaInformation[rowInstruction.TableName];
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