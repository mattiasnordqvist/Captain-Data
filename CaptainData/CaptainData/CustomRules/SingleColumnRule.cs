using CaptainData.Schema;

namespace CaptainData
{
    public abstract class SingleColumnRule : IRule
    {
        public void Apply(RowInstruction rowInstruction, InstructionContext instructionContext)
        {
            var columns = instructionContext.CaptainContext.SchemaInformation[instructionContext.TableName];
            foreach (var column in columns)
            {
                if (Match(rowInstruction, column, instructionContext))
                {
                    Apply(rowInstruction, column, instructionContext);
                }
            }
        }

        public abstract bool Match(RowInstruction rowInstruction, ColumnSchema column, InstructionContext instructionContext);

        public abstract void Apply(RowInstruction rowInstruction, ColumnSchema column, InstructionContext instructionContext);

        

        
    }
}