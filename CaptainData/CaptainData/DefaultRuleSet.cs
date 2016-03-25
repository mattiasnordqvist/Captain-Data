using System;

using CaptainData.Schema;

namespace CaptainData
{
    public class DefaultRuleSet : RuleSet
    {

        public override void Apply(RowInstruction rowInstruction, InstructionContext instructionContext)
        {
            var columns = instructionContext.CaptainContext.SchemaInformation[instructionContext.TableName];
            foreach (var column in columns)
            {
                if (!rowInstruction.IsDefinedFor(column.ColumnName))
                {
                    ApplyDefaults(rowInstruction, column);
                }
            }
        }

        private void ApplyDefaults(RowInstruction rowInstruction, ColumnSchema column)
        {
            if (column.IsIdentity)
            {
                return;
            }

            if (column.IsNullable)
            {
                rowInstruction[column.ColumnName] = null;
            }
            else
            {
                switch (column.DataType)
                {
                    case SqlDataType.Int:
                        rowInstruction[column.ColumnName] = 0;
                        break;
                    case SqlDataType.Nvarchar:
                        rowInstruction[column.ColumnName] = string.Empty;
                        break;
                    case SqlDataType.Unknown:
                        throw new ArgumentException();
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        
    }
}