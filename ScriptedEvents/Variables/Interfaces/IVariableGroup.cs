namespace ScriptedEvents.Variables.Interfaces
{
    /// <summary>
    /// Represents a standard grouping of variables.
    /// </summary>
    public interface IVariableGroup
    {
        /// <summary>
        /// Gets the name of the group.
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// Gets the variables present in this group.
        /// </summary>
        public IVariable[] Variables { get; }
    }
}
