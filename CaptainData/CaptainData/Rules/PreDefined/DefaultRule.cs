﻿using System;
using CaptainData.Schema;

namespace CaptainData.Rules.PreDefined
{
    public class DefaultRule : IRule
    {
        public void Apply(RowInstruction rowInstruction)
        {
            var columns = rowInstruction.CaptainContext.SchemaInformation[rowInstruction.TableName];
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

            if (column.HasDefault)
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
                    case "numeric":
                        rowInstruction[column.ColumnName] = 0;
                        break;
                    case "nvarchar":
                    case "varchar":
                        rowInstruction[column.ColumnName] = string.Empty;
                        break;
                    case "bit":
                        rowInstruction[column.ColumnName] = false;
                        break;
                    case "date":
                    case "datetime":
                    case "datetime2":
                        rowInstruction[column.ColumnName] = new DateTime(1753, 1, 1, 12, 0, 0);
                        break;
                    case "varbinary":
                        rowInstruction[column.ColumnName] = new byte[0];
                        break;
                    case "datetimeoffset":
                        rowInstruction[column.ColumnName] = new DateTimeOffset(1753, 1, 1, 12, 0 ,0 ,TimeSpan.Zero);
                        break;
                    default:
                        throw new NotImplementedException($"no default value for type {column.DataType} on column {column.TableSchema}.{column.TableName}.{column.ColumnName}");
                }
            }
        }
    }
}