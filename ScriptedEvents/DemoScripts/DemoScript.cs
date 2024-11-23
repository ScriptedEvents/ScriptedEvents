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
        public string Contents => @"#!-- AutoRun
# This is an example of a script that you can create to automate random events and/or admin events in your server.
# Remove the '#' from before the '!-- AutoRun' in order for it to work.

# Wait until the round starts.
WaitUntil {RoundInfo:HasStarted}

# Wait 5 seconds.
Wait 5s

# Open and lock all doors
Door Open *
Door Lock *

# CASSIE Announcement (note that there is no delay here between this instruction and the last two)
Cassie Loud All doors have been locked opened

# Wait for announcement to finish before making new one
WaitUntil {MapInfo:IsCassieSpeaking} = false

# CASSIE Announcement (cassie can support subtitles, if you add the | seperator to seperate from announcement and subtitle).
Cassie Loud MtfUnit Epsilon 11 Designated Alpha 1 HasEntered|Mobile Task Force Unit Epsilon 11 Designated Alpha-01 has entered the facility.

# Wait 10 seconds.
Wait 10s

# Destroy all doors
Door Destory *

# Lights off for 10 seconds.
LightsOff * 10s

# Run a command. It needs / before it if it's a RA command, or . before it if its a console command.
Command /cleanup ragdolls
";
    }
}
