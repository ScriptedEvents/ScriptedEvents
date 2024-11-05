namespace ScriptedEvents.DemoScripts
{
    public class HitMarker : IDemoScript
    {
        /// <inheritdoc/>
        public string FileName => "HitMarker";

        /// <inheritdoc/>
        public string Contents => @"!-- EVENT Died

HINT {EVATTACKER} 5 <size=28>[ Subject <color=red>{GET:EVPLAYER:DPNAME}</color> terminated ]</size>

##
{EVPLAYER} is the user who the event is happening towards
The opposite would be {EVATTACKER}, who caused said event
{GET} collects a user's data in a Player Variable
!-- EVENT uses an EXILED event to fire, which can be viewed below:
https://github.com/Exiled-Team/EXILED/tree/master/Exiled.Events/EventArgs

Remember to use [shelp list/listvar] in your server console for any help,
Or don't hesitate to reach out to the support team in the SE Discord!
##";
    }
}