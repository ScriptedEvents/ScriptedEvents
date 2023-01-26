using ScriptedEvents;
namespace ScriptedEvents.Handlers
{
    public class DemoScript
    {
        public static readonly string Demo = @$"# This is an example of a script that you can create to automate random events and/or admin events in your server.
# All scripts in this folder can be loaded by running ""executescript/es [filename]"" in-game (without the .txt extension).
# Example: To run this file, type ""executescript/es DemoScript"" in the RemoteAdmin panel.
# 'es.execute' permission is required to execute scripts.
# Any line starting with a # will be ignored.

# Wait until the round starts (only action here is ROUNDSTART, more soon).
WAITUNTIL ROUNDSTART

# Wait 5 seconds.
WAITSEC 5

# Open and lock all doors for 10 seconds
OPENDOOR * 10
LOCKDOOR * 10

# CASSIE Announcement (note that there is no delay here between this instruction and the last two)
CASSIE All doors have been locked opened for 10 seconds

# CASSIE Announcement (cassie can support subtitles, if you add the | seperator to seperate from announcement and subtitle).
CASSIE MtfUnit Epsilon 11 Designated Alpha 1 HasEntered|Mobile Task Force Unit Epsilon 11 Designated Alpha-01 has entered the facility.

# Wait 10 seconds.
WAITSEC 10

# Lights off for anywhere from 5-10 seconds (random).
LIGHTSOFF 5-10
";
    }
}
