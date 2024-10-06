namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class MuteAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "MUTE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Mutes specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("SET", "Mute player(s)."),
                new("REMOVE", "Unmute player(s).")),
            new Argument("players", typeof(PlayerCollection), "Players to change mute status for.", true),
            new Argument("longterm", typeof(bool), "If TRUE, player will be muted even after rejoining.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[1];
            bool longTerm = (bool)Arguments[2];

            Action<Player> playerAction;

            switch (Arguments[0].ToUpper())
            {
                case "SET":
                    playerAction = (plr) => { if (longTerm) plr.Mute(); else plr.IsMuted = true; };
                    break;

                case "REMOVE":
                    playerAction = (plr) => { if (longTerm) plr.UnMute(); else plr.IsMuted = false; };
                    break;

                default:
                    return new(false, $"Invalid mode '{Arguments[0].ToUpper()}' provided.");
            }

            foreach (Player player in players)
            {
                playerAction(player);
            }

            return new(true);
        }
    }
}