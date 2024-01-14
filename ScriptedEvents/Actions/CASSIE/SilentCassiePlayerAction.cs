namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using Exiled.API.Extensions;
    using Exiled.API.Features;

    using ScriptedEvents.Actions.Samples.Interfaces;
    using ScriptedEvents.Actions.Samples.Providers;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
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
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Cassie;

        /// <inheritdoc/>
        public string Description => "Makes a silent cassie announcement for specific players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to play the CASSIE announcement for.", true),
            new Argument("message", typeof(string), "The message. Separate message with a | to specify a caption. Variables are supported.", true),
        };

        /// <inheritdoc/>
        public ISampleProvider Samples { get; } = new CassieSamples();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out PlayerCollection players, script))
                return new(false, players.Message);

            string text = string.Join(" ", Arguments.Skip(1));

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
