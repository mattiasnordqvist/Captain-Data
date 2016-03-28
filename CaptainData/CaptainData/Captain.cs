using System.Collections.Generic;
using System.Data;

namespace CaptainData
{
    public class Captain
    {
        public CaptainContext Context { get; }

        internal List<RuleSet> Rules => _rules;

        private readonly List<RuleSet> _rules = new List<RuleSet>();
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

        private void AddRules(RuleSet ruleSet)
        {
            _rules.Add(ruleSet);
        }

        public Captain Insert(string tableName, object overrides = null)
        {
            var instructionContext = new InstructionContext { TableName = tableName, Overrides = overrides ?? new {}, CaptainContext = Context};
            Context.AddInstructionContext(instructionContext);
            return this;
        }

        

        public Captain Go(IDbTransaction transaction)
        {
            return Go(transaction.Connection, transaction);
        }

        public Captain Go(IDbConnection connection, IDbTransaction transaction = null)
        {
            Context.Apply(connection, transaction);
            Context.ClearInstructions();
            return this;
        }
    }
}

    
