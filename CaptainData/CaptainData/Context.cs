using System.Collections.Generic;
using System.Data.SqlClient;

using CaptainData.Schema;

namespace CaptainData
{
    public class Context
    {
        public SchemaInformation SchemaInformation { get; }

        private readonly List<Instruction> _instructions = new List<Instruction>();

        public Context(SchemaInformation schemaInformation)
        {
            SchemaInformation = schemaInformation;
        }

        public void AddInstruction(Instruction instruction)
        {
            _instructions.Add(instruction);
        }

        public void Apply(SqlConnection sqlConnection)
        {
            _instructions.ForEach(x => x.Apply(sqlConnection));
        }
    }
}