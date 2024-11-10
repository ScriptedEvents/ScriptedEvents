using System.Linq;

namespace ScriptedEvents.Actions.Logic
{
    using System;
    using System.IO;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class StopAction : IScriptAction, ILogicAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Stop";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Stops script execution.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("scriptName", typeof(string), "Leave empty to stop this script. Provide a script name to stop a specific script or use '*' to stop all scripts EXCEPT this script.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length == 0) 
                return new(true, flags: ActionFlags.StopEventExecution);

            string scriptName = (string)Arguments[0]!;

            if (scriptName == "*")
            {
                foreach (var toStop in MainPlugin.ScriptModule.RunningScripts.Keys.Where(toStop => toStop != script))
                {
                    MainPlugin.ScriptModule.StopScript(toStop);
                }

                return new(true);
            }
            
            MainPlugin.ScriptModule.StopScripts(scriptName);
            return new(true);
        }
    }
}
