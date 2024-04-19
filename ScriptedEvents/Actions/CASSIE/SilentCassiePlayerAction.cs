namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Extensions;
    using Exiled.API.Features;

    using ScriptedEvents.Actions.Samples.Interfaces;
    using ScriptedEvents.Actions.Samples.Providers;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class SilentCassiePlayerAction : IScriptAction, IHelpInfo, ISampleAction
    {
        /// <inheritdoc/>
        public string Name => "SILENTCASSIEPLAYER";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Cassie;

        /// <inheritdoc/>
        public string Description => "Makes a silent cassie announcement for specific players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to play the CASSIE announcement for.", true),
            new Argument("message", typeof(string), "The message. Separate message with a | to specify a caption.", true),
        };

        /// <inheritdoc/>
        public ISampleProvider Samples { get; } = new CassieSamples();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[0];

            string text = Arguments.JoinMessage(1);

            if (string.IsNullOrWhiteSpace(text))
                return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string[] cassieArgs = text.Split('|');

            for (int i = 0; i < cassieArgs.Length; i++)
            {
                cassieArgs[i] = VariableSystem.ReplaceVariables(cassieArgs[i], script);
            }

            if (cassieArgs.Length == 1)
            {
                text = VariableSystem.ReplaceVariables(text, script);
                foreach (Player ply in players)
                {
                    ply.MessageTranslated(text, text, makeNoise: false);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(cassieArgs[0]))
                    return new(MessageType.CassieCaptionNoAnnouncement, this, "message");

                if (string.IsNullOrWhiteSpace(cassieArgs[1]))
                {
                    foreach (Player ply in players)
                    {
                        ply.PlayCassieAnnouncement(cassieArgs[0], makeNoise: false);
                    }
                }
                else
                {
                    foreach (Player ply in players)
                    {
                        ply.MessageTranslated(cassieArgs[0], cassieArgs[1], makeNoise: false);
                    }
                }
            }

            return new(true, string.Empty);
        }
    }
}
