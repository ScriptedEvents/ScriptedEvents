namespace ScriptedEvents.Variables.Player.Zones
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class ZoneVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Zones";

        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Player;

        /// <inheritdoc/>
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
        public string Description => "Gets all the alive players in Light Containment Zone.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.Zone.HasFlag(ZoneType.LightContainment));
    }

    public class HCZ : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{HCZ}";

        /// <inheritdoc/>
        public string Description => "Gets all the alive players in Heavy Containment Zone.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.Zone.HasFlag(ZoneType.HeavyContainment));
    }

    public class EZ : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{EZ}";

        /// <inheritdoc/>
        public string Description => "Gets all the alive players in Entrance Zone.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.Zone.HasFlag(ZoneType.Entrance));
    }

    public class Surface : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{SURFACE}";

        /// <inheritdoc/>
        public string Description => "Gets all the alive players on the Surface.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.Zone is ZoneType.Surface);
    }

    public class Pocket : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{POCKET}";

        /// <inheritdoc/>
        public string Description => "Gets all the alive players in the Pocket Dimension.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.CurrentRoom?.Type is RoomType.Pocket);
    }
}
