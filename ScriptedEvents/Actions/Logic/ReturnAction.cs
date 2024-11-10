namespace ScriptedEvents.Actions.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class ReturnAction : IScriptAction, ILogicAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Return";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Stops the script and returns variables to the original caller of the script.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variables", typeof(object), "The values to return. These values will be able to be accessed using the extraction operator.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (script.CallerScript == null)
            {
                return new(
                    false,
                    null,
                    new ErrorInfo(
                        "Script was not called",
                        "You cannot return to a script; this script was not called by another script using the CALL action.",
                        $"{Name} action")
                        .ToTrace());
            }

            var returns = new List<object>();
            foreach (string varName in RawArguments)
            {
                if (!VariableSystem.IsValidVariableSyntax<IVariable>(varName, out string processedName, out _))
                {
                    returns.Add(varName);
                    continue;
                }
                
                if (!VariableSystem.TryGetVariable(
                        processedName,
                        script,
                        out IVariable? variable,
                        true,
                        out var error))
                {
                    return new(false, null, error);
                }

                switch (variable!)
                {
                    case IPlayerVariable playerVariable:
                        returns.Add(playerVariable.Players.ToArray());
                        break;
                    case ILiteralVariable literalVariable:
                        returns.Add(literalVariable.Value);
                        break;
                    default:
                        throw new ImpossibleException();
                }
            }

            return new(true, new ActionReturnValues(returns), null, ActionFlags.StopScriptAndSendReturnedValues);
        }
    }
}
