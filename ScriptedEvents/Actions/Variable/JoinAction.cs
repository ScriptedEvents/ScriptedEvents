using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions.VariableMimicingActions.PlayerVariableManipulation
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    public class JoinAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "JOIN";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.PlayerVariableManipualtion;

        /// <inheritdoc/>
        public string Description => "Returns a player value where players from the first and second reference are joined together.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players1", typeof(PlayerCollection), "First player reference.", true),
            new Argument("players2", typeof(PlayerCollection), "Second player reference.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var players = ((PlayerCollection)Arguments[0]).GetInnerList();
            foreach (var plr in (PlayerCollection)Arguments[1])
            {
                if (!players.Contains(plr))
                {
                    players.Add(plr);
                }
            }

            return new(true, variablesToRet: new object[] { players.ToArray() });
        }
    }
}