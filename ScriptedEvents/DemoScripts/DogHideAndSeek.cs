namespace ScriptedEvents.DemoScripts
{
    public class DogHideAndSeek : IDemoScript
    {
        /// <inheritdoc/>
        public string FileName => "DogHideAndSeek";

        /// <inheritdoc/>
        public string Contents => @"##
Name: Dog Hide and Seek
One SCP-939, everyone else is a Class-D with a flashlight.

Setup spawn rules (One SCP-939, everyone else Class-D)
This part must run BEFORE the round starts.
##
SPAWNRULE Scp939 1
SPAWNRULE ClassD

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

##
Northwood does not allow us to permanently have the lights off.
So we'll just have them off for a very very long time instead.
* Indicates lights off in EVERY room.
##
LIGHTSOFF * 9999

# Give every Class-D a Flashlight.
ITEM ADD {CLASSD} Flashlight

# Open and lock every door
# * Indicates ALL doors open and lock.
DOOR OPEN *
DOOR LOCK *

# 65-second countdown
# * Indicates to show the countdown to EVERY player.
COUNTDOWN * 65 Seeker released in...

# Optionally: Close the surface blast doors, so players cannot hide on the surface.
# This does NOT detonate the warhead - only closes the blast doors.
WARHEAD BLASTDOORS

# Teleport SCP-939 to Gate A and close the gate temporarily, giving players a chance to hide.
DOOR CLOSE GateA
WAIT SEC 2
TPROOM {SCP939} EzGateA

# After 65 seconds, release the beast
WAIT SEC 65
DOOR OPEN GateA";
    }
}