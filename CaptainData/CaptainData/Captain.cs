using System.Collections.Generic;
using System.Data.SqlClient;

using CaptainData.Schema;

namespace CaptainData
{
    public class Captain
    {
        private readonly SqlConnection _sqlConnection;
        private readonly CaptainContext _captainContext;
        private readonly List<RuleSet> _rules = new List<RuleSet>(); 

        public Captain(SqlConnection sqlConnection, RuleSet customRules = null)
        {
            _sqlConnection = sqlConnection;

            var schemaInformation = SchemaInformation.Create(sqlConnection);
            _captainContext = new CaptainContext(this, schemaInformation);
            AddRules(new OverridesRuleSet());
            if (customRules != null)
            {
                AddRules(customRules);
            }
            AddRules(new DefaultRuleSet());
        }

        private void AddRules(RuleSet ruleSet)
        {
            _rules.Add(ruleSet);
        }

        public Captain Insert(string tableName, object overrides = null)
        {
            var instructionContext = new InstructionContext { TableName = tableName, Overrides = overrides ?? new {}, CaptainContext = _captainContext};
            Insert(instructionContext);
            return this;
        }

        public Captain Insert(InstructionContext instructionContext)
        {
            var instruction = new RowInstruction();
            instruction.SetTable(instructionContext.TableName);
            _rules.ForEach(x => x.Apply(instruction, instructionContext));
            _captainContext.AddInstruction(instruction);
            return this;
        }
        public void Go()
        {
            _captainContext.Apply(_sqlConnection);
            _captainContext.Clear();
        }
    }
}

    
