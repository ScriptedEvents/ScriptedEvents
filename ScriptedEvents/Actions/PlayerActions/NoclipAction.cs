using System;
using Exiled.API.Features;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.PlayerActions
{
    public class NoclipAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "NoClip";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Grants or removes noclip to specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "Players to change noclip state.", true),
            new Argument("mode", typeof(bool), "Noclip mode.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Player[] players = (Player[])Arguments[0]!;
            bool mode = (bool)Arguments[1]!;

            foreach (Player player in players)
            {
                player.IsNoclipPermitted = mode;
            }

            return new(true);
        }
    }
}