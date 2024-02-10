namespace ScriptedEvents.API.Constants
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Exiled.API.Enums;

    using InventorySystem.Items.Usables.Scp330;
    using PlayerRoles;
    using Respawning;
    using ScriptedEvents.Structures;

    public static class EnumDefinitions
    {
        public static ReadOnlyCollection<EnumDefinition> Definitions { get; } = new List<EnumDefinition>()
        {
            new EnumDefinition<RoleTypeId>("The list of valid in-game roles."),
            new EnumDefinition<Team>("The list of valid in-game teams."),
            new EnumDefinition<ItemType>("The list of valid in-game items."),
            new EnumDefinition<CandyKindID>("The list of valid in-game SCP-330 candies."),
            new EnumDefinition<ZoneType>("The list of valid in-game zones."),
            new EnumDefinition<RoomType>("The list of valid in-game rooms."),
            new EnumDefinition<DamageType>("The list of valid in-game damage types."),
            new EnumDefinition<DoorType>("The list of valid in-game door types."),
            new EnumDefinition<ElevatorType>("The list of valid in-game elevator types."),
            new EnumDefinition<SpawnLocationType>("The list of valid in-game spawn location types."),
            new EnumDefinition<SpawnableTeamType>("The list of valid in-game spawnable team types."),
            new EnumDefinition<EffectType>("The list of valid in-game effects."),
        }.AsReadOnly();
    }
}
