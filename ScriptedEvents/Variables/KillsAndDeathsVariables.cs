namespace ScriptedEvents.Variables.KillsAndDeaths
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;

    using Exiled.API.Features;

    using PlayerRoles;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class KillsAndDeathsVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Kills & Deaths";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Kills(),
            new ScpKills(),
        };
    }

    public class Kills : IFloatVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{KILLS}";

        /// <inheritdoc/>
        public string Description => "The amount of kills, the amount of kills per-role, or -1 if an invalid role type is provided.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("role", typeof(RoleTypeIdOrTeam), "The role or team to filter by. Optional.", false),
        };

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                if (Arguments.Length < 1)
                    return MainPlugin.Handlers.Kills.Count;

                if (Arguments[0] is RoleTypeId rt)
                {
                    if (MainPlugin.Handlers.Kills.TryGetValue(rt, out int amt))
                        return amt;
                    else
                        return 0;
                }
                else if (Arguments[0] is Team team)
                {
                    int total = 0;
                    foreach (var kills in MainPlugin.Handlers.Kills)
                    {
                        if (kills.Key.GetTeam() == team)
                            total += kills.Value;
                    }

                    return total;
                }

                throw new ArgumentException(ErrorGen.Get(ErrorCode.UnknownError));
            }
        }
    }

    public class ScpKills : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{SCPKILLS}";

        /// <inheritdoc/>
        public string Description => "The amount of SCP-related kills.";

        /// <inheritdoc/>
        public float Value => Round.KillsByScp;
    }
}
