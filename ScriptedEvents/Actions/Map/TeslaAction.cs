using System;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Map
{
    // Todo: Needs reworked entirely
    public class TeslaAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Tesla";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Modifies tesla gates.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => Array.Empty<Argument>();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(
                false, 
                null, 
                new ErrorInfo(
                    "Not implemented",
                    "This action is not implemented.",
                    "Tesla action"
                ).ToTrace()
            );
        }
    }
}
