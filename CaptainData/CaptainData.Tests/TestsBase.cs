using CaptainData;
using Dapper;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;

namespace Tests
{
    public abstract class TestsBase
    {
        private readonly RuleSet customRules = null;
        protected ISqlExecutor sqlExecutor = A.Fake<ISqlExecutor>();
        protected IDbConnection fakeConnection = A.Fake<IDbConnection>();
        protected Captain captain;

        public TestsBase()
        {

        }
        public TestsBase(RuleSet customRules)
        {
            this.customRules = customRules;
        }

        [SetUp]
        public void BaseSetup()
        {
            captain = CreateCaptain(customRules);
        }

        protected Captain CreateCaptain(RuleSet customRules)
        {
            return new Captain(customRules)
            {
                SqlExecutor = sqlExecutor,
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