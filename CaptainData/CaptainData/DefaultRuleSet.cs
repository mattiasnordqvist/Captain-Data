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
                    case SqlDataType.BigInt:
                    case SqlDataType.Decimal:
                        rowInstruction[column.ColumnName] = 0;
                        break;
                    case SqlDataType.Nvarchar:
                    case SqlDataType.Varchar:
                        rowInstruction[column.ColumnName] = string.Empty;
                        break;
                    case SqlDataType.Bit:
                        rowInstruction[column.ColumnName] = false;
                        break;
                    case SqlDataType.Datetime:
                        rowInstruction[column.ColumnName] = new DateTime(1753, 1, 1, 12, 0, 0);
                        break;
                    case SqlDataType.Varbinary:
                        rowInstruction[column.ColumnName] = new byte[0];
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