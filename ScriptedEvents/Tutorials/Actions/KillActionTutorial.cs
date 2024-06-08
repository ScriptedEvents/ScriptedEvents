namespace ScriptedEvents.Tutorials.Actions
{
    public class KillActionTutorial : ITutorial
    {
        public string FileName => "KillAction";

        public string TutorialName => "The Kill Action";

        public string Author => "Thunder";

        public string Category => "Actions";

        public string Contents => @"As self explanatory as it is, the kill action has several uses.

The first, and most self-explanatory use, is to simply kill players with no message, by simply running the ""KILL"" action with no message and only players.
For example, ""KILL *"" to kill all players with no message (or rather the 'Unknown cause of death' message).

Second, players can be killed using a base-game message. This can be done by using a base-game DamageType. For all valid options, run ""shelp DamageType"" in the server console.
For example, ""KILL * MicroHid"" will kill all players with the death message used by the Micro-HID.

Third, and the most common use, players can be killed using a custom message.
For example, ""KILL * Jumped too much"" will kill all players with a custom message.

Lastly, bodies can simply be vaporized (like being killed by the Particle Disruptor), by using the key 'VAPORIZE'.
For example, ""KILL * VAPORIZE"" will kill all players and vaporize the bodies.";
    }
}
