using System.Collections.Generic;

namespace CaptainData
{
    public class InstructionContext : Dictionary<string, object>
    {
        public CaptainContext CaptainContext { get; set; }

        public string TableName { get; set; }

        public object Overrides { get; set; }
    }
}