namespace CaptainData.Schema
{
    public class ColumnSchema
    {
        public string ColumnName { get; set; }

        public string TableName { get; set; }

        public bool IsNullable { get; set; }

        public SqlDataType SqlDataType { get; set; }

        public bool IsIdentity { get; set; }
    }
}
