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
        protected ISqlExecutor sqlExecutor = A.Fake<ISqlExecutor>();
        protected IDbConnection fakeConnection = A.Fake<IDbConnection>();
        protected Captain captain;

        [SetUp]
        public void BaseSetup()
        {
            captain = new Captain()
            {
                SqlExecutor = sqlExecutor
            };
        }

        protected void AssertSql(ExpectedSql expectedSql, Func<DynamicParameters, bool> p)
        {
            A.CallTo(() => sqlExecutor.Execute(
                A<IDbConnection>.Ignored,
                expectedSql,
                A<DynamicParameters>.That.Matches(p, "Parameters does not match"),
                A<IDbTransaction>.Ignored)).MustHaveHappened();
        }

    }
}