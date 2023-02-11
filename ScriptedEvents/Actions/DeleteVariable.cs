using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.Handlers.Variables;
using System;

namespace ScriptedEvents.Actions
{
    public class DeleteVariable : IScriptAction, IHelpInfo
    {
        public string Name => "DELVARIABLE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Deletes a previously-defined variable.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(string), "The name of the variable.", true),
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(false, "Missing argument: variableName");
            if (ConditionVariables.DefinedVariables.ContainsKey(Arguments[0]))
            {
                ConditionVariables.DefinedVariables.Remove(Arguments[0]);
                return new(true);
            }

            return new(false, $"Invalid variable '{Arguments[0]}'");
        }
    }
}
