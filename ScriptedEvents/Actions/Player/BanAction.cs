namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class BanAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "BAN";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Bans specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "Players to ban.", true),
            new Argument("duration", typeof(int), "Ban duration (in seconds).", true),
            new Argument("reason", typeof(string), "Ban reason.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[0];
            int duration = (int)Arguments[1];

            foreach (Player player in players)
            {
                player.Ban(duration, VariableSystemV2.ReplaceVariables(Arguments.JoinMessage(2), script).Replace("\\n", "\n"));
            }

            return new(true);
        }
    }
}