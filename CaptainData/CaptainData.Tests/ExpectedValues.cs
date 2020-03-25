using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    internal class ExpectedValues
    {
        private List<Func<DynamicParameters, bool>> Predicates = new List<Func<DynamicParameters, bool>>{ x => true };
        public ExpectedValues()
        {
        }

        internal static ExpectedValues New<T>(string name, T value)
        {
            var e =  new ExpectedValues();
            return e.Add(name, value);
        }

        internal static ExpectedValues New<T>(string name, T value, Func<T,T,bool> comparator)
        {
            var e = new ExpectedValues();
            return e.Add(name, value, comparator);
        }
        internal static ExpectedValues Empty()
        {
            return new ExpectedValues();
        }

        public ExpectedValues Add<T>(string name, T value)
        {
            Add(name, value, (a, b) => a.Equals(b));
            return this;
        }

        public ExpectedValues Add<T>(string name, T value, Func<T,T,bool> comparator)
        {
            Predicates.Add(x => comparator(x.Get<T>(name),value));
            return this;
        }

        public static implicit operator Func<DynamicParameters, bool>(ExpectedValues expectedValues)
        {
            return x => expectedValues.Predicates.All(p => p(x));
        }
    }
}