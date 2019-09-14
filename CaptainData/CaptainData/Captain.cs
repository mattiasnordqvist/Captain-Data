using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainData.Schema;

using Dapper;

namespace CaptainData
{
    public class Captain
    {
        public CaptainContext Context { get; }

        private readonly List<RuleSet> _rules = new List<RuleSet>();
        private readonly List<RowInstruction> _instructions = new List<RowInstruction>();
        private readonly List<InstructionContext> _instructionContexts = new List<InstructionContext>();

        public ISqlGenerator SqlGenerator { get; set; } = new SqlGenerator();
        public ISqlExecutor SqlExecutor { get; set; } = new SqlExecutor();
        public ISchemaInformationFactory SchemaInformationFactory { get; set; } = new SchemaInformationFactory();

        public RuleSet DefaultRuleSet { get; set; } = new DefaultRuleSet();
        public RuleSet OverridesRuleSet { get; set; } = new OverridesRuleSet();

        public Captain()
        {
            Context = new CaptainContext(this);
        }

        public Captain Insert(string tableName, object overrides = null)
        {
            var instructionContext = new InstructionContext { TableName = tableName, Overrides = overrides ?? new { }, CaptainContext = Context };
            AddInstructionContext(instructionContext);
            return this;
        }

        public void Insert(InstructionContext instructionContext)
        {
            var instruction = new RowInstruction();
            instruction.SetContext(instructionContext);
            OverridesRuleSet?.Apply(instruction, instructionContext);
            _rules.ForEach(x => x.Apply(instruction, instructionContext));
            DefaultRuleSet?.Apply(instruction, instructionContext);

            AddInstruction(instruction);
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

            _instructionContexts.ForEach(Insert);
            foreach (var x in _instructions)
            {
                await Apply(connection, transaction, x);
            }

            ClearInstructions();
            return this;
        }

        
        internal async Task Apply(IDbConnection connection, IDbTransaction transaction, RowInstruction rowInstruction)
        {
            foreach (var action in rowInstruction.Before)
            {
                action(connection, transaction);
            }

            var sql = new StringBuilder();
            var values = new DynamicParameters();
            foreach (var x in rowInstruction.ColumnInstructions)
            {
                values.Add(x.Key, x.Value.Value, x.Value.DbType);
            }

            sql.AppendLine(SqlGenerator.CreateInsertStatement(rowInstruction));
            sql.AppendLine(SqlGenerator.CreateGetScopeIdentityQuery(rowInstruction));

            rowInstruction.InstructionContext.CaptainContext.ScopeIdentity = await SqlExecutor.Execute(connection, sql.ToString(), values, transaction);

            foreach (var action in rowInstruction.After)
            {
                action(connection, transaction);
            }
        }

        internal void AddInstruction(RowInstruction rowInstruction)
        {
            _instructions.Add(rowInstruction);
        }

        internal void AddInstructionContext(InstructionContext instructionContext)
        {
            _instructionContexts.Add(instructionContext);
        }

        internal void ClearInstructions()
        {
            _instructions.Clear();
            _instructionContexts.Clear();
        }

        public void AddRules(RuleSet ruleSet)
        {
            _rules.Add(ruleSet);
        }
    }
}