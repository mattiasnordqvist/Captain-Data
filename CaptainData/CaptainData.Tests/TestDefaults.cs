using CaptainData.Schema;
using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class TestDefaults : TestsBase
    {
        [TestCase("int", 0)]
        [TestCase("smallint", 0)]
        [TestCase("bigint", 0)]
        [TestCase("decimal", 0)]
        [TestCase("numeric", 0)]
        public async Task Insert_Numeric_Defaults(string dataType, int expectedValue)
        {
            Captain.SchemaInformationFactory = new FakeSchemaFactory(dataType);
            await Captain.Insert("Test").Go(FakeConnection);
            AssertSql(
                ExpectedSql.New($"INSERT INTO Test ([{dataType}]) VALUES (@{dataType});").AddSelectScope(), 
                ExpectedValues.New(dataType, expectedValue));
        }

        [TestCase("nvarchar", "")]
        [TestCase("varchar", "")]
        public async Task Insert_String_Defaults(string dataType, string expectedValue)
        {
            Captain.SchemaInformationFactory = new FakeSchemaFactory(dataType);
            await Captain.Insert("Test").Go(FakeConnection);
            AssertSql(
                ExpectedSql.New($"INSERT INTO Test ([{dataType}]) VALUES (@{dataType});").AddSelectScope(), 
                ExpectedValues.New(dataType, expectedValue));
        }

        [TestCase("bit", false)]
        public async Task Insert_Boolean_Defaults(string dataType, bool expectedValue)
        {
            Captain.SchemaInformationFactory = new FakeSchemaFactory(dataType);
            await Captain.Insert("Test").Go(FakeConnection);
            AssertSql(
                ExpectedSql.New($"INSERT INTO Test ([{dataType}]) VALUES (@{dataType});").AddSelectScope(), 
                ExpectedValues.New(dataType, expectedValue));
        }

        [TestCase("date")]
        [TestCase("datetime")]
        [TestCase("datetime2")]
        public async Task Insert_DateTime_Defaults(string dataType)
        {
            Captain.SchemaInformationFactory = new FakeSchemaFactory(dataType);
            await Captain.Insert("Test").Go(FakeConnection);
            AssertSql(
                ExpectedSql.New($"INSERT INTO Test ([{dataType}]) VALUES (@{dataType});").AddSelectScope(), 
                ExpectedValues.New(dataType, new DateTime(1753, 1, 1, 12, 0, 0)));
        }

        [TestCase("datetimeoffset")]
        public async Task Insert_DateTimeOffset_Defaults(string dataType)
        {
            Captain.SchemaInformationFactory = new FakeSchemaFactory(dataType);
            await Captain.Insert("Test").Go(FakeConnection);
            AssertSql(
                ExpectedSql.New($"INSERT INTO Test ([{dataType}]) VALUES (@{dataType});").AddSelectScope(), 
                ExpectedValues.New(dataType, new DateTimeOffset(1753, 1, 1, 12, 0, 0, TimeSpan.Zero)));            
        }

        [TestCase("varbinary", new byte[0])]
        public async Task Insert_ByteArray_Defaults(string dataType, byte[] expectedValues)
        {
            Captain.SchemaInformationFactory = new FakeSchemaFactory(dataType);
            await Captain.Insert("Test").Go(FakeConnection);
            AssertSql(
                ExpectedSql.New($"INSERT INTO Test ([{dataType}]) VALUES (@{dataType});").AddSelectScope(), 
                ExpectedValues.New(dataType, expectedValues, (a,b) => a.SequenceEqual(b)));
        }

        private class FakeSchemaFactory : ISchemaInformationFactory
        {
            private readonly string[] _dataTypes;
            public FakeSchemaFactory(params string[] dataTypes) => _dataTypes = dataTypes;
            
            public SchemaInformation Create(IDbConnection connection, IDbTransaction transaction)
            {
                return new SchemaInformation(_dataTypes.Select(dataType => new ColumnSchema
                {
                    TableName = "Test",
                    ColumnName = dataType,
                    DataType = dataType,
                    TableSchema = "dbo"
                }).ToList());
            }                
        }
    }
}
