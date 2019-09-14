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
            captain.SchemaInformationFactory = new FakeSchemaFactory();
        }

        [Test]
        public async Task Insert_MissingDataForNvarcharColumn_GeneratesEmptyStringForColumn()
        {
            // Act
            await captain.Insert("Person").Go(fakeConnection);

            // Assert
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Name]) VALUES (@Name);").AddSelectScope(), 
                ExpectedValues.New("Name",""));

        }

        private void AssertSql(ExpectedSql expectedSql, Func<DynamicParameters, bool> p)
        {
            A.CallTo(() => sqlExecutor.Execute(
                A<IDbConnection>.Ignored,
                expectedSql,
                A<DynamicParameters>.That.Matches(p, "Parameters does not match"),
                A<IDbTransaction>.Ignored)).MustHaveHappened();
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