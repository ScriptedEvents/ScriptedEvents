namespace ScriptedEvents.Variables.Roles
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class RoleVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Roles";

        /// <inheritdoc/>
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

    public class Guards : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{GUARDS}";

        /// <inheritdoc/>
        public string Description => "The amount of facility guards alive. Equivalent to {FACILITYGUARD}";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(RoleTypeId.FacilityGuard);
    }

    public class MtfAndGuards : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{MTFANDGUARDS}";

        /// <inheritdoc/>
        public string Description => "The amount of facility guards & MTF alive.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(Team.FoundationForces);
    }

    public class Scps : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{SCPS}";

        /// <inheritdoc/>
        public string Description => "The amount of SCPs alive.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(Team.SCPs);
    }

    public class Mtf : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{MTF}";

        /// <inheritdoc/>
        public string Description => "The amount of MTF alive.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(ply => ply.Role.Team is Team.FoundationForces && ply.Role.Type is not RoleTypeId.FacilityGuard);
    }

    public class Chaos : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{CI}";

        /// <inheritdoc/>
        public string Description => "The amount of Chaos Insurgency alive.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(Team.ChaosInsurgency);
    }

    public class SerpentsHand : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{SH}";

        /// <inheritdoc/>
        public string Description => "The amount of Serpent's Hand alive (always 0 if the plugin is not installed).";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.SessionVariables.ContainsKey("IsSH"));
    }

    public class UIU : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{UIU}";

        /// <inheritdoc/>
        public string Description => "The amount of UIU alive (always 0 if the plugin is not installed).";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.SessionVariables.ContainsKey("IsUIU"));
    }

    public class RoleTypeVariable : IFloatVariable, IPlayerVariable
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
        public string Description => $"The amount of {RoleType} alive.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(RoleType);
    }
}
