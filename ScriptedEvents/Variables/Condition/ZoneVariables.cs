namespace ScriptedEvents.Variables.Condition.Zone
{
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class ZoneVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

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

    public class LCZ : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{LCZ}";

        /// <inheritdoc/>
        public string Description => "The total amount of players in Light Containment Zone.";

        /// <inheritdoc/>
        public float Value => Player.List.Count(ply => ply.Zone is ZoneType.LightContainment);
    }

    public class HCZ : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{HCZ}";

        /// <inheritdoc/>
        public string Description => "The total amount of players in Heavy Containment Zone.";

        /// <inheritdoc/>
        public float Value => Player.List.Count(ply => ply.Zone is ZoneType.HeavyContainment);
    }

    public class EZ : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{EZ}";

        /// <inheritdoc/>
        public string Description => "The total amount of players in Entrance Zone.";

        /// <inheritdoc/>
        public float Value => Player.List.Count(ply => ply.Zone is ZoneType.Entrance);
    }

    public class Surface : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{SURFACE}";

        /// <inheritdoc/>
        public string Description => "The total amount of players on the Surface.";

        /// <inheritdoc/>
        public float Value => Player.List.Count(ply => ply.Zone is ZoneType.Surface);
    }

    public class Pocket : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{POCKET}";

        /// <inheritdoc/>
        public string Description => "The total amount of players in the Pocket Dimension.";

        /// <inheritdoc/>
        public float Value => Player.List.Count(ply => ply.CurrentRoom?.Type is RoomType.Pocket);
    }
}
