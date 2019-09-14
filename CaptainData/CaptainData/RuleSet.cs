using CaptainData.CustomRules;

namespace CaptainData
{
    public abstract class RuleSet
    {
        public abstract void Apply(RowInstruction rowInstruction, InstructionContext instructionContext);
    }
}