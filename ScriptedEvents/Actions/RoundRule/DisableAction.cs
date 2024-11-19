using System;
using ScriptedEvents.API.Constants;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.RoundRule
{
    public class DisableAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "DisableRule";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Disables a feature for the entire round.";

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.DisableKeys;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("key", typeof(string), "The key of the feature to disable. See documentation for a whole list of keys.", true),
        };

        public ActionResponse Execute(Script script)
        {
            string key = Arguments[0]!.ToUpper();

            if (!MainPlugin.EventHandlingModule.DisabledKeys.Contains(key))
                MainPlugin.EventHandlingModule.DisabledKeys.Add(key);

            return new(true);
        }
    }
}
