using System;
using System.Linq;
using CaptainData.Schema;

namespace CaptainData.Rules.PreDefined.Identity
{
    public class FKDefaults
    {
        public Table_Id Matches__Table_Id() => new Table_Id();
        public class Table_Id : ISmartIdInsertForeignKeyResolver
        {
            Func<ColumnSchema, string[], bool> ISmartIdInsertForeignKeyResolver.Is => (x, t) => (x.ColumnName.IndexOf("_Id") > 0) && t.Contains(x.ColumnName.Substring(0, x.ColumnName.IndexOf("_Id")));

            Func<ColumnSchema, string> ISmartIdInsertForeignKeyResolver.Get => x => x.ColumnName.Substring(0, x.ColumnName.Length - 3);
        }
    }
}