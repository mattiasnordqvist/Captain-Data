using CaptainData;
using Dapper;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Data;

namespace Tests
{
    public abstract class TestsBase
    {
        protected ISqlExecutor SqlExecutor = A.Fake<ISqlExecutor>();
        protected IDbConnection FakeConnection = A.Fake<IDbConnection>();
        protected Captain Captain;

        [SetUp]
        public void BaseSetup()
        {
            Captain = new Captain()
            {
                SqlExecutor = SqlExecutor
            };
        }

        protected void AssertSql(ExpectedSql expectedSql, Func<DynamicParameters, bool> p)
        {
            A.CallTo(() => SqlExecutor.Execute(
                A<IDbConnection>.Ignored,
                expectedSql,
                A<DynamicParameters>.That.Matches(p, "Parameters does not match"),
                A<IDbTransaction>.Ignored)).MustHaveHappened();
        }

        protected void AssertSql(ExpectedSql expectedSql)
        {
            AssertSql(expectedSql, x => true);
        }

    }
}