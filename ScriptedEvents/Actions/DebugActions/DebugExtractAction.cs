using System;
using System.Linq;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.DebugActions
{
    public class DebugExtractAction : IScriptAction, IHiddenAction
    {
        /// <inheritdoc/>
        public string Name => "DEBUGEXTRACT";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Debug;

        public Argument[] ExpectedArguments => Array.Empty<Argument>();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, variablesToRet: new[] { (object)Exiled.API.Features.Player.List.ToArray(), (object)"10" });
        }
    }
}