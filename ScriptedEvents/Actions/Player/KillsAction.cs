using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;

    using PlayerRoles;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class KillsAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "KILLS";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Returns the amount of kills, the amount of kills per-role or per-team.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("role", typeof(RoleTypeIdOrTeam), "The role or team to filter by. Optional.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1)
                return new(true, variablesToRet: new[] { MainPlugin.Handlers.Kills.Count.ToString() });

            if (Arguments[0] is RoleTypeId rt)
            {
                if (MainPlugin.Handlers.Kills.TryGetValue(rt, out int amt))
                    return new(true, variablesToRet: new[] { amt.ToString() });
                else
                    return new(true, variablesToRet: new[] { 0.ToString() });
            }
            else if (Arguments[0] is Team team)
            {
                int total = 0;
                foreach (var kills in MainPlugin.Handlers.Kills)
                {
                    if (kills.Key.GetTeam() == team)
                        total += kills.Value;
                }

                return new(true, variablesToRet: new[] { total.ToString() });
            }

            return new(false, "ARGPROC failed. Report this error to SE developers as soon as possible!");
        }
    }
}