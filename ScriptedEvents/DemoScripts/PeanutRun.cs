namespace ScriptedEvents.DemoScripts
{
    public class PeanutRun : IDemoScript
    {
        /// <inheritdoc/>
        public string FileName => "PeanutRun";

        /// <inheritdoc/>
        public string Contents => @"##
Event name: Peanut Run
Spawns one player in as peanut and everyone else as Class-D. The Class-Ds must escape.
Any Class-D killed by SCP-173 becomes SCP-173!

Setup spawn rules (One SCP-173, everyone else Class-D)
This part must run BEFORE the round starts.
## 
SPAWNRULE Scp173 1
SPAWNRULE ClassD

# Setup infection rule: Class-D -> Scp173 when a Class-D dies
# TRUE = Move players who die to their death position
INFECTRULE ClassD Scp173 TRUE

# Disable certain features to make running the event more smoothly and fair.
DISABLE DROPPING
DISABLE ITEMPICKUPS
DISABLE SCP330
DISABLE SCP914
DISABLE TESLAS

# Wait for the round to start.
WAIT UNTIL {ROUND:STARTED} = TRUE

# 2 second wait for players to spawn
WAIT SEC 2

# Give every Class-D an O5 keycard.
GIVE {CLASSD} KeycardO5

# Extra fun: Start the warhead (optional)
# If you start the warhead, giving O5 keycards is probably optional. This is just an example, after all.
WARHEAD START
WARHEAD LOCK
";
    }
}