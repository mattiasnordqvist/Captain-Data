using CaptainData.Schema;

namespace CaptainData
{
    public class Context
    {
        public SchemaInformation SchemaInformation { get; }

        public Context(SchemaInformation schemaInformation)
        {
            SchemaInformation = schemaInformation;
        }
    }
}