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
                "WAITSEC 500\nIF {PLAYERSALIVE} > 15\nDECONTAMINATE"),

            new Sample(
                "Random Light Flicker? idk",
                "Flicker the lights off for 10 seconds if there are exactly 8 players alive, exactly 5 players dead, exactly 3 SCPs, and at least 3 MTF.",
                "IF {PLAYERSALIVE} = 8 AND {PLAYERSDEAD} = 5 AND {SCPS} = 3 AND {MTF} >= 3\nLIGHTSOFF * 10"),

            new Sample(
                "Early Decontamination But BETTER",
                "Decontaminate LCZ after 500 seconds if there are more than 20 players in the server, OR if all the Class-D & Scientists are dead.",
                "WAITSEC 500\nIF {PLAYERS} > 20 OR ({CLASSD} = 0 AND {SCIENTIST} = 0)\nDECONTAMINATE"),
        };
    }
}
