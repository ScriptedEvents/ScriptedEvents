namespace ScriptedEvents.Variables.Interfaces
{
    public interface IObjectVariable : IConditionVariable
    {
        /// <summary>
        /// Gets the value of this variable.
        /// </summary>
        public object Value { get; }
    }
}
