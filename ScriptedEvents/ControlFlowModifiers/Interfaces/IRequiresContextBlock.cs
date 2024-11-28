using System.Collections;
using System.Collections.Generic;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.ControlFlowModifiers.Interfaces
{
    /// <summary>
    /// Represents a keyword after which `then` block must be used and actions have to be provided.
    /// </summary>
    public interface IRequiresContextBlock : IControlFlowModifier
    {
        public IEnumerable<IAction> ContextActions { get; }
        
        public int EndsAtLine { get; }
    }
}