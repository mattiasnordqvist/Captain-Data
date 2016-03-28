using CaptainData.Schema;

namespace CaptainData
{
    public class CaptainContext
    {
        public SchemaInformation SchemaInformation { get; internal set; }

        public Captain Captain { get; private set; }

        public object ScopeIdentity { get; internal set; }

        internal CaptainContext(Captain captain)
        {
            Captain = captain;
        }
    }
}