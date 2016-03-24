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

        public Captain(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;

            var schemaInformation = SchemaInformation.Create(sqlConnection);
            _captainContext = new CaptainContext(schemaInformation);
            AddRules(new OverridesRuleSet());
            AddRules(new DefaultRuleSet());
        }

        public void AddRules(RuleSet ruleSet)
        {
            _rules.Add(ruleSet.SetCaptainContext(_captainContext));
        }

        public Captain Insert(string tableName, object overrides = null)
        {
            var instructionContext = new InstructionContext { TableName = tableName, ["overrides"] = overrides ?? new {} };
            var instruction = new RowInstruction();
            instruction.SetTable(tableName);
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

    
