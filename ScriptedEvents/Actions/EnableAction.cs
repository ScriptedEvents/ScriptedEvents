namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    public class EnableAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "ENABLE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Enables a previously disabled round feature.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("key", typeof(string), "The key of the feature to disable. See documentation for a whole list of keys.", true),
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string key = Arguments[0].ToUpper();

            if (MainPlugin.Handlers.DisabledKeys.Contains(key))
                MainPlugin.Handlers.DisabledKeys.Remove(key);

            return new(true);
        }
    }
}
