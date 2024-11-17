using System;
using Exiled.API.Features;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.PlayerActions
{
    public class MuteAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Mute";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Mutes specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("Set", "Mute player(s)."),
                new Option("Remove", "Unmute player(s).")),
            new Argument("players", typeof(PlayerCollection), "Players to change mute status for.", true),
            new Argument("longterm", typeof(bool), "If TRUE, player will be muted even after rejoining.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[1]!;
            bool longTerm = (bool)Arguments[2]!;

            Action<Player> playerAction = Arguments[0]!.ToUpper() switch
            {
                "SET" => (plr) =>
                {
                    if (longTerm)
                        plr.Mute();
                    else
                        plr.IsMuted = true;
                },
                "REMOVE" => (plr) =>
                {
                    if (longTerm)
                        plr.UnMute();
                    else
                        plr.IsMuted = false;
                },
                _ => throw new ImpossibleException()
            };

            foreach (Player player in players)
            {
                playerAction(player);
            }

            return new(true);
        }
    }
}