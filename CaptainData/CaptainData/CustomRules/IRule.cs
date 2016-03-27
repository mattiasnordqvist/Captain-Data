namespace CaptainData.CustomRules
{
    public interface IRule
    {
        void Apply(RowInstruction rowInstruction, InstructionContext instructionContext);
    }
}