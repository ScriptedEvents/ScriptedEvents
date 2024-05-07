namespace ScriptedEvents.Actions
{
    using System;
    using System.IO;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
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
        public string Description => "Stops the event execution at this line, or stop a script with the specific name.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("scriptName", typeof(string), "The script name to be stopped. Leave empty to stop this script. Use '*' to stop all scripts except the one running the action.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length == 0) return new(true, flags: ActionFlags.StopEventExecution);

            string scriptName = (string)Arguments[0];

            if (scriptName == "*")
            {
                foreach (Script toStop in ScriptModule.RunningScripts.Keys)
                {
                    if (toStop != script) ScriptModule.StopScript(toStop);
                }

                return new(true);
            }

            if (!Directory.Exists(ScriptModule.ScriptPath))
            {
                return new(false, ErrorGen.Get(ErrorCode.IOMissing));
            }

            ScriptModule.StopScripts(scriptName);
            return new(true);
        }
    }
}
