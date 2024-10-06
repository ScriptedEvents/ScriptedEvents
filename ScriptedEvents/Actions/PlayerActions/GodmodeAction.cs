namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class GodmodeAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "GODMODE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Grants or removes godmode to specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "Players to change godmode state for.", true),
            new Argument("mode", typeof(bool), "Godmode state to grant.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[0];
            bool mode = (bool)Arguments[1];

            foreach (Player player in players)
            {
                player.IsGodModeEnabled = mode;
            }

            return new(true);
        }
    }
}