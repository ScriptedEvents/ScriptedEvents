namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using ScriptedEvents.Variables.Interfaces;

    public class ReturnAction : IScriptAction, ILogicAction, IHelpInfo, IArgumentVariable
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
        public string Description => "Stops the script and returns variables to the script which called it.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variables", typeof(object), "The variables to return. These variables will be copied to the original caller with the same name and value.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (script.CallerScript == null)
                return new(false, "You cannot return to a script; this script was not called by another script using the CALL action.");

            foreach (string varName in RawArguments)
            {
                if (VariableSystem.TryGetPlayers(varName, out PlayerCollection players, script))
                {
                    script.CallerScript.AddPlayerVariable(varName, "Created using the RETURN action.", players);
                }
                else if (VariableSystem.TryGetVariable(varName, out IConditionVariable value, out bool _, script))
                {
                    script.CallerScript.AddVariable(varName, "Created using the RETURN action.", value.String());
                }
                else
                {
                    return new(false, $"Return variable '{varName}' is invalid.");
                }
            }

            return new(true, flags: ActionFlags.StopEventExecution);
        }
    }
}
