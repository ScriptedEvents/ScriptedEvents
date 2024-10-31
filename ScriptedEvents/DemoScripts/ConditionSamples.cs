namespace ScriptedEvents.DemoScripts
{
    /// <summary>
    /// Demo script with examples of conditions.
    /// </summary>
    public class ConditionSamples : IDemoScript
    {
        /// <inheritdoc/>
        public string FileName => "ConditionSamples";

        /// <inheritdoc/>
        public string Contents => @"!-- DISABLE
# Wait until 10 seconds after the round started.
WAIT UNTIL {TIME:ROUNDSECONDS} > 10

# End the script immediately if there are more than 5 players alive.
STOP $IF {PLAYERSALIVE} > 5

# Wait anywhere from 100 to 500 seconds.
WAIT SEC {RANDOM:INT:100:500}

# Lights off for anywhere from 10 to 20 seconds, plus 1 second for each player.
LIGHTSOFF * #1
// {PLAYERS} + {RANDOM:INT:10:20}
";
    }
}
