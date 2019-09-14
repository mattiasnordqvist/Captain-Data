using CaptainData.Rules.PreDefined.Identity;
using CaptainData.Schema;
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
            captain.AddRule(new SmartIntIdInsertRule()
                .EnableForeignKeys(x => x.Matches__Table_Id()));
            captain.AddRule(new AllowIdentityInsertRule());
            captain.SchemaInformationFactory = new FakeSchemaFactory();
        }

        [Test]
        public async Task Insert_ForeignKeyIsSetToLastGeneratedIdForReferencedTable()
        {
            // Act
            await captain
                .Insert("Family")
                .Insert("Person")
                .Go(fakeConnection);

            // Assert
            AssertSql(ExpectedSql.New("INSERT INTO Family ([Id]) VALUES (@Id);").AddSelectScope(), ExpectedValues.New("Id", 1));
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Id], [Family_Id]) VALUES (@Id, @Family_Id);").AddSelectScope(), ExpectedValues.New("Id", 1).Add("Family_Id", 1));
        }

        [Test]
        public async Task Insert_ForeignKeyIsSetToLastGeneratedIdForReferencedTable_TwoReferences()
        {
            // Act
            await captain
                .Insert("Family")
                .Insert("Person")
                .Insert("Person")
                .Go(fakeConnection);

            // Assert
            AssertSql(ExpectedSql.New("INSERT INTO Family ([Id]) VALUES (@Id);").AddSelectScope(), ExpectedValues.New("Id", 1));
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Id], [Family_Id]) VALUES (@Id, @Family_Id);").AddSelectScope(), ExpectedValues.New("Id", 1).Add("Family_Id", 1));
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Id], [Family_Id]) VALUES (@Id, @Family_Id);").AddSelectScope(), ExpectedValues.New("Id", 2).Add("Family_Id", 1));
        }

        [Test]
        public async Task Insert_ForeignKeyIsSetToLastGeneratedIdForReferencedTable_TwoReferencesToTwoFamilies()
        {
            // Act
            await captain
                .Insert("Family")
                .Insert("Person")
                .Insert("Person")
                .Insert("Family")
                .Insert("Person")
                .Insert("Person")
                .Go(fakeConnection);

            // Assert
            AssertSql(ExpectedSql.New("INSERT INTO Family ([Id]) VALUES (@Id);").AddSelectScope(), ExpectedValues.New("Id", 1));
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Id], [Family_Id]) VALUES (@Id, @Family_Id);").AddSelectScope(), ExpectedValues.New("Id", 1).Add("Family_Id", 1));
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Id], [Family_Id]) VALUES (@Id, @Family_Id);").AddSelectScope(), ExpectedValues.New("Id", 2).Add("Family_Id", 1));
            AssertSql(ExpectedSql.New("INSERT INTO Family ([Id]) VALUES (@Id);").AddSelectScope(), ExpectedValues.New("Id", 2));
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Id], [Family_Id]) VALUES (@Id, @Family_Id);").AddSelectScope(), ExpectedValues.New("Id", 3).Add("Family_Id", 2));
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Id], [Family_Id]) VALUES (@Id, @Family_Id);").AddSelectScope(), ExpectedValues.New("Id", 4).Add("Family_Id", 2));
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