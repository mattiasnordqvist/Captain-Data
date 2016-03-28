﻿using System.Collections.Generic;
using System.Data;

using CaptainData.Schema;

namespace CaptainData
{
    public class Captain
    {
        private readonly IDbConnection _connection;

        private readonly IDbTransaction _transaction;

        public CaptainContext Context { get; }

        private readonly List<RuleSet> _rules = new List<RuleSet>(); 

        public Captain(IDbConnection connection, RuleSet customRules = null, IDbTransaction transaction = null)
        {
            _connection = connection;
            _transaction = transaction;

            var schemaInformation = SchemaInformation.Create(connection, transaction);
            Context = new CaptainContext(this, schemaInformation);
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
            Insert(instructionContext);
            return this;
        }

        public Captain Insert(InstructionContext instructionContext)
        {
            var instruction = new RowInstruction();
            instruction.SetContext(instructionContext);
            _rules.ForEach(x => x.Apply(instruction, instructionContext));
            Context.AddInstruction(instruction);
            return this;
        }
        public void Go()
        {
            Context.Apply(_connection, _transaction);
            Context.Clear();
        }
    }
}

    
