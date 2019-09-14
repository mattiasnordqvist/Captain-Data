namespace CaptainData.Rules
{
    public interface IRule
    {
        void Apply(RowInstruction rowInstruction, InstructionContext instructionContext);
    }
}