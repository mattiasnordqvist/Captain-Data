using System;
using System.Data;

namespace CaptainData
{
    public class ColumnInstruction
    {
        private object _value;

        public ColumnInstruction(object value)
        {
            Value = value;
        }

        public object Value { set => _value = value; }

        public object GetValue()
        {
            if(_value is Delegate d)
            {
                _value = d.DynamicInvoke();
            }
            return _value;
        }

        public DbType? DbType { get; set; }

        public bool IgnoreColumn { get; private set; }

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

        /// <summary>
        /// Instruct default ruleset to ignore this column
        /// </summary>
        public static ColumnInstruction Ignore()
        {
            return new ColumnInstruction(null) { IgnoreColumn = true };
        }
    }
}