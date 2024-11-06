namespace ScriptedEvents.DemoScripts
{
    /// <summary>
    /// Demo script providing information to the server host.
    /// </summary>
    public class JoinBroadcast : IDemoScript
    {
        /// <inheritdoc/>
        public string FileName => "JoinBroadcast";

        /// <inheritdoc/>
        public string Contents => @"!-- EVENT Verified
##
This script triggers for every 'Verified' event that happens. 
Why not 'Joined' event? Because 'Verified' is basically the same, but has a valid player reference, where 'Joined' doesn't.

All player events can be found here:
https://github.com/ExMod-Team/EXILED/tree/master/EXILED/Exiled.Events/Handlers/Player.cs
##


BROADCAST {EVPLAYER} 10 <b>Welcome, {GET:EVPLAYER:NAME}!\n<size=20>This server is running <b><color=#ffd983>Scripted</color><color=#8899a6>Events</color>, have a nice stay!</size></b>
##
BROADCAST -> an action that sends a broadcast to specified players
{EVPLAYER} -> this specifies the players to send the broadcasts to, in this case, the player who joined the server
5 -> the duration of the broadcast (in seconds)
and the rest is the message
##
";
    }
}
