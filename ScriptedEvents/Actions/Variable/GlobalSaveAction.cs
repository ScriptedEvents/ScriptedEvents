using System;
using ScriptedEvents.API.Modules;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;
using ScriptedEvents.Variables.Interfaces;

namespace ScriptedEvents.Actions.Variable
{
    public class GlobalSaveAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Global";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Takes a local variable, deletes it, and creates a new global variable with the same name and value.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variable", typeof(IVariable), "The variable to make global. MUST BE LOCAL!", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            IVariable variable = (IVariable)Arguments[0]!;
            if (!script.RemoveVariable(variable))
            {
                var err = new ErrorInfo(
                    "Not a local variable.",
                    $"Provided variable '{variable.Name}' is not a local variable.",
                    Name).ToTrace();
                return new(false, null, err);
            }
            
            VariableSystem.DefineGlobalVariable(variable);
            return new(true);
        }
    }
}
