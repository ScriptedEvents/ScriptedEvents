namespace ScriptedEvents.ControlFlowModifiers.Interfaces
{
    /// <summary>
    /// Represents a control flow modifier.
    /// </summary>
    public interface IControlFlowModifier
    {
        public string Representation { get; }
        
        public string Description { get; }
    }
}