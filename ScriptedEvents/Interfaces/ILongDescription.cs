namespace ScriptedEvents.Interfaces
{
    /// <summary>
    /// Indicates an action that has an extra description portion.
    /// </summary>
    public interface ILongDescription
    {
        /// <summary>
        /// Gets the long description of an action.
        /// </summary>
        public string LongDescription { get; }
    }
}
