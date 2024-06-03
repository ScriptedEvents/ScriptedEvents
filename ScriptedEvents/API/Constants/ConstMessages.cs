namespace ScriptedEvents.API.Constants
{
    using System;
    using System.Linq;

    using Exiled.API.Enums;

    public static class ConstMessages
    {
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
- SCP173ATTACK - Disables SCP-173's primary attack.
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
- CUFFING - Disables cuffing.

The following keys can ONLY be used in DISABLE and ENABLE. They cannot be tied to specific players.
- NTFANNOUNCEMENT * - Disables NTF spawn announcements.
- RESPAWNS - Prevents player(s) from respawning as Chaos/NTF.
- SCPANNOUNCEMENT - Disables SCP termination announcements.
";

        public static readonly string RoomInput = $@"The following inputs can be used to target rooms:
- '*'/'ALL' - Targets ALL rooms
- 'LightContainment' - Targets light containment rooms
- 'HeavyContainment' - Targets heavy containment rooms
- 'Entrance' - Targets entrance rooms

Alternatively, a Room ID can be used. A full list of valid Room IDs (as of {DateTime.Now:g}) follows:
{string.Join("\n", ((RoomType[])Enum.GetValues(typeof(RoomType))).Where(r => r is not RoomType.Unknown).Select(r => $"- [{r:d}] {r}"))}";

        public static readonly string ItemInput = $@" A full list of valid Item IDs (as of {DateTime.Now:g}) follows:
{string.Join("\n", ((ItemType[])Enum.GetValues(typeof(ItemType))).Where(r => r is not ItemType.None).Select(r => $"- [{r:d}] {r}"))}

Alternatively, the ID of a CustomItem can be used.";
    }
}
