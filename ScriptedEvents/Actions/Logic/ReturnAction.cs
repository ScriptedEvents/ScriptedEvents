namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

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
        public string Description => "Moves to the last CALL action executed.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode to return with. Use LABEL to go back to the called label, or SCRIPT to stop the execution. Defaults to LABEL", false),
            new Argument("values", typeof(object), "The variables to return. These variables will be copied to the original caller with the same name and value. Used only with SCRIPT mode.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = Arguments.Length > 0 ? Arguments.First().ToUpper() : null;

            if (mode == "LABEL" || mode == null)
            {
                if (script.CallLines.Count == 0)
                    return new(false, "This action must be run after the CALL action.");

                int lastLine = script.CallLines.Count - 1;

                script.Jump(script.CallLines[lastLine] + 1);
                script.CallLines.RemoveAt(lastLine);

                return new(true);
            }
            else if (mode == "SCRIPT")
            {
                if (script.CallerScript == null) return new(false, "You cannot return to a script; this script was not called by another script using the CALL action.");
            }
            else
            {
                return new(false, $"Invalid mode '{mode}' provided.");
            }

            foreach (string varName in RawArguments.Skip(1))
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

            return new(true);
        }
    }
}
