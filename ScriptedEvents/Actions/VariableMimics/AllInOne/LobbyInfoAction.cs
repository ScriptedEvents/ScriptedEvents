namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class LobbyInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "LOBBYINFO";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting lobby related information.";

        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("WAITINGTIME", "Returns the timer for waiting for players in lobby."),
                new("ISACTIVE", "Returns a TRUE/FALSE value saying if the lobby is active."),
                new("ISLOCKED", "Returns a TRUE/FALSE value saying if the lobby is locked.")),
        };

        public string[] RawArguments { get; set; }

        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.AllInOneInfo;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string ret = Arguments[0].ToUpper() switch
            {
                "WAITINGTIME" => Round.LobbyWaitingTime.ToString(),
                "ISACTIVE" => Round.IsLobby.ToUpper(),
                "ISLOCKED" => Round.IsLobbyLocked.ToUpper(),
                _ => throw new ArgumentException()
            };

            return new(true, variablesToRet: new[] { ret });
        }
    }
}