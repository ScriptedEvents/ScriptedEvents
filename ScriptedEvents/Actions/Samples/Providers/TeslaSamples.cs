namespace ScriptedEvents.Actions.Samples.Providers
{
    using ScriptedEvents.Actions.Samples.Interfaces;

    public class TeslaSamples : ISampleProvider
    {
        /// <inheritdoc/>
        public Sample[] Samples => new[]
        {
            new Sample(
                "Case 1",
                "Disable tesla gates entirely for 10 seconds.",
                @"TELSA DISABLE 10"),

            new Sample(
                "Case 2",
                "Disable tesla gate for facility guards.",
                @"TESLA ROLETYPE FacilityGuard"),
        };
    }
}
