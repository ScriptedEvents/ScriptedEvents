namespace ScriptedEvents.Actions.Samples.Providers
{
    using ScriptedEvents.Actions.Samples.Interfaces;

    public class RadioRangeSamples : ISampleProvider
    {
        /// <inheritdoc/>
        public Sample[] Samples => new[]
        {
            new Sample(
                "Case 1",
                "Locks all Class-D to only be able to use Short range. Does not affect players that spawn after the command is executed.",
                @"RADIORANGE LOCK {CLASSD} Short"),

            new Sample(
                "Case 2",
                "Set all guard radios to be Ultra by default. Does not affect players that spawn after the command is executed.",
                @"RADIORANGE SET {GUARDS} Ultra"),
        };
    }
}
