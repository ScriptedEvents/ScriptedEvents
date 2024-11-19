using System;
using System.Linq;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.ScriptInfo
{
    public class IsRunningAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "IsRunning";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.ScriptInfo;

        /// <inheritdoc/>
        public string Description => "Returns TRUE if a specific script is running, else FALSE.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("scriptName", typeof(string), "The name of a script.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string ret = MainPlugin.ScriptModule.RunningScripts.Any(scr => scr.Key.ScriptName == (string)Arguments[0]! && scr.Value.IsRunning).ToUpper();
            return new(true, new(ret));
        }
    }
}