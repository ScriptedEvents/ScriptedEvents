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
                "Disable tesla gates entirely for 5 seconds, plus 5 seconds for each alive Chaos Insurgent.",
                @"TELSA DISABLE 5 + (5 * {CI})"),

            new Sample(
                "Case 2",
                "Disable tesla gate for facility guards.",
                @"TESLA ROLETYPE FacilityGuard"),
        };
    }
}
