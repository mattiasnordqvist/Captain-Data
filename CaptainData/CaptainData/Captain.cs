using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

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

        public Captain(RuleSet customRules = null)
        {
            Context = new CaptainContext(this);
            AddRules(new OverridesRuleSet());
            if (customRules != null)
            {
                AddRules(customRules);
            }

            AddRules(new DefaultRuleSet());
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
            _rules.ForEach(x => x.Apply(instruction, instructionContext));
            AddInstruction(instruction);
        }

        public Captain Go(IDbTransaction transaction)
        {
            return Go(transaction.Connection, transaction);
        }

        public Captain Go(IDbConnection connection, IDbTransaction transaction = null)
        {
            if (Context.SchemaInformation == null)
            {
                Context.SchemaInformation = SchemaInformation.Create(connection, transaction);
            }

            _instructionContexts.ForEach(Insert);
            _instructions.ForEach(x => Apply(connection, transaction, x));
            ClearInstructions();
            return this;
        }

        protected virtual string CreateInsertStatement(RowInstruction rowInstruction)
        {
            return $"INSERT INTO {rowInstruction.InstructionContext.TableName} ({string.Join(", ", rowInstruction.ColumnInstructions.Keys)}) VALUES ({string.Join(", ", rowInstruction.ColumnInstructions.Keys.Select(x => $"@{x}"))});";
        }

        protected virtual string CreateGetScopeIdentityQuery(RowInstruction rowInstruction)
        {
            return "SELECT SCOPE_IDENTITY();";
        }

        internal void Apply(IDbConnection connection, IDbTransaction transaction, RowInstruction rowInstruction)
        {
            foreach (var action in rowInstruction.Before)
            {
                action(connection, transaction);
            }

            var sql = new StringBuilder();
            var values = new DynamicParameters(rowInstruction.ColumnInstructions.ToDictionary(x => x.Key, x => x.Value.Value).AsEnumerable());

            sql.AppendLine(CreateInsertStatement(rowInstruction));
            sql.AppendLine(CreateGetScopeIdentityQuery(rowInstruction));

            rowInstruction.InstructionContext.CaptainContext.ScopeIdentity = connection.ExecuteScalar(sql.ToString(), values, transaction);

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

        private void AddRules(RuleSet ruleSet)
        {
            _rules.Add(ruleSet);
        }
    }
}