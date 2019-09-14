using CaptainData;
using FakeItEasy;
using NUnit.Framework;
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
                SqlExecutor = sqlExecutor,
            };
        }

    }
}