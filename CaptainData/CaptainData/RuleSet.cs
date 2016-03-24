namespace CaptainData
{
    public abstract class RuleSet
    {
        public RuleSet SetCaptainContext(CaptainContext captainContext)
        {
            CaptainContext = captainContext;
            return this;
        }

        public abstract void Apply(RowInstruction rowInstruction, InstructionContext instructionContext);

        public CaptainContext CaptainContext { get; private set; }
    }
}