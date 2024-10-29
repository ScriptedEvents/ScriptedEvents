using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class IsRunningAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "ISRUNNING";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.ScriptInfo;

        /// <inheritdoc/>
        public string Description => "Returns TRUE if a specific script is running, else FALSE.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("scriptName", typeof(string), "The name of the script.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string ret = MainPlugin.ScriptModule.RunningScripts.Any(scr => scr.Key.ScriptName == (string)Arguments[0] && scr.Value.IsRunning).ToUpper();
            return new(true, variablesToRet: new[] { ret });
        }
    }
}