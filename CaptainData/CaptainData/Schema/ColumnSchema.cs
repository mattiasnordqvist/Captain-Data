using System.Data;

namespace CaptainData.Schema
{
    public class ColumnSchema
    {
        public string ColumnName { get; set; }

        public string TableName { get; set; }

        public string TableSchema { get; set; }

        public bool IsNullable { get; set; }

        public SqlDbType DataType { get; set; }

        public bool IsIdentity { get; set; }

        public bool IsComputed { get; set; }
    }
}
