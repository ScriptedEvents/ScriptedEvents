using System;

namespace ScriptedEvents.API.Features.Exceptions
{
    public class DisabledScriptException : Exception
    {
        public string ScriptName { get; }
        public DisabledScriptException(string scriptName)
            : base()
        {
            ScriptName = scriptName;
            Message = $"The given script '{scriptName}' is disabled.";
        }
    }
}
