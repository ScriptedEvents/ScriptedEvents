namespace ScriptedEvents.Actions
{
    using System;
    using System.IO;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using ScriptedEvents.Variables.Interfaces;

    public class CallAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "CALL";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Moves to the provided label or executes a script.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("LABEL", "Moves to a specific label."),
                new("SCRIPT", "Executes a different script.")),
            new Argument("destination", typeof(string), "The target of this action. Dictated by the mode selected.", true),
            new Argument("vars", typeof(string), "The variables to create for a called script. Seperate variables with spaces to create multiple. Used only for the SCRIPT mode.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = Arguments[0].ToUpper();
            if (mode == "LABEL")
            {
                int curLine = script.CurrentLine;

                if (!script.Jump((string)Arguments[1]))
                {
                    return new(false, "Invalid line or label provided!");
                }

                script.CallLines.Add(curLine);

                script.DebugLog(script.CallLines[0].ToString());
                return new(true);
            }

            if (mode == "SCRIPT")
            {
                string scriptName = (string)Arguments[1];
                Script calledScript;

                try
                {
                    calledScript = ScriptModule.ReadScript(scriptName, script.Sender, false);
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

                if (Arguments.Length < 3)
                {
                    calledScript.Execute();
                    return new(true);
                }

                calledScript.AddVariable($"{{ARGS}}", "Variable created using the CALL action.", Arguments.JoinMessage(2));
                string[] variables = RawArguments.JoinMessage(2).Split(' ');

                int arg = 0;
                foreach (string varName in variables)
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

                    Log.Warn(ErrorGen.Get(ErrorCode.InvalidVariable));
                }

                calledScript.Execute();
                return new(true);
            }

            return new(false, "Invalid mode provided.");
        }
    }
}