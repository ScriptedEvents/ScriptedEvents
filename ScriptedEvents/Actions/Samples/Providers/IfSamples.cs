namespace ScriptedEvents.Actions.Samples.Providers
{
    using ScriptedEvents.Actions.Samples.Interfaces;

    public class IfSamples : ISampleProvider
    {
        /// <inheritdoc/>
        public Sample[] Samples => new[]
        {
            new Sample(
                "Early Decontamination",
                "Begin LCZ decontamination automatically after 500 seconds if there are more than 15 alive players.",
                "WAIT SEC 500\nSTOP $IF {PLAYERSALIVE} <= 15\nDECONTAMINATE"),

            new Sample(
                "Random Light Flicker? idk",
                "Flicker the lights off for 10 seconds if there are more than 5 players in LCZ.",
                "STOP $IF {LCZ} <= 5\nLIGHTSOFF * 10"),

            new Sample(
                "Early Decontamination But BETTER",
                "Decontaminate LCZ after 500 seconds if all the Class-D & Scientists are dead.",
                "WAIT SEC 500\nSTOP $IF {CLASSD} != 0 OR {SCIENTIST} != 0\nDECONTAMINATE"),
        };
    }
}
