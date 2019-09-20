using CaptainData.Schema;
using Dapper;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Tests
{
    public class Tests : TestsBase
    {
        
        [SetUp]
        public void Setup()
        {
            Captain.SchemaInformationFactory = new FakeSchemaFactory();
        }

        [Test]
        public async Task Insert_MissingDataForNvarcharColumn_GeneratesEmptyStringForColumn()
        {
            // Act
            await Captain.Insert("Person").Go(FakeConnection);

            // Assert
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Name]) VALUES (@Name);").AddSelectScope(), 
                ExpectedValues.New("Name",""));

        }

        [Test]
        public async Task Insert_FunctionsWork()
        {
            // Act
            await Captain.Insert("Person", new { Name = (Func<string>) (() => "lateboundname")}).Go(FakeConnection);

            // Assert
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Name]) VALUES (@Name);").AddSelectScope(),
                ExpectedValues.New("Name", "lateboundname"));

        }

        [Test]
        public async Task Insert_CallbacksWork()
        {
            int callbackCaptureId = 0;
            // Act
            await Captain.Insert("Person", new { Name = "test" }).Go(FakeConnection);
            await Captain.Insert("Person", new { Name = "test" }, callback: (Action<int>)(c => callbackCaptureId = c)).Go(FakeConnection);

            // Assert
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Name]) VALUES (@Name);").AddSelectScope(),
                ExpectedValues.New("Name", "test"));
            Assert.AreEqual(2, callbackCaptureId);

        }

        private class FakeSchemaFactory : ISchemaInformationFactory
        {
            public SchemaInformation Create(IDbConnection connection, IDbTransaction transaction)
            {
                return new SchemaInformation(new System.Collections.Generic.List<ColumnSchema>
                {
                    new ColumnSchema{TableName = "Person", ColumnName = "Id", DataType = "int", HasDefault = false, IsComputed = false, IsIdentity = true, IsNullable = false, TableSchema = "dbo" },
                    new ColumnSchema{TableName = "Person", ColumnName = "Name", DataType = "nvarchar", HasDefault = false, IsComputed = false, IsIdentity = false, IsNullable = false, TableSchema = "dbo" }
                });
            }
        }
    }
}