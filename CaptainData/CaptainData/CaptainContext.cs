using CaptainData.Schema;
using System.Collections.Generic;

namespace CaptainData
{
    public class CaptainContext : Dictionary<string, object>
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