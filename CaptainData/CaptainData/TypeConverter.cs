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
            public readonly SqlDbType SqlDbType;
            public DbTypeMapEntry(Type type, DbType dbType, SqlDbType sqlDbType)
            {
                Type = type;
                DbType = dbType;
                SqlDbType = sqlDbType;
            }

        };

        private static readonly ArrayList DbTypeList = new ArrayList();


        static TypeConverter()
        {
            DbTypeList.Add(new DbTypeMapEntry(typeof(bool), DbType.Boolean, SqlDbType.Bit));
            DbTypeList.Add(new DbTypeMapEntry(typeof(DateTime), DbType.DateTime, SqlDbType.DateTime));
            DbTypeList.Add(new DbTypeMapEntry(typeof(decimal), DbType.Decimal, SqlDbType.Decimal));
            DbTypeList.Add(new DbTypeMapEntry(typeof(double), DbType.Double, SqlDbType.Float));
            DbTypeList.Add(new DbTypeMapEntry(typeof(Guid), DbType.Guid, SqlDbType.UniqueIdentifier));
            DbTypeList.Add(new DbTypeMapEntry(typeof(short), DbType.Int16, SqlDbType.SmallInt));
            DbTypeList.Add(new DbTypeMapEntry(typeof(int), DbType.Int32, SqlDbType.Int));
            DbTypeList.Add(new DbTypeMapEntry(typeof(long), DbType.Int64, SqlDbType.BigInt));
            DbTypeList.Add(new DbTypeMapEntry(typeof(object), DbType.Object, SqlDbType.Variant));
            DbTypeList.Add(new DbTypeMapEntry(typeof(string), DbType.String, SqlDbType.VarChar));
            DbTypeList.Add(new DbTypeMapEntry(typeof(string), DbType.String, SqlDbType.NVarChar));
            DbTypeList.Add(new DbTypeMapEntry(typeof(byte[]), DbType.Binary, SqlDbType.VarBinary));
        }


        public static Type ToNetType(SqlDbType sqlDbType)
        {
            var entry = Find(sqlDbType);
            return entry.Type;
        }

        
        public static DbType ToDbType(SqlDbType sqlDbType)
        {
            var entry = Find(sqlDbType);
            return entry.DbType;
        }
    
       
        private static DbTypeMapEntry Find(SqlDbType sqlDbType)
        {
            object retObj = null;
            foreach (var t in DbTypeList)
            {
                var entry = (DbTypeMapEntry)t;
                if (entry.SqlDbType == sqlDbType)
                {
                    retObj = entry;
                    break;
                }
            }
            if (retObj == null)
            {
                throw
                    new ApplicationException("Referenced an unsupported SqlDbType");
            }

            return (DbTypeMapEntry)retObj;
        }
    }
}