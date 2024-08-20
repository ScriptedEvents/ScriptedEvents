namespace ScriptedEvents.DemoScripts
{
    /// <summary>
    /// Demo script showing multiple different actions.
    /// </summary>
    public class DemoScript : IDemoScript
    {
        /// <inheritdoc/>
        public string FileName => "DemoScript";

        /// <inheritdoc/>
        public string Contents => @"!-- DISABLE
# This is an example of a script that you can create to automate random events and/or admin events in your server.

# Wait until the round starts.
WAIT UNTIL {ROUND:STARTED}

# Wait 5 seconds.
WAIT SEC 5

# Open and lock all doors
DOOR OPEN *
DOOR LOCK *

# CASSIE Announcement (note that there is no delay here between this instruction and the last two)
CASSIE LOUD All doors have been locked opened

# Wait for announcement to finish before making new one
WAIT UNTIL {!CASSIESPEAKING}

# CASSIE Announcement (cassie can support subtitles, if you add the | seperator to seperate from announcement and subtitle).
CASSIE LOUD MtfUnit Epsilon 11 Designated Alpha 1 HasEntered|Mobile Task Force Unit Epsilon 11 Designated Alpha-01 has entered the facility.

# Wait 10 seconds.
WAIT SEC 10

# Destroy all doors
DOOR DESTROY *

# Lights off for 10 seconds.
LIGHTSOFF * 10

# Run a command. It needs / before it if it's a RA command, or . before it if its a console command.
COMMAND /cleanup ragdolls
";
    }
}
