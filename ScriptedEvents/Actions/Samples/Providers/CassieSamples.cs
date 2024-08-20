namespace ScriptedEvents.Actions.Samples.Providers
{
    using ScriptedEvents.Actions.Samples.Interfaces;

    public class CassieSamples : ISampleProvider
    {
        /// <inheritdoc/>
        public Sample[] Samples => new[]
        {
            new Sample(
                "Case 1",
                "Announce \"Hello\" with the caption of \"Goodbye\"",
                @"CASSIE LOUD Hello|Goodbye"),

            new Sample(
                "Case 2",
                "Announce \"Hello\" with the caption of \"Hello\". No caption is needed in this sample, since by default the caption is equivalent to the text CASSIE speaks.",
                @"CASSIE LOUD Hello"),

            new Sample(
                "Case 3",
                "Announce \"Hello\" with no caption.",
                @"CASSIE LOUD Hello|"),
        };
    }
}
