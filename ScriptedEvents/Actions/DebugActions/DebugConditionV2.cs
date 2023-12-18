namespace ScriptedEvents.Actions
{
    using System;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class DebugConditionV2 : IScriptAction, IHiddenAction
    {
        /// <inheritdoc/>
        public string Name => "DEBUGCONDITIONV2";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Debug;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Log.Info(ConditionHelperV2.Evaluate(string.Join(" ", Arguments), script));
            return new(true);
        }
    }
}