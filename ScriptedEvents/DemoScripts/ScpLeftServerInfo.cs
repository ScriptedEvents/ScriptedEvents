namespace ScriptedEvents.DemoScripts
{
    public class ScpLeftServerInfo : IDemoScript
    {
        /// <inheritdoc/>
        public string FileName => "ScpLeftInfo";

        /// <inheritdoc/>
        public string Contents => @"!-- EVENT Left
##
""Left"" and ""ChangingRole"" events get following additional variables:
- {EVLASTNAME}
- {EVLASTUSERID}
- {EVLASTROLE}
- {EVLASTTEAM}
- {EVLASTZONE}
- {EVLASTROOM}

This is because the normal {EVPLAYER} will be outdates as the script is triggered.
Since evplayer left the server, we cannot use the {GET} variable with {EVPLAYER}.
{GET} needs the player to be on the server, which is not the case with this event.

Thanks to the variables above, we're able to still retreive some info about the player while not having an active reference.
##

# stop if the player was not an SCP
STOP $IF {EVLASTTEAM} != SCPs

# save the message
LOCAL {MSG} Player ""{EVLASTNAME}"" ({EVLASTUSERID}) playing as ""{EVLASTROLE}"" role has left the server.

# use the message variable to send info to the server staff and the server console 
PRINT {MSG} 
BROADCAST {SERVERSTAFF} 10 <color=red>{MSG}</color>\nConsider getting a different player to replace the SCP.";
    }
}