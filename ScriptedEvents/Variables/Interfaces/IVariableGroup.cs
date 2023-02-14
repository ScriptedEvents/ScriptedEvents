namespace ScriptedEvents.Variables.Interfaces
{
    using ScriptedEvents.API.Enums;

    /// <summary>
    /// Represents a standard grouping of variables.
    /// </summary>
    public interface IVariableGroup
    {
        /// <summary>
        /// Gets the type of variables contained in this group.
        /// </summary>
        public VariableGroupType GroupType { get; }

        /// <summary>
        /// Gets the variables present in this group.
        /// </summary>
        public IVariable[] Variables { get; }
    }
}
