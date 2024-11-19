using System;
using System.Collections.Generic;
using Exiled.API.Features;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.PlayerVariableManipulationActions
{
    public class MaxAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "Max";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.PlayerVariableManipualtion;

        /// <inheritdoc/>
        public string Description => "Returns a copy of the provided player variable in which the number of players is capped by the 'max' argument.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("players", typeof(PlayerCollection), "The players to cap.", true),
             new Argument("max", typeof(int), "The maximum amount of players allowed.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            int max = (int)Arguments[1]!;
            var list = ((PlayerCollection)Arguments[0]!).GetInnerList();

            for (int i = 0; i <= max; i++)
            {
                if (list.Count == 0)
                    break;

                list.PullRandomItem();
            }

            return new(true, new(list));
        }
    }
}