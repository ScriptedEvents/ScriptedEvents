namespace ScriptedEvents.Actions
{
    using System;
    using System.IO;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

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
        public string Description => "Executes a provided script. Will NOT wait until called script finishes execution. Can provide arguments for the called script.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("script", typeof(string), "The script to call.", true),
            new Argument("arguments", typeof(string), "The arguments to provide for the called script. Can be empty. All arguments will be provided to the called script as {ARG1}, {ARG2} etc. and {ARGS}.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string scriptName = (string)Arguments[0];
            Script calledScript;

            try
            {
                calledScript = MainPlugin.ScriptModule.ReadScript(scriptName, script.Sender, false);
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
                calledScript.Execute();
                return new(true);
            }

            string[] args = RawArguments.JoinMessage(1).Split(' ');

            calledScript.AddVariable("{ARGS}", "Variable created using the CALL action.", VariableSystemV2.ReplaceVariables(RawArguments.JoinMessage(1), script));

            int arg = 0;
            foreach (string varName in args)
            {
                arg++;
                if (VariableSystemV2.TryGetPlayers(varName, script, out PlayerCollection val, requireBrackets: true))
                {
                    calledScript.AddPlayerVariable($"{{ARG{arg}}}", "Variable created using the CALL action.", val);

                    script.DebugLog($"Added player variable {varName} (as '{{ARG{arg}}}') to the called script.");
                    continue;
                }

                if (VariableSystemV2.TryGetVariable(varName, script, out VariableResult var, requireBrackets: true))
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