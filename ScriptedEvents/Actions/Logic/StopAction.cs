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
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Stops the event execution at this line.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("scriptName", typeof(string), "The script name to be stopped.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length == 0) return new(true, flags: ActionFlags.StopEventExecution);

            if (!Directory.Exists(ScriptHelper.ScriptPath))
            {
                // thunder add the error code cause i cant be bothered
                // thankies >wo
                throw new Exception("ooga booga");
            }

            ScriptHelper.StopScripts(Arguments[0]);

            return new(true);
        }
    }
}
