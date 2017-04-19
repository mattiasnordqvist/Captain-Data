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
                switch (column.DataType.ToLower())
                {
                    case "int":
                    case "smallint":
                    case "bigint":
                    case "decimal":
                        rowInstruction[column.ColumnName] = 0;
                        break;
                    case "nvarchar":
                    case "varchar":
                        rowInstruction[column.ColumnName] = string.Empty;
                        break;
                    case "bit":
                        rowInstruction[column.ColumnName] = false;
                        break;
                    case "datetime":
                        rowInstruction[column.ColumnName] = new DateTime(1753, 1, 1, 12, 0, 0);
                        break;
                    case "varbinary":
                        rowInstruction[column.ColumnName] = new byte[0];
                        break;
                    default:
                        throw new NotImplementedException($"no default value for type {column.DataType}");
                }
            }
        }
    }
}