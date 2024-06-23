namespace ScriptedEvents.Tutorials.Actions
{
    public class RandomHCZBlackout : ITutorial
    {
        public string FileName => "RandomHCZBlackout";

        public string TutorialName => "Random Heavy Containment Room Blackout (feat. Scp079)";

        public string Author => "Elektryk_Andrzej";

        public string Category => "Scripts";

        public string Contents => @"##
SCRIPT PURPOSE: 
Flicker lights in random rooms in HeavyContainment if an Scp079 is alive

INSTALATION: 
Move this script to your 'Scripts' folder and restart the round

DIFFICULTY:
3/10

RATED FOR VERSION:  
v3.0.0

ORIGINAL CREATOR:
@thunder300 <@189495219383697409>
##

# start this script as soon as the server starts
!-- AUTORUN

# wait until the round has started
WAIT UNTIL {ROUND:STARTED} = TRUE

Loop:
    # wait some time to not give players a seizure
    WAIT SEC 5

    # wait until there is at least 1 Scp079
    # we dont want it to turn off lights if there isnt an Scp079 alive
    WAIT UNTIL {SCP079} >= 1

    # turn off lights in a random room for 2 seconds
    LIGHTSOFF {RANDOMROOM:HeavyContainment} 2

# continue executing from the Loop label and repeat the process
GOTO Loop";
    }
}
