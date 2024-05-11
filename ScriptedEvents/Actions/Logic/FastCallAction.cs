namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using MEC;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using ScriptedEvents.Variables.Interfaces;

    public class FastCallAction : IHelpInfo, IScriptAction
    {
        /// <inheritdoc/>
        public string Name => "FASTCALL";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Calls the provided script. This action does not yield until the called script stops running.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("destination", typeof(string), "The target of this action. Dictated by the mode selected.", true),
            new Argument("vars", typeof(string), "The variables to create for a called script. Seperate variables with spaces to create multiple.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string scriptName = (string)Arguments[0];
            Script calledScript;

            try
            {
                calledScript = ScriptHelper.ReadScript(scriptName, script.Sender, false);
                calledScript.CallerScript = script;
            }
            catch (DisabledScriptException)
            {
                return new(false, $"Script '{scriptName}' is disabled.");
            }
            catch (FileNotFoundException)
            {
                return new(false, $"Script '{scriptName}' not found.");
            }

            if (Arguments.Length < 2)
            {
                return new(true);
            }

            string[] args = RawArguments.JoinMessage(1).Split(' ');

            calledScript.AddVariable("{ARGS}", "Variable created using the CALL action.", VariableSystem.ReplaceVariables(RawArguments.JoinMessage(1), script));

            int arg = 0;
            foreach (string varName in args)
            {
                arg++;
                if (VariableSystem.TryGetPlayers(varName, out PlayerCollection val, script, requireBrackets: true))
                {
                    calledScript.AddPlayerVariable($"{{ARG{arg}}}", "Variable created using the CALL action.", val);

                    script.DebugLog($"Added player variable {varName} (as '{{ARG{arg}}}') to the called script.");
                    continue;
                }

                if (VariableSystem.TryGetVariable(varName, out IConditionVariable var, out bool _, script, requireBrackets: true))
                {
                    calledScript.AddVariable($"{{ARG{arg}}}", "Variable created using the CALL action.", var.String());

                    script.DebugLog($"Added variable {varName} (as '{{ARG{arg}}}') to the called script.");
                    continue;
                }

                calledScript.AddVariable($"{{ARG{arg}}}", "Variable created using the CALL action.", varName);
            }

            calledScript.Execute();
            return new(true);
        }
    }
}