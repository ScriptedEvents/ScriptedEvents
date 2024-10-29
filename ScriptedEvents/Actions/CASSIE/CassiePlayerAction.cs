using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class CassiePlayerAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "CASSIEPLAYER";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "CASSIEPLR" };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Cassie;

        /// <inheritdoc/>
        public string Description => "Makes a cassie announcement for specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("Silent", "Makes a silent announcement."),
                new("Loud", "Makes a loud announcement.")),
            new Argument("players", typeof(PlayerCollection), "The players to play the CASSIE announcement for.", true),
            new Argument("message", typeof(string), "The message for cassie.", true),
            new Argument("caption", typeof(string), "An optional caption for the announcement. Use NONE for no captions.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[1];
            bool isNoisy = Arguments[0].ToUpper() == "LOUD";
            string message = Arguments[2].ToString();

            if (Arguments.Length > 3)
            {
                foreach (var player in players)
                {
                    player.MessageTranslated(message, Arguments[3].ToString().ToUpper() == "NONE" ? string.Empty : Arguments[3].ToString(), makeNoise: isNoisy);
                }

                return new(true);
            }

            foreach (var player in players)
            {
                player.PlayCassieAnnouncement(message, makeNoise: isNoisy);
            }

            return new(true);
        }
    }
}
