using System.Data.SqlClient;

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
    }
}

    
