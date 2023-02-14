namespace ScriptedEvents.Variables.Player.Roles
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class RoleVariables : IVariableGroup
    {
        public VariableGroupType GroupType => VariableGroupType.Player;

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

    public class Guards : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{GUARDS}";

        /// <inheritdoc/>
        public string Description => "Gets all the alive facility guards. Equivalent to {FACILITYGUARD}.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(RoleTypeId.FacilityGuard);
    }

    public class MtfAndGuards : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{MTFANDGUARDS}";

        /// <inheritdoc/>
        public string Description => "Gets all the alive facility guards & MTF.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(Team.FoundationForces);
    }

    public class Scps : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{SCPS}";

        /// <inheritdoc/>
        public string Description => "Gets all the alive SCPs.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(Team.SCPs);
    }

    public class Mtf : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{MTF}";

        /// <inheritdoc/>
        public string Description => "Gets all the alive MTF.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(ply => ply.Role.Team is Team.FoundationForces && ply.Role.Type is not RoleTypeId.FacilityGuard);
    }

    public class Chaos : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{CI}";

        /// <inheritdoc/>
        public string Description => "Gets all the alive Chaos Insurgency.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(Team.ChaosInsurgency);
    }

    public class SerpentsHand : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{SH}";

        /// <inheritdoc/>
        public string Description => "Gets all the alive Serpent's Hand (always none if the plugin is not installed).";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.SessionVariables.ContainsKey("IsSH"));
    }

    public class UIU : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{UIU}";

        /// <inheritdoc/>
        public string Description => "Gets all the alive UIU (always none if the plugin is not installed).";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.SessionVariables.ContainsKey("IsUIU"));
    }

    public class RoleTypeVariable : IPlayerVariable
    {
        public RoleTypeVariable()
        {
            RoleType = RoleTypeId.None;
        }

        public RoleTypeVariable(RoleTypeId rt)
        {
            RoleType = rt;
        }

        public RoleTypeId RoleType { get; }

        /// <inheritdoc/>
        public string Name => $"{{{RoleType.ToString().ToUpper()}}}";

        /// <inheritdoc/>
        public string Description => $"Gets all the alive {RoleType}.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(RoleType);
    }
}
