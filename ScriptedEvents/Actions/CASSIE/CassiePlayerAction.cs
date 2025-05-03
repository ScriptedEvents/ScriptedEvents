using Exiled.API.Extensions;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.CASSIE
{
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
                new("SILENT", "Makes a silent announcement."),
                new("LOUD", "Makes a loud announcement.")),
            new Argument("players", typeof(PlayerCollection), "The players to play the CASSIE announcement for.", true),
            new Argument("message", typeof(string), "The message. Separate message with a | to specify a caption.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[1];
            bool isNoisy = Arguments[0].ToUpper() == "LOUD";
            string text = RawArguments.JoinMessage(2);

            string[] cassieArgs = text.Split('|');

            if (cassieArgs.Length == 1)
            {
                foreach (Exiled.API.Features.Player ply in players)
                {
                    ply.MessageTranslated(text, text, makeNoise: isNoisy);
                }

                return new(true);
            }

            if (string.IsNullOrWhiteSpace(cassieArgs[0]))
                return new(MessageType.CassieCaptionNoAnnouncement, this, "message");

            if (string.IsNullOrWhiteSpace(cassieArgs[1]))
            {
                foreach (Exiled.API.Features.Player ply in players)
                {
                    ply.PlayCassieAnnouncement(cassieArgs[0], makeNoise: isNoisy);
                }

                return new(true);
            }

            foreach (Exiled.API.Features.Player ply in players)
            {
                ply.MessageTranslated(cassieArgs[0], cassieArgs[1], makeNoise: isNoisy);
            }

            return new(true);
        }
    }
}
