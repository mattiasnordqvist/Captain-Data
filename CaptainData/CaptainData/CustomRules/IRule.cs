namespace CaptainData
{
    public interface IRule
    {
        void Apply(RowInstruction rowInstruction, InstructionContext instructionContext);
    }
}