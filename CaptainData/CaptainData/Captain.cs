using System;
using System.Data.SqlClient;
using System.Linq;

using CaptainData.Schema;

namespace CaptainData
{
    public class Captain
    {
        private readonly SqlConnection _sqlConnection;
        private readonly Context _context;

        public Captain(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;

            var schemaInformation = SchemaInformation.Create(sqlConnection);
            _context = new Context(schemaInformation);
        }

        public Captain Insert(string tableName, object overrides)
        {
            var overridesDictionary = overrides.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(overrides, null));
            var instruction = new Instruction { TableName = tableName };
            var columns = _context.SchemaInformation[tableName];

            foreach (var column in columns)
            {
                if (overridesDictionary.ContainsKey(column.ColumnName))
                {
                    instruction[column.ColumnName] = overridesDictionary[column.ColumnName];
                }
                else
                {
                    ApplyDefaults(instruction, column);
                }
            }

            _context.AddInstruction(instruction);
            return this;
        }

        private void ApplyDefaults(Instruction instruction, ColumnSchema column)
        {
            if (column.IsIdentity)
            {
                return;
            }

            if (column.IsNullable)
            {
                instruction[column.ColumnName] = null;
            }
            else
            {
                switch (column.DataType)
                {
                    case SqlDataType.Int:
                        instruction[column.ColumnName] = 0;
                        break;
                    case SqlDataType.Nvarchar:
                        instruction[column.ColumnName] = string.Empty;
                        break;
                    case SqlDataType.Unknown:
                        throw new ArgumentException();
                }
            }
        }

        public void Go()
        {
            _context.Apply(_sqlConnection);
        }
    }
}

    
