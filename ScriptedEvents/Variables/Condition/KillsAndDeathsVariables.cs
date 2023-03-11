namespace ScriptedEvents.Variables.Condition.KillsAndDeaths
{
#pragma warning disable SA1402 // File may only contain a single type
    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;
    using System;

    public class KillsAndDeathsVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Kills & Deaths";

        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Kills(),
            new ScpKills(),
        };
    }

    public class Kills : IFloatVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{KILLS}";

        /// <inheritdoc/>
        public string Description => "The total amount of kills, the amount of kills per-role, or -1 if an invalid role type is provided.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("role", typeof(RoleTypeId), "The role or team to filter by. Optional.", false),
        };

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                if (Arguments.Length < 1)
                {
                    return MainPlugin.Handlers.Kills.Count;
                }
                else
                {
                    if (Enum.TryParse(Arguments[0], true, out RoleTypeId rt))
                    {
                        if (MainPlugin.Handlers.Kills.TryGetValue(rt, out int amt))
                            return amt;
                        else
                            return 0;
                    }
                    else if (Enum.TryParse(Arguments[0], true, out Team team))
                    {
                        int total = 0;
                        foreach (var kills in MainPlugin.Handlers.Kills)
                        {
                            if (kills.Key.GetTeam() == team)
                                total += kills.Value;
                        }

                        return total;
                    }

                    return -1f;
                }
            }
        }
    }

    public class ScpKills : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{SCPKILLS}";

        /// <inheritdoc/>
        public string Description => "The total amount of SCP-related kills.";

        /// <inheritdoc/>
        public float Value => Round.KillsByScp;
    }
}
