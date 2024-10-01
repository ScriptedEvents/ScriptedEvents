namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class DisplayPlayerAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "DISPLAY";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Returns a message where all provided players are formatted like 'playerName1, playerName2, etc'. Returns 'NONE' if player reference holds 0 players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("players", typeof(PlayerCollection), "The players to display.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[0];
            if (players.Length == 0)
            {
                return new(true, variablesToRet: new[] { "NONE" });
            }

            return new(true, variablesToRet: new[] { string.Join(", ", players.Select(p => p.Nickname)) });
        }
    }
}