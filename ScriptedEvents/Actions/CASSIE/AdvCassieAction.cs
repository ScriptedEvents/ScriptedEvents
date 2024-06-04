namespace ScriptedEvents.Actions
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class AdvCassieAction : IScriptAction, IHelpInfo
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
            new Argument("players", typeof(Player[]), "The players to play the CASSIE announcement for.", true),
            new Argument("message", typeof(string), "The message. Separate message with a | to specify a caption.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[1];
            bool isNoisy = Arguments[0].ToUpper() == "LOUD";
            string text = Arguments.JoinMessage(2);

            string[] cassieArgs = text.Split('|');

            for (int i = 0; i < cassieArgs.Length; i++)
            {
                cassieArgs[i] = VariableSystemV2.ReplaceVariables(cassieArgs[i], script);
            }

            if (cassieArgs.Length == 1)
            {
                text = VariableSystemV2.ReplaceVariables(text, script);
                foreach (Player ply in players)
                {
                    ply.MessageTranslated(text, text, makeNoise: isNoisy);
                }

                return new(true);
            }

            if (string.IsNullOrWhiteSpace(cassieArgs[0]))
                return new(MessageType.CassieCaptionNoAnnouncement, this, "message");

            if (string.IsNullOrWhiteSpace(cassieArgs[1]))
            {
                foreach (Player ply in players)
                {
                    ply.PlayCassieAnnouncement(cassieArgs[0], makeNoise: isNoisy);
                }

                return new(true);
            }

            foreach (Player ply in players)
            {
                ply.MessageTranslated(cassieArgs[0], cassieArgs[1], makeNoise: isNoisy);
            }

            return new(true);
        }
    }
}
