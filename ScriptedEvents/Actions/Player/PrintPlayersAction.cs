using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Player
{
    public class PrintPLayersAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "PRINTPLAYER";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "PRINTPLR" };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Prints a message in the specified players' console.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "Players to affect.", true),
            new Argument("message", typeof(string), "The message content.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[0];
            string message = Arguments.JoinMessage(1).Replace("\\n", "\n");

            foreach (Exiled.API.Features.Player player in players)
            {
                player.SendConsoleMessage(message, "green");
            }

            return new(true);
        }
    }
}