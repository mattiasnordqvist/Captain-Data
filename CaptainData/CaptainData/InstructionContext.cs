using System.Collections.Generic;

namespace CaptainData
{
    public class InstructionContext : Dictionary<string, object>
    {
        public string TableName { get; set; }

        public T GetContext<T>(string name)
        {
            return (T)this[name];
        }
    }
}