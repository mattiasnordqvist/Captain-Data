using CaptainData.Schema;
using System.Collections.Generic;

namespace CaptainData
{
    public class CaptainContext : Dictionary<string, object>
    {
        public SchemaInformation SchemaInformation { get; internal set; }

        public Captain Captain { get; private set; }

        public object LastId(string tableName) => LastIds()[SchemaInformation.FTN(tableName)];
        public object ScopeIdentity { get; internal set; }

        internal CaptainContext(Captain captain)
        {
            Captain = captain;
        }

        internal Dictionary<string, object> LastIds()
        {
            if (!ContainsKey("LastIdContext"))
            {
                this["LastIdContext"] = new Dictionary<string, object>();
            }
            return (Dictionary<string, object>)this["LastIdContext"];
        }
    }
}