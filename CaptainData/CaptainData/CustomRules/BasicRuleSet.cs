using System.Collections.Generic;

namespace CaptainData.CustomRules
{
    public class BasicRuleSet : RuleSet
    {
        private readonly List<IRule> _rules = new List<IRule>();

        public override void Apply(RowInstruction rowInstruction, InstructionContext instructionContext)
        {
            foreach (var rule in _rules)
            {
                rule.Apply(rowInstruction, instructionContext);
            }
        }

        protected void AddRule(IRule rule)
        {
            _rules.Add(rule);
        }
    }
}