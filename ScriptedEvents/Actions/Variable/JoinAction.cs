using System;
using System.Linq;
using Exiled.API.Features;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Variable
{
    public class JoinAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "Join";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.PlayerVariableManipualtion;

        /// <inheritdoc/>
        public string Description => "Returns a player value where players from the first and second reference are joined together.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("firstPlayers", typeof(Player[]), "First player reference.", true),
            new Argument("secondPlayers", typeof(Player[]), "Second player reference.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var players = ((Player[])Arguments[0]!).ToList();
            foreach (var plr in (Player[])Arguments[1]!)
            {
                if (!players.Contains(plr))
                {
                    players.Add(plr);
                }
            }

            return new(true, new(players));
        }
    }
}