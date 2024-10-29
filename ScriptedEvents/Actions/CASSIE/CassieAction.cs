namespace ScriptedEvents.Actions.CASSIE
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class CassieAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Cassie";

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
                new Option("Silent", "Makes a silent announcement."),
                new Option("Loud", "Makes a loud announcement.")),
            new Argument("message", typeof(string), "The message for cassie.", true),
            new Argument("caption", typeof(string), "An optional caption for the announcement. Use NONE for no captions.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            bool isNoisy = Arguments[0].ToUpper() == "LOUD";
            string message = Arguments[1].ToString();

            if (Arguments.Length > 2)
            {
                Cassie.MessageTranslated(message, Arguments[2].ToString().ToUpper() == "NONE" ? string.Empty : Arguments[2].ToString(), isNoisy: isNoisy);
                return new(true);
            }

            Cassie.Message(message, isNoisy: isNoisy);
            return new(true);
        }
    }
}
