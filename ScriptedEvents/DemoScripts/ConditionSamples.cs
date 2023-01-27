using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.DemoScripts
{
    public class ConditionSamples : IDemoScript
    {

        public string FileName => "ConditionSamples";
        public string Contents => @"#Wait until 10 seconds after the round started.
WAITUNTIL ROUNDSECONDS > 10

#End the script immediately if there are more than 5 players alive.
STOPIF PLAYERSALIVE > 5

#Wait anywhere from 100 to 500 seconds (CHANCE always generates a random value from 0-1).
WAITSEC 100 + (400 * CHANCE)

# Lights off for anywhere from 10 to 20 seconds, plus 1 second for each player.
LIGHTSOFF (1 * PLAYERS) + 10 + (10 * CHANCE)
";
    }
}
