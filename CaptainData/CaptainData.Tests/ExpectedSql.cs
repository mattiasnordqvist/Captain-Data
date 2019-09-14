namespace Tests
{
    public class ExpectedSql
    {
        private string sql = "";

        public ExpectedSql(string sql)
        {
            this.Add(sql);
        }

        internal static ExpectedSql New(string sql)
        {

            return new ExpectedSql(sql);
        }

        internal ExpectedSql Add(string sql)
        {
            this.sql += sql + "\r\n";
            return this;
        }

        internal ExpectedSql AddSelectScope()
        {
            sql += "SELECT SCOPE_IDENTITY();\r\n";
            return this;
        }

        public static implicit operator string(ExpectedSql expectedSql)
        {
            return expectedSql.sql;
        }
    }
}