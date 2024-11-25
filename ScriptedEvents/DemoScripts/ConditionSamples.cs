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
#Wait until 10 seconds after the round started.
WaitUntil {ROUNDSECONDS} > 10

#End the script immediately if there are more than 5 players alive.
STOPIF {PLAYERSALIVE} > 5

#Wait anywhere from 100 to 500 seconds (CHANCE always generates a random value from 0-1).
WAITSEC 100 + (400 * {CHANCE})

# Lights off for anywhere from 10 to 20 seconds, plus 1 second for each player.
# The ""SAVE"" action creates a new variable using the input formula, and supports basic math operations.
SAVE {LIGHTS_DURATION} (1 * {PLAYERS}) + 10 + (10 * {CHANCE})
LIGHTSOFF * {LIGHTS_DURATION}
";
    }
}
