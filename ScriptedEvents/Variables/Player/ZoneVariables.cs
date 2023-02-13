namespace ScriptedEvents.Variables.Player.Zones
{
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class ZoneVariables : IVariableGroup
    {
        public VariableGroupType GroupType => VariableGroupType.Player;

        public IVariable[] Variables { get; } = new IVariable[]
        {
            new LCZ(),
            new HCZ(),
            new EZ(),
            new Surface(),
            new Pocket(),
        };
    }

    public class LCZ : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{LCZ}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.Zone is ZoneType.LightContainment);
    }

    public class HCZ : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{HCZ}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.Zone is ZoneType.HeavyContainment);
    }

    public class EZ : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{EZ}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.Zone is ZoneType.Entrance);
    }

    public class Surface : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{SURFACE}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.Zone is ZoneType.Surface);
    }

    public class Pocket : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{POCKET}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.CurrentRoom?.Type is RoomType.Pocket);
    }
}
