using CaptainData.Schema;

namespace CaptainData.Rules
{
    public abstract class SingleColumnRule : IRule
    {
        public void Apply(RowInstruction rowInstruction)
        {
            var columns = rowInstruction.CaptainContext.SchemaInformation[rowInstruction.TableName];
            foreach (var column in columns)
            {
                if (Match(rowInstruction, column))
                {
                    Apply(rowInstruction, column);
                }
            }
        }

        public abstract bool Match(RowInstruction rowInstruction, ColumnSchema column);

        public abstract void Apply(RowInstruction rowInstruction, ColumnSchema column);

        

        
    }
}