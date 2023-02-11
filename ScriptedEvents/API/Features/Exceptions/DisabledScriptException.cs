namespace ScriptedEvents.API.Features.Exceptions
{
    using System;

    public class DisabledScriptException : Exception
    {
        public DisabledScriptException(string scriptName)
            : base($"The given script '{scriptName}' is disabled.")
        {
            ScriptName = scriptName;
        }

        public string ScriptName { get; }
    }
}
