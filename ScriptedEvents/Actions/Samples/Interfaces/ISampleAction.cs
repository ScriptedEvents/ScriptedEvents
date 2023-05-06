namespace ScriptedEvents.Actions.Samples.Interfaces
{
    /// <summary>
    /// Represents an action that can provide samples.
    /// </summary>
    public interface ISampleAction
    {
        /// <summary>
        /// Gets the <see cref="ISampleProvider"/> that this action will reference for its samples.
        /// </summary>
        public ISampleProvider Samples { get; }
    }
}
