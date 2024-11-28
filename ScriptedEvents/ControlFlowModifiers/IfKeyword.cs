using System.Collections.Generic;
using ScriptedEvents.ControlFlowModifiers.Interfaces;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.ControlFlowModifiers
{
    public class IfKeyword : BaseKeyword, IStandaloneKeyword, IRequiresContextBlock
    {
        public string Representation => "if";
        
        public string Description => "Represents an if block.";

        public IEnumerable<IAction> ContextActions { get; }

        public int EndsAtLine { get; }

        public IfKeyword(IEnumerable<IAction> actions, int endsAtLine)
        {
            ContextActions = actions;
            EndsAtLine = endsAtLine;
        }
    }
}