using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainData.Rules;
using CaptainData.Rules.PreDefined;
using CaptainData.Schema;

using Dapper;

namespace CaptainData
{
    public class Captain
    {
        public CaptainContext Context { get; }

        private readonly List<IRule> _rules = new List<IRule>();
        private readonly List<RowInstruction> _instructions = new List<RowInstruction>();

        public ISqlGenerator SqlGenerator { get; set; } = new SqlGenerator();
        public ISqlExecutor SqlExecutor { get; set; } = new SqlExecutor();
        public ISchemaInformationFactory SchemaInformationFactory { get; set; } = new SchemaInformationFactory();

        public IRule DefaultRuleSet { get; set; } = new DefaultRule();
        public IRule OverridesRuleSet { get; set; } = new OverridesRule();

        public Captain()
        {
            Context = new CaptainContext(this);
        }

        public Captain Insert(string tableName, object overrides = null, Delegate callback = null)
        {
            var instruction = new RowInstruction {
                TableName = tableName,
                Overrides = overrides ?? new { },
                CaptainContext = this.Context,
                Callback = callback,
            };
            AddInstruction(instruction);
            return this;
        }


        public async Task<Captain> Go(IDbTransaction transaction)
        {
            return await Go(transaction.Connection, transaction);
        }

        public async Task<Captain> Go(IDbConnection connection, IDbTransaction transaction = null)
        {
            if (Context.SchemaInformation == null)
            {
                Context.SchemaInformation = SchemaInformationFactory.Create(connection, transaction);
            }

            foreach (var x in _instructions)
            {
                await Apply(connection, transaction, x);
            }

            ClearInstructions();
            return this;
        }

        
        internal async Task Apply(IDbConnection connection, IDbTransaction transaction, RowInstruction rowInstruction)
        {
            OverridesRuleSet?.Apply(rowInstruction);
            _rules.ForEach(x => x.Apply(rowInstruction));
            DefaultRuleSet?.Apply(rowInstruction);

            var sql = new StringBuilder();
            var values = new DynamicParameters();
            
            foreach (var x in rowInstruction.ColumnInstructions)
            {
                values.Add(x.Key, x.Value.GetValue(), x.Value.DbType);
            }

            if (rowInstruction.RequiresIdentityInsert)
            {
                sql.AppendLine($"SET IDENTITY_INSERT {rowInstruction.TableName} ON");
            }
            sql.AppendLine(SqlGenerator.CreateInsertStatement(rowInstruction));
            sql.AppendLine(SqlGenerator.CreateGetScopeIdentityQuery(rowInstruction));
            if (rowInstruction.RequiresIdentityInsert)
            {
                sql.AppendLine($"SET IDENTITY_INSERT {rowInstruction.TableName} OFF");
            }
            var lastId = await SqlExecutor.Execute(connection, sql.ToString(), values, transaction);
            rowInstruction.CaptainContext.ScopeIdentity = lastId;

            var fullTableName = SchemaInformation.FTN(rowInstruction.TableName);

            if (!rowInstruction.CaptainContext.LastIds().ContainsKey(fullTableName))
            {
                rowInstruction.CaptainContext.LastIds().Add(fullTableName, lastId);
            }
            else
            {
                rowInstruction.CaptainContext.LastIds()[fullTableName] = lastId;
            }
            rowInstruction.Callback?.DynamicInvoke(lastId);

        }

        internal void AddInstruction(RowInstruction rowInstruction)
        {
            _instructions.Add(rowInstruction);
        }

        internal void ClearInstructions()
        {
            _instructions.Clear();
        }

        public void AddRule(IRule rule)
        {
            _rules.Add(rule);
        }
    }
}