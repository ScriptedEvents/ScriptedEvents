namespace ScriptedEvents.Variables.Strings
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;
    using System.Linq;

    using Exiled.API.Features;

    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class PlayerInfoVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Player info";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new DisplayVariable(),
        };
    }

    public class DisplayVariable : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{DISPLAY}";

        /// <inheritdoc/>
        public string Description => "Displays players in a player variable.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to display.", true),
            new OptionsArgument("mode", false,
                new("NAME", "Display players' names. The default option"),
                new("DPNAME", "Display players' display names.")),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                PlayerCollection players = (PlayerCollection)Arguments[0];
                if (players.Length == 0)
                {
                    return "NONE";
                }

                Func<Player, string> action = (Arguments.Length > 1 ? Arguments[1].ToUpper() : "NAME") switch
                {
                    "NAME" => p => { return p.Nickname; },
                    "DPNAME" => p => { return p.DisplayNickname; },
                    _ => throw new ArgumentException(),
                };
                return string.Join(", ", players.Select(action));
            }
        }

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }
    }
}
