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

    public class CallAction : IHelpInfo, ITimingAction
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
        public float? Execute(Script script, out ActionResponse message)
        {
            string mode = Arguments[0].ToUpper();
            if (mode == "LABEL")
            {
                int curLine = script.CurrentLine;

                if (!script.Jump((string)Arguments[1]))
                {
                    message = new(false, "Invalid line or label provided!");
                    return 0;
                }

                script.CallLines.Add(curLine);

                script.DebugLog(script.CallLines[0].ToString());
                message = new(true);
                return 0;
            }

            if (mode == "SCRIPT")
            {
                string scriptName = (string)Arguments[1];
                Script calledScript;

                try
                {
                    calledScript = ScriptHelper.ReadScript(scriptName, script.Sender, false);
                    calledScript.CallerScript = script;
                }
                catch (DisabledScriptException)
                {
                    message = new(false, $"Script '{scriptName}' is disabled.");
                    return 0;
                }
                catch (FileNotFoundException)
                {
                    message = new(false, $"Script '{scriptName}' not found.");
                    return 0;
                }

                if (Arguments.Length < 3)
                {
                    message = new(true);
                    return Timing.WaitUntilDone(RunScript(calledScript, script));
                }

                string[] args = RawArguments.JoinMessage(2).Split(' ');

                calledScript.AddVariable("{ARGS}", "Variable created using the CALL action.", VariableSystem.ReplaceVariables(RawArguments.JoinMessage(2), script));

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

                message = new(true);
                return Timing.WaitUntilDone(RunScript(calledScript, script));
            }

            message = new(false, "Invalid mode provided.");
            return 0;
        }

        private CoroutineHandle RunScript(Script scriptToCall, Script script)
        {
            scriptToCall.Execute();
            string coroutineKey = $"CALL_WAIT_FOR_FINISH_COROUTINE_{DateTime.UtcNow.Ticks}";
            CoroutineHandle handle = Timing.RunCoroutine(InternalWaitUntil(scriptToCall), coroutineKey);
            CoroutineHelper.AddCoroutine("CALL", handle, script);
            return handle;
        }

        private IEnumerator<float> InternalWaitUntil(Script calledScript)
        {
            while (ScriptHelper.RunningScripts.ContainsKey(calledScript))
            {
                yield return Timing.WaitForSeconds(1 / MainPlugin.Configs.WaitUntilFrequency);
            }
        }
    }
}