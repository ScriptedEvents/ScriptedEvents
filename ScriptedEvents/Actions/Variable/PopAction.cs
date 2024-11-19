using System;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;
using ScriptedEvents.Variables.Interfaces;

namespace ScriptedEvents.Actions.Variable
{
    public class PopAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Pop";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Deletes the provided local variable and returns its value. Can be used for renaming local variables.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variable", typeof(IVariable), "The variable to delete.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var variable = (IVariable)Arguments[0]!;
            if (!script.IsVariableLocal(variable))
            {
                var err = new ErrorInfo(
                    "Not a local variable.",
                    $"Provided variable {variable.Name} is not a local variable.",
                    Name).ToTrace();
                return new(false, null, err);
            }

            switch (variable)
            {
                case IPlayerVariable plrVar:
                    script.RemoveVariable(plrVar);
                    return new(true, new(plrVar.GetPlayers()));
                case ILiteralVariable lvVar:
                    script.RemoveVariable(lvVar);
                    return new(true, new(lvVar.Value));
                default:
                    throw new ArgumentException("Variable is not a valid variable type");
            }
        }
    }
}