namespace ScriptedEvents.Actions
{
    using System;
    using System.IO;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;

    public class ExecuteScriptAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "EXECUTESCRIPT";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Executes a different script.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("scriptName", typeof(string), "The name of the script.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);
            string scriptName = Arguments[0];

            try
            {
                ScriptHelper.ReadAndRun(scriptName, script.Sender);
                return new(true);
            }
            catch (DisabledScriptException)
            {
                return new(false, $"Script '{scriptName}' is disabled.");
            }
            catch (FileNotFoundException)
            {
                return new(false, $"Script '{scriptName}' not found.");
            }
        }
    }
}
