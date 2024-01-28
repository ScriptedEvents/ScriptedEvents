namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class EnableAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "ENABLE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Enables a previously disabled round feature.";

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.DisableKeys;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("key", typeof(string), "The key of the feature to enable. See documentation for a whole list of keys.", true),
        };

        public ActionResponse Execute(Script script)
        {
            string key = Arguments[0].ToUpper();

            if (MainPlugin.Handlers.DisabledKeys.Contains(key))
                MainPlugin.Handlers.DisabledKeys.Remove(key);

            return new(true);
        }
    }
}
