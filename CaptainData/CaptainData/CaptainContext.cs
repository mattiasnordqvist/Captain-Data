using System.Collections.Generic;
using System.Data;

using CaptainData.Schema;

namespace CaptainData
{
    public class CaptainContext
    {
        public SchemaInformation SchemaInformation { get; private set; }

        public Captain Captain { get; private set; }

        public object ScopeIdentity { get; set; }

        private readonly List<RowInstruction> _instructions = new List<RowInstruction>();

        public CaptainContext(Captain captain)
        {
            Captain = captain;
        }

        public void AddInstruction(RowInstruction rowInstruction)
        {
            _instructions.Add(rowInstruction);
        }

        public void Insert(InstructionContext instructionContext)
        {
            var instruction = new RowInstruction();
            instruction.SetContext(instructionContext);
            Captain.Rules.ForEach(x => x.Apply(instruction, instructionContext));
            AddInstruction(instruction);
        }

        public void Apply(IDbConnection connection, IDbTransaction transaction)
        {
            if (SchemaInformation == null)
            {
                SchemaInformation = SchemaInformation.Create(connection, transaction);
            }

            _instructionContexts.ForEach(Insert);
            _instructions.ForEach(x => x.Apply(connection, transaction));
        }

        public void ClearInstructions()
        {
            _instructions.Clear();
            _instructionContexts.Clear();
        }

        private readonly List<InstructionContext> _instructionContexts = new List<InstructionContext>(); 

        internal void AddInstructionContext(InstructionContext instructionContext)
        {
            _instructionContexts.Add(instructionContext);
        }
    }
}