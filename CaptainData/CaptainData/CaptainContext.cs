using System.Collections.Generic;
using System.Data.SqlClient;

using CaptainData.Schema;

namespace CaptainData
{
    public class CaptainContext
    {
        public SchemaInformation SchemaInformation { get; }

        public Captain Captain { get; private set; }

        private readonly List<RowInstruction> _instructions = new List<RowInstruction>();

        public CaptainContext(Captain captain, SchemaInformation schemaInformation)
        {
            Captain = captain;
            SchemaInformation = schemaInformation;
        }

        public void AddInstruction(RowInstruction rowInstruction)
        {
            _instructions.Add(rowInstruction);
        }

        public void Apply(SqlConnection sqlConnection)
        {
            _instructions.ForEach(x => x.Apply(sqlConnection));
        }

        public void Clear()
        {
            _instructions.Clear();
        }
    }
}