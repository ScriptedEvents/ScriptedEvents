namespace ScriptedEvents.API.Constants
{
    public static class ConstMessages
    {
        public const string RoomInput = @"The following inputs can be used to target rooms:
- '*'/'ALL' - Targets ALL rooms
- 'LightContainment' - Targets light containment rooms
- 'HeavyContainment' - Targets heavy containment rooms
- 'Entrance' - Targets entrance rooms
- The ID of a room. All valid room IDs can be found on Exiled's documentation at: https://exiled-team.github.io/EXILED/api/Exiled.API.Enums.RoomType.html";

        public const string GotoInput = @"The following keywords can be used in place of a label:
- START - Moves to the start of the script.
- STOP - Immediately stops the script.
- NEXT - Moves to the next line.";

        public const string DisableKeys = @"The following keys contain functionality when used in DISABLE/ENABLE and DISABLEPLAYER/ENABLEPLAYER.

- DOORS - Disables all door interactions.
- DROPPING - Disables dropping and throwing items.
- DYING - Prevents player(s) from dying.
- ELEVATORS - Disables all elevator functionality.
- ESCAPING - Prevents player(s) from escaping.
- GENERATORS - Prevents player(s) from interacting with generators.
- HAZARDS - Disables all hazard functionality.
- HURTING - Prevents player(s) from taking damage.
- ITEMPICKUPS - Disables all item pickup functionality.
- LOCKERS - Disables all locker functionality.
- MICROPICKUPS - Disables item pickup functionality only for Micro-HID drops.
- PEDESTALS - Disables all pedestal functionality.
- SCP049ATTACK - Disables SCP-049's primary attack.
- SCP0492ATTACK - Disables SCP-049-2's primary attack.
- SCP096ATTACK - Disables SCP-096's primary attack.
- SCP106ATTACK - Disables SCP-106's primary attack.
- SCP173ATTACKv - Disables SCP-173's primary attack.
- SCP330 - Disables all SCP-330 functionality.
- SCP914 - Disables all SCP-914 functionality.
- SCP939ATTACK - Disables SCP-939's primary attack.
- SCP3114ATTACK - Disables SCP-3114's primary attack.
- SCPALLABILITIES - Disables all SCP abilities.
- SCPATTACK - Disables all SCP attacks.
- SHOOTING - Disables all shooting functionality.
- TESLAS - Disables all tesla functionality.
- WARHEAD - Prevents player(s) from interacting with the warhead.
- WORKSTATIONS - Disables all workstation functionality.

The following keys can ONLY be used in DISABLE and ENABLE. They cannot be tied to specific players.
- NTFANNOUNCEMENT * - Disables NTF spawn announcements.
- RESPAWNS - Prevents player(s) from respawning as Chaos/NTF.
- SCPANNOUNCEMENT - Disables SCP termination announcements.
";
    }
}
