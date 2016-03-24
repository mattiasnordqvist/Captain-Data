﻿using System.Data.SqlClient;

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

        public Captain Insert(string tableName)
        {
            var instruction = new Instruction { TableName = tableName };
            var columns = _context.SchemaInformation[tableName];

            foreach (var column in columns)
            {
                ApplyDefaults(instruction, column);
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
                switch (column.SqlDataType)
                {
                    case SqlDataType.Int:
                        instruction[column.ColumnName] = 0;
                        break;
                    case SqlDataType.Nvarchar:
                        instruction[column.ColumnName] = string.Empty;
                        break;
                }
            }
        }

        public void Go()
        {
            _context.Apply(_sqlConnection);
        }
    }
}

    
