namespace ScriptedEvents.Variables.Zone
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.Variables.Interfaces;

    public class ZoneVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Zone";

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

    public class LCZ : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{LCZ}";

        /// <inheritdoc/>
        public string Description => "The amount of players in Light Containment Zone.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List.Where(ply => ply.Zone.HasFlag(ZoneType.LightContainment));
    }

    public class HCZ : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{HCZ}";

        /// <inheritdoc/>
        public string Description => "The amount of players in Heavy Containment Zone.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List.Where(ply => ply.Zone.HasFlag(ZoneType.HeavyContainment));
    }

    public class EZ : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{EZ}";

        /// <inheritdoc/>
        public string Description => "The amount of players in Entrance Zone.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List.Where(ply => ply.Zone.HasFlag(ZoneType.Entrance));
    }

    public class Surface : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{SURFACE}";

        /// <inheritdoc/>
        public string Description => "The amount of players on the Surface.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List.Where(ply => ply.Zone.HasFlag(ZoneType.Surface));
    }

    public class Pocket : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{POCKET}";

        /// <inheritdoc/>
        public string Description => "The amount of players in the Pocket Dimension.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List.Where(ply => ply.CurrentRoom?.Type is RoomType.Pocket);
    }
}
