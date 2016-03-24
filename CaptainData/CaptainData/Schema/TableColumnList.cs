using System.Collections.Generic;
using System.Linq;

namespace CaptainData.Schema
{
    public class TableColumnList : List<ColumnSchema>
    {
        public TableColumnList(IEnumerable<ColumnSchema> source)
        {
            AddRange(source);
        }

        public ColumnSchema this[string columnName]
        {
            get
            {
                return this.First(x => x.ColumnName == columnName);
            }
        }
    }
}