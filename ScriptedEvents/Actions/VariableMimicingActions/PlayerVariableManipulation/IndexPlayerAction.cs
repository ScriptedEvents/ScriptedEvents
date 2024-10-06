namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    /// <inheritdoc/>
    public class IndexPlayerAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "INDEXPLR";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.PlayerVariableManipualtion;

        /// <inheritdoc/>
        public string Description => "Returns a player inhabiting the specified index in the player variable.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("playervar", typeof(IPlayerVariable), "The name of the variable to index.", true),
            new Argument("type", typeof(int), "The index from the player should be extracted.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            IEnumerable<Player> players = ((IPlayerVariable)Arguments[0]).Players;
            int index = (int)Arguments[1];

            if (index > players.Count() - 1)
                throw new IndexOutOfRangeException(ErrorGen.Get(ErrorCode.IndexTooLarge, index));

            return new(true, variablesToRet: new[] { new[] { players.ElementAt(index) } });
        }
    }
}