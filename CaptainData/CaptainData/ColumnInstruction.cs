using System;
using System.Data;

namespace CaptainData
{
    public class ColumnInstruction
    {

        public ColumnInstruction(object value)
        {
            Value = value;
        }

        public object Value { get; set; }

        public DbType? DbType { get; set; }

        public static implicit operator ColumnInstruction(DateTime value)
        {
            return new ColumnInstruction(value)
            {
                DbType = System.Data.DbType.DateTime
            };
        }

        public static implicit operator ColumnInstruction(DateTimeOffset value)
        {
            return new ColumnInstruction(value)
            {
                DbType = System.Data.DbType.DateTimeOffset
            };
        }

        public static implicit operator ColumnInstruction(bool value)
        {
            return new ColumnInstruction(value)
            {
                Value = value,
                DbType = System.Data.DbType.Boolean
            };
        }

        public static implicit operator ColumnInstruction(int value)
        {
            return new ColumnInstruction(value)
            {
                DbType = System.Data.DbType.Int32
            };
        }

        public static implicit operator ColumnInstruction(string value)
        {
            return new ColumnInstruction(value)
            {
                DbType = System.Data.DbType.String
            };
        }

        public static implicit operator ColumnInstruction(byte[] value)
        {
            return new ColumnInstruction(value)
            {
                DbType = System.Data.DbType.Binary
            };
        }
    }
}