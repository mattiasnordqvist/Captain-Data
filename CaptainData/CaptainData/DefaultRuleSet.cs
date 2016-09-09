using System;
using System.Data;

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
            if (column.IsComputed)
            {
                return;
            }

            if (column.IsIdentity)
            {
                return;
            }

            if (column.IsNullable)
            {
                rowInstruction[column.ColumnName] = new ColumnInstruction(null) {DbType = TypeConverter.ToDbType(column.DataType)};
            }
            else
            {
                switch (column.DataType)
                {
                    case SqlDbType.Int:
                    case SqlDbType.SmallInt:
                    case SqlDbType.BigInt:
                    case SqlDbType.Decimal:
                        rowInstruction[column.ColumnName] = 0;
                        break;
                    case SqlDbType.NVarChar:
                    case SqlDbType.VarChar:
                        rowInstruction[column.ColumnName] = string.Empty;
                        break;
                    case SqlDbType.Bit:
                        rowInstruction[column.ColumnName] = false;
                        break;
                    case SqlDbType.DateTime:
                        rowInstruction[column.ColumnName] = new DateTime(1753, 1, 1, 12, 0, 0);
                        break;
                    case SqlDbType.VarBinary:
                        rowInstruction[column.ColumnName] = new byte[0];
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}