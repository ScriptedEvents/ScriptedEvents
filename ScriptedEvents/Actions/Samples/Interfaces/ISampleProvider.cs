namespace ScriptedEvents.Actions.Samples.Interfaces
{
    /// <summary>
    /// Provides samples for an action to reference.
    /// </summary>
    public interface ISampleProvider
    {
        /// <summary>
        /// The samples contained in this sample provider.
        /// </summary>
        public Sample[] Samples { get; }
    }
}
