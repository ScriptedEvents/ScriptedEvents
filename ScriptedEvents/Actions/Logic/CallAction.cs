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
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

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
        public string Description => "Executes a provided script. Will wait until called script finishes execution. Can provide arguments for the called script.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("script", typeof(string), "The script to call.", true),
            new Argument("arguments", typeof(string), "The arguments to provide for the called script. Can be empty. All arguments will be provided to the called script as {ARG1}, {ARG2} etc. and {ARGS}.", false),
        };

        /// <inheritdoc/>
        public float? Execute(Script script, out ActionResponse message)
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
                message = new(false, $"Script '{scriptName}' is disabled.");
                return 0;
            }
            catch (FileNotFoundException)
            {
                message = new(false, $"Script '{scriptName}' not found.");
                return 0;
            }

            if (Arguments.Length < 2)
            {
                message = new(true);
                return Timing.WaitUntilDone(RunScript(calledScript, script));
            }

            string[] args = RawArguments.JoinMessage(1).Split(' ');

            calledScript.AddVariable("{ARGS}", "Variable created using the CALL action.", VariableSystemV2.ReplaceVariables(Arguments.JoinMessage(1), script));

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

                if (VariableSystemV2.TryGetVariable(varName, script, out VariableResult result, requireBrackets: true) && result.ProcessorSuccess)
                {
                    calledScript.AddVariable($"{{ARG{arg}}}", "Variable created using the CALL action.", result.String());

                    script.DebugLog($"Added variable {varName} (as '{{ARG{arg}}}') to the called script.");
                    continue;
                }

                calledScript.AddVariable($"{{ARG{arg}}}", "Variable created using the CALL action.", varName);
            }

            message = new(true);
            return Timing.WaitUntilDone(RunScript(calledScript, script));
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
            while (MainPlugin.ScriptModule.RunningScripts.ContainsKey(calledScript))
            {
                yield return Timing.WaitForSeconds(1 / MainPlugin.Configs.WaitUntilFrequency);
            }
        }
    }
}