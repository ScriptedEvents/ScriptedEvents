namespace ScriptedEvents.Variables.Roles
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;

    using PlayerRoles;

    using ScriptedEvents.Variables.Interfaces;

    public class RoleVariables : IVariableGroup
    {
        private List<IVariable> variables;

        /// <inheritdoc/>
        public string GroupName => "Roles";

        /// <inheritdoc/>
        public IVariable[] Variables
        {
            get
            {
                if (variables is not null)
                    return variables.ToArray();

                List<IVariable> roleVars = ((RoleTypeId[])Enum.GetValues(typeof(RoleTypeId)))
                        .Where(role => role is not RoleTypeId.None)
                        .Select(role => new RoleTypeVariable(role))
                        .ToList<IVariable>();

                roleVars.AddRange(new List<IVariable>()
                {
                    new Guards(),
                    new MtfAndGuards(),
                    new Scps(),
                    new Mtf(),
                    new Chaos(),
                    new SerpentsHand(),
                    new UIU(),
                });

                variables = roleVars;
                return variables.ToArray();
            }
        }
    }

    public class Guards : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "@GUARDS";

        /// <inheritdoc/>
        public string Description => "Returns players playing as facility guards. Equivalent to @FACILITYGUARDS";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(RoleTypeId.FacilityGuard);
    }

    public class MtfAndGuards : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "@MTFANDGUARDS";

        /// <inheritdoc/>
        public string Description => "Returns players playing as facility guards & MTF.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(Team.FoundationForces);
    }

    public class Scps : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "@SCPS";

        /// <inheritdoc/>
        public string Description => "Returns players playing as SCPs.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(Team.SCPs);
    }

    public class Mtf : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "@MTF";

        /// <inheritdoc/>
        public string Description => "Returns players playing as MTF.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(ply => ply.Role.Team is Team.FoundationForces && ply.Role.Type is not RoleTypeId.FacilityGuard);
    }

    public class Chaos : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "@CI";

        /// <inheritdoc/>
        public string Description => "Returns players playing as Chaos Insurgency.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(Team.ChaosInsurgency);
    }

    public class SerpentsHand : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "@SH";

        /// <inheritdoc/>
        public string Description => "Returns players playing as the Serpent's Hand (always 0 if the plugin is not installed).";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.SessionVariables.ContainsKey("IsSH"));
    }

    public class UIU : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "@UIU";

        /// <inheritdoc/>
        public string Description => "Returns players playing as the UIU squad (always 0 if the plugin is not installed).";

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
        public string Name
        {
            get
            {
                string role = $"{RoleType.ToString().ToUpper()}";

                if (role.EndsWith("TCH"))
                {
                    role += "ES";
                }
                else if (role.EndsWith("MAN"))
                {
                    role = role.Substring(0, role.Length - 3) + "MEN";
                }
                else if (role == "NONE")
                {
                }
                else
                {
                    role += "S";
                }

                return "@" + role;
            }
        }

        /// <inheritdoc/>
        public string Description => $"Returns players playing as the '{RoleType}' role.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(RoleType);
    }
}
