using CaptainData.Rules.PreDefined.Identity;
using CaptainData.Schema;
using Dapper;
using FakeItEasy;
using NUnit.Framework;
using System.Data;
using System.Threading.Tasks;

namespace Tests
{
    public class SmartIdInsertRuleTests : TestsBase
    {
     
        [SetUp]
        public void Setup()
        {
            Captain.AddRule(new SmartForeignKeyRule());
            Captain.SchemaInformationFactory = new FakeSchemaFactory();
        }

        [Test]
        public async Task Insert_ForeignKeyIsSetToLastGeneratedIdForReferencedTable()
        {
            A.CallTo(() => SqlExecutor.Execute(A<IDbConnection>.Ignored, A<string>.Ignored, A<DynamicParameters>.Ignored, A<IDbTransaction>.Ignored)).Returns(1);
            // Act
            await Captain
                .Insert("Family")
                .Insert("Person")
                .Go(FakeConnection);

            // Assert
            AssertSql(ExpectedSql.New("INSERT INTO Family () VALUES ();").AddSelectScope());
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Family_Id]) VALUES (@Family_Id);").AddSelectScope(), ExpectedValues.New("Family_Id", 1));
        }

        [Test]
        public async Task Insert_ForeignKeyIsSetToLastGeneratedIdForReferencedTable_TwoReferences()
        {
            A.CallTo(() => SqlExecutor.Execute(A<IDbConnection>.Ignored, A<string>.Ignored, A<DynamicParameters>.Ignored, A<IDbTransaction>.Ignored)).Returns(1);
            // Act
            await Captain
                .Insert("Family")
                .Insert("Person")
                .Insert("Person")
                .Go(FakeConnection);

            // Assert
            AssertSql(ExpectedSql.New("INSERT INTO Family () VALUES ();").AddSelectScope());
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Family_Id]) VALUES (@Family_Id);").AddSelectScope(), ExpectedValues.New("Family_Id", 1));
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Family_Id]) VALUES (@Family_Id);").AddSelectScope(), ExpectedValues.New("Family_Id", 1));
        }

        [Test]
        public async Task Insert_ForeignKeyIsSetToLastGeneratedIdForReferencedTable_TwoReferencesToTwoFamilies()
        {
            var nextId = 0;
            A.CallTo(() => SqlExecutor.Execute(A<IDbConnection>.Ignored, A<string>.Ignored, A<DynamicParameters>.Ignored, A<IDbTransaction>.Ignored)).ReturnsLazily(() => ++nextId);
            // Act
            await Captain
                .Insert("Family")
                .Insert("Person")
                .Insert("Person")
                .Insert("Family")
                .Insert("Person")
                .Insert("Person")
                .Go(FakeConnection);

            // Assert
            AssertSql(ExpectedSql.New("INSERT INTO Family () VALUES ();").AddSelectScope());
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Family_Id]) VALUES (@Family_Id);").AddSelectScope(), ExpectedValues.New("Family_Id", 1));
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Family_Id]) VALUES (@Family_Id);").AddSelectScope(), ExpectedValues.New("Family_Id", 1));
            AssertSql(ExpectedSql.New("INSERT INTO Family () VALUES ();").AddSelectScope());
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Family_Id]) VALUES (@Family_Id);").AddSelectScope(), ExpectedValues.New("Family_Id", 4));
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Family_Id]) VALUES (@Family_Id);").AddSelectScope(), ExpectedValues.New("Family_Id", 4));
        }



        private class FakeSchemaFactory : ISchemaInformationFactory
        {
            public SchemaInformation Create(IDbConnection connection, IDbTransaction transaction)
            {
                return new SchemaInformation(new System.Collections.Generic.List<ColumnSchema>
                {
                    new ColumnSchema{TableName = "Family", ColumnName = "Id", DataType = "int", HasDefault = false, IsComputed = false, IsIdentity = true, IsNullable = false, TableSchema = "dbo" },
                    new ColumnSchema{TableName = "Person", ColumnName = "Id", DataType = "int", HasDefault = false, IsComputed = false, IsIdentity = true, IsNullable = false, TableSchema = "dbo" },
                    new ColumnSchema{TableName = "Person", ColumnName = "Family_Id", DataType = "int", HasDefault = false, IsComputed = false, IsIdentity = false, IsNullable = false, TableSchema = "dbo" },
                });
            }
        }
    }
}