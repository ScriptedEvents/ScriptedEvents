using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class ReturnAction : IScriptAction, ILogicAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "RETURN";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Stops the script and returns variables to the original caller of the script.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variables", typeof(object), "The variables to return. These variables will be copied to the original caller with the same name and value.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            /*
            if (script.CallerScript == null)
                return new(false, "You cannot return to a script; this script was not called by another script using the CALL action.", ActionFlags.FatalError);

            foreach (string varName in RawArguments)
            {
                if (VariableSystem.TryGetPlayers(varName, script, out PlayerCollection players))
                {
                    script.CallerScript.AddPlayerVariable(varName, "Created using the RETURN action.", players);
                }
                else if (VariableSystem.TryGetVariable(varName, script, out VariableResult res))
                {
                    if (!res.ProcessorSuccess)
                        return new(false, res.Message);

                    script.CallerScript.AddLiteralVariable(varName, "Created using the RETURN action.", res.String(script));
                }
                else
                {
                    return new(false, $"Return variable '{varName}' is invalid.");
                }
            }

            return new(true, flags: ActionFlags.StopEventExecution);
            */
            return new(false, "Action not implemented.");
        }
    }
}
