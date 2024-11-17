using System;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.RoundActions
{
    public class LobbyAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Lobby";

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
                new Option("Lock"),
                new Option("Unlock")),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Exiled.API.Features.Round.IsLobbyLocked = (string)Arguments[0] switch
            {
                "LOCK" => true,
                "UNLOCK" => false,
                _ => throw new ImpossibleException()
            };

            return new(true);
        }
    }
}