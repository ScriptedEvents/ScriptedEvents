namespace ScriptedEvents.Actions
{
    using System;
    using System.IO;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class StopAction : IScriptAction, ILogicAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "STOP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

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
            if (Arguments.Length == 0) return new(true, flags: ActionFlags.StopEventExecution);

            string scriptName = (string)Arguments[0];

            if (scriptName == "*")
            {
                foreach (Script toStop in MainPlugin.ScriptModule.RunningScripts.Keys)
                {
                    if (toStop != script) MainPlugin.ScriptModule.StopScript(toStop);
                }

                return new(true);
            }

            if (!Directory.Exists(ScriptModule.BasePath))
            {
                return new(false, ErrorGen.Get(ErrorCode.IOMissing));
            }

            MainPlugin.ScriptModule.StopScripts(scriptName);
            return new(true);
        }
    }
}
