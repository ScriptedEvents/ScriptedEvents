namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class LimitAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "LIMIT";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.PlayerVariableManipualtion;

        /// <inheritdoc/>
        public string Description => "Returns a copy of the provided players in which the number of players is limited by the amount argument.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("players", typeof(PlayerCollection), "The players use.", true),
             new Argument("amount", typeof(int), "The amount of players to limit.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            int max = (int)Arguments[1];
            List<Player> list = ((PlayerCollection)Arguments[0]).GetInnerList();

            while (list.Count > max && list.Count > 0)
            {
                list.PullRandomItem();
            }

            return new(true, variablesToRet: new[] { list.ToArray() });
        }
    }
}