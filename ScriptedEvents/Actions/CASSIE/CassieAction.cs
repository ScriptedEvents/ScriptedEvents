namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.Actions.Samples.Interfaces;
    using ScriptedEvents.Actions.Samples.Providers;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class CassieAction : IScriptAction, IHelpInfo, ISampleAction
    {
        /// <inheritdoc/>
        public string Name => "CASSIE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Cassie;

        /// <inheritdoc/>
        public string Description => "Makes a cassie announcement for the entire facility.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("SILENT", "Makes a silent announcement."),
                new("LOUD", "Makes a loud announcement.")),
            new Argument("message", typeof(string), "The message. Separate message with a | to specify a caption.", true),
        };

        /// <inheritdoc/>
        public ISampleProvider Samples { get; } = new CassieSamples();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            bool isNoisy = Arguments[0].ToUpper() == "LOUD";
            string text = Arguments.JoinMessage(1);

            if (string.IsNullOrWhiteSpace(text))
                return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string[] cassieArgs = text.Split('|');

            for (int i = 0; i < cassieArgs.Length; i++)
            {
                cassieArgs[i] = VariableSystemV2.ReplaceVariables(cassieArgs[i], script);
            }

            if (cassieArgs.Length == 1)
            {
                text = VariableSystemV2.ReplaceVariables(text, script);
                Cassie.MessageTranslated(text, text, isNoisy: isNoisy);
                return new(true);
            }

            if (string.IsNullOrWhiteSpace(cassieArgs[0]))
                return new(MessageType.CassieCaptionNoAnnouncement, this, "message");

            if (string.IsNullOrWhiteSpace(cassieArgs[1]))
                Cassie.Message(cassieArgs[0], isNoisy: isNoisy);
            else
                Cassie.MessageTranslated(cassieArgs[0], cassieArgs[1], isNoisy: isNoisy);

            return new(true);
        }
    }
}
