namespace ScriptedEvents.Variables.Condition.Roles
{
    using System.Linq;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class RoleVariables : IVariableGroup
    {
        public VariableGroupType GroupType => VariableGroupType.Condition;

        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Guards(),
            new MtfAndGuards(),
            new Scps(),
            new Mtf(),
            new Chaos(),
            new SerpentsHand(),
            new UIU(),
        };
    }

    public class Guards : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{GUARDS}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => Player.Get(RoleTypeId.FacilityGuard).Count();
    }

    public class MtfAndGuards : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{MTFANDGUARDS}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => Player.Get(Team.FoundationForces).Count();
    }

    public class Scps : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{SCPS}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => Player.Get(Team.SCPs).Count();
    }

    public class Mtf : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{MTF}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => Player.Get(ply => ply.Role.Team is Team.FoundationForces && ply.Role.Type is not RoleTypeId.FacilityGuard).Count();
    }

    public class Chaos : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CI}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => Player.Get(Team.ChaosInsurgency).Count();
    }

    public class SerpentsHand : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{SH}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => Player.Get(player => player.SessionVariables.ContainsKey("IsSH")).Count();
    }

    public class UIU : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{UIU}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => Player.Get(player => player.SessionVariables.ContainsKey("IsUIU")).Count();
    }
}
