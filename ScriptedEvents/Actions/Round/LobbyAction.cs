using System;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Round
{
    public class LobbyAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "LOBBY";

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
            new OptionsArgument("mode", true,
                new("LOCK"),
                new("UNLOCK")),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            switch ((string)Arguments[0])
            {
                case "LOCK":
                    Exiled.API.Features.Round.IsLobbyLocked = true; break;
                case "UNLOCK":
                    Exiled.API.Features.Round.IsLobbyLocked = false; break;
            }

            return new(true);
        }
    }
}