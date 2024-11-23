using System;
using System.Linq;
using Exiled.API.Features;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;
using ScriptedEvents.Variables.Interfaces;

namespace ScriptedEvents.Actions.PlayerVariableManipulationActions
{
    public class IndexPlayerAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "IndexPlr";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.PlayerVariableManipualtion;

        /// <inheritdoc/>
        public string Description => "Gets a single player, where the index specified is the place in the list where player inhabits. Index 1 = First player in var, Index 2 = Second player in var etc..";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to index.", true),
            new Argument("type", typeof(int), "The index from the player should be extracted.", true, ArgFlag.BiggerThan0),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var players = ((IPlayerVariable)Arguments[0]!).GetPlayers().ToArray();
            int index = (int)Arguments[1]!;

            if (index <= players.Length) 
                return new(true, new(players.ElementAt(index)));
            
            var err = new ErrorInfo(
                "Index too large",
                $"Provided index {index} is bigger than the amount of players in the list ({players.Length}).",
                Name).ToTrace();
            return new(false, null, err);

        }
    }
}