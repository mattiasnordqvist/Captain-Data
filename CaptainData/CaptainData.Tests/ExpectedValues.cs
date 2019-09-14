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
        internal static ExpectedValues Empty()
        {
            return new ExpectedValues();
        }

        public ExpectedValues Add<T>(string name, T value)
        {
            Predicates.Add(x => x.Get<T>(name).Equals(value));
            return this;
        }

        public static implicit operator Func<DynamicParameters, bool>(ExpectedValues expectedValues)
        {
            return x => expectedValues.Predicates.All(p => p(x));
        }
    }
}