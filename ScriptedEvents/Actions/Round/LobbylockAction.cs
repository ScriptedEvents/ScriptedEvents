namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class LobbylockAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "LOBBYLOCK";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Round;

        /// <inheritdoc/>
        public string Description => "Set server's lobbylock.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(bool), "Whether or not to lock the lobby.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Round.IsLobbyLocked = (bool)Arguments[0];
            return new(true);
        }
    }
}