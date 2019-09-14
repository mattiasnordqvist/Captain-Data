using CaptainData;
using CaptainData.CustomRules;
using CaptainData.CustomRules.PreDefined;
using CaptainData.Schema;
using NUnit.Framework;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class SmartIdInsertRuleTests : TestsBase
    {
        public SmartIdInsertRuleTests()
        {

        }

        [SetUp]
        public void Setup()
        {
            captain = CreateCaptain(new SmartIdInsertTestRules());
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
            AssertSql(ExpectedSql.New("INSERT INTO Family ([Id]) VALUES (@Id);").AddSelectScope(), ExpectedValues.New("Id",1));
            AssertSql(ExpectedSql.New("INSERT INTO Person ([Id], [Family_Id]) VALUES (@Id, @Family_Id);").AddSelectScope(), ExpectedValues.New("Id",1).Add("Family_Id", 1));
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

    internal class SmartIdInsertTestRules : BasicRuleSet
    {
        public SmartIdInsertTestRules()
        {
            AddRule(new SmartIntIdInsertRule()
                .EnableForeignKeys((x,t) => (x.ColumnName.IndexOf("_Id") > 0) && t.Contains(x.ColumnName.Substring(0,x.ColumnName.IndexOf("_Id"))),
                x => x.ColumnName.Substring(0, x.ColumnName.Length-3)));
            AddRule(new AllowIdentityInsertRule());
        }
    }
}