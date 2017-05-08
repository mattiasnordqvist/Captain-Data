using System;
using System.Collections;
using System.Data;

namespace CaptainData
{
    public static class TypeConverter
    {
        private struct DbTypeMapEntry
        {
            public readonly Type Type;
            public readonly DbType DbType;
            public readonly string SqlDbType;
            public DbTypeMapEntry(Type type, DbType dbType, string sqlDbType)
            {
                Type = type;
                DbType = dbType;
                SqlDbType = sqlDbType;
            }

        };

        private static readonly ArrayList DbTypeList = new ArrayList();


        static TypeConverter()
        {
            DbTypeList.Add(new DbTypeMapEntry(typeof(bool), DbType.Boolean, "bit"));
            DbTypeList.Add(new DbTypeMapEntry(typeof(DateTime), DbType.DateTime, "datetime"));
            DbTypeList.Add(new DbTypeMapEntry(typeof(DateTime), DbType.DateTime2, "datetime2"));
            DbTypeList.Add(new DbTypeMapEntry(typeof(DateTimeOffset), DbType.DateTimeOffset, "datetimeoffset"));
            DbTypeList.Add(new DbTypeMapEntry(typeof(decimal), DbType.Decimal, "decimal"));
            DbTypeList.Add(new DbTypeMapEntry(typeof(double), DbType.Double, "float"));
            DbTypeList.Add(new DbTypeMapEntry(typeof(Guid), DbType.Guid, "uniqueidentifier"));
            DbTypeList.Add(new DbTypeMapEntry(typeof(short), DbType.Int16, "smallint"));
            DbTypeList.Add(new DbTypeMapEntry(typeof(int), DbType.Int32, "int"));
            DbTypeList.Add(new DbTypeMapEntry(typeof(long), DbType.Int64, "bigint"));
            DbTypeList.Add(new DbTypeMapEntry(typeof(object), DbType.Object, "variant"));
            DbTypeList.Add(new DbTypeMapEntry(typeof(string), DbType.String, "varchar"));
            DbTypeList.Add(new DbTypeMapEntry(typeof(string), DbType.String, "nvarchar"));
            DbTypeList.Add(new DbTypeMapEntry(typeof(byte[]), DbType.Binary, "varbinary"));
            DbTypeList.Add(new DbTypeMapEntry(typeof(string), DbType.String, "geography"));
        }


        public static Type ToNetType(string sqlDbType)
        {
            var entry = Find(sqlDbType);
            return entry.Type;
        }

        
        public static DbType ToDbType(string sqlDbType)
        {
            var entry = Find(sqlDbType);
            return entry.DbType;
        }
    
       
        private static DbTypeMapEntry Find(string sqlDbType)
        {
            object retObj = null;
            foreach (var t in DbTypeList)
            {
                var entry = (DbTypeMapEntry)t;
                if (entry.SqlDbType.ToLower() == sqlDbType.ToLower())
                {
                    retObj = entry;
                    break;
                }
            }
            if (retObj == null)
            {
                throw
                    new ApplicationException($"Referenced an unsupported type ({sqlDbType})");
            }

            return (DbTypeMapEntry)retObj;
        }
    }
}