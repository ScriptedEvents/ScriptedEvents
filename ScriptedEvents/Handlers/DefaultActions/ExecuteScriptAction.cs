using ScriptedEvents.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.API.Helpers;
using System;
using System.IO;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class ExecuteScriptAction : IAction
    {
        public string Name => "EXECUTESCRIPT";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 1) return new(false, "Missing argument: script name");
            string scriptName = Arguments[0];

            try
            {
                Script scr = ScriptHelper.ReadScript(scriptName);

                ScriptHelper.RunScript(scr);
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
