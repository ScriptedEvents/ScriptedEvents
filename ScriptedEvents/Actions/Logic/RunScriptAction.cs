using ScriptedEvents.API.Modules;

namespace ScriptedEvents.Actions.Logic
{
    using System;
    using System.Collections.Generic;
    using MEC;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class RunScriptAction : IHelpInfo, ITimingAction
    {
        /// <inheritdoc/>
        public string Name => "RunScript";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Executes a provided script with custom arguments.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("NoWait", "Action will not wait until the script stops running."),
                new Option("WaitForFinish", "Action will wait until the called script stops running before allowing the current script to proceed.")),
            new Argument("script", typeof(Script), "The script to call.", true),
            new Argument("arguments", typeof(string), "The optional arguments to provide for the called script. Arguments are provided in the same way as with custom commands.", false),
        };

        /// <inheritdoc/>
        public float? Execute(Script script, out ActionResponse message)
        {
            bool shouldWait = string.Equals((string)Arguments[0]!, "WaitForFinish", StringComparison.OrdinalIgnoreCase);
            Script calledScript = (Script)Arguments[1]!;

            switch (Arguments.Length <= 2)
            {
                case true when !shouldWait:
                {
                    if (!ScriptModule.Singleton!.TryRunScript(calledScript, out var err, out _))
                    {
                        message = new(false, null, err);
                    }
                    else
                    {
                        message = new(true);
                    }

                    return 0;
                }

                case true when shouldWait:
                {
                    if (!ScriptModule.Singleton!.TryRunScript(calledScript, out var err, out _))
                    {
                        message = new(false, null, err);
                        return 0;
                    }

                    message = new(true);
                    return Timing.WaitUntilDone(WaitCoroutine(calledScript, script));
                }
            }

            string[] args = Arguments.JoinMessage(2).Split(' ');

            calledScript.AddLiteralVariable("ARGS", Arguments.JoinMessage(2), true);

            int argCount = 0;
            foreach (string arg in args)
            {
                argCount++;
                calledScript.AddLiteralVariable($"ARG{argCount}", arg, true);
            }

            if (!ScriptModule.Singleton!.TryRunScript(calledScript, out var err1, out _))
            {
                message = new(false, null, err1);
                return 0;
            }

            message = new(true);
            return shouldWait
                ? Timing.WaitUntilDone(WaitCoroutine(calledScript, script))
                : 0;
        }

        private CoroutineHandle WaitCoroutine(Script scriptToCall, Script script)
        {
            string coroutineKey = $"CALL_WAIT_FOR_FINISH_COROUTINE_{DateTime.Now.Ticks}";
            CoroutineHandle handle = Timing.RunCoroutine(InternalWaitUntil(scriptToCall), coroutineKey);
            CoroutineHelper.AddCoroutine("CALL", handle, script);
            return handle;
        }

        private IEnumerator<float> InternalWaitUntil(Script calledScript)
        {
            while (ScriptModule.Singleton!.RunningScripts.ContainsKey(calledScript))
            {
                yield return Timing.WaitForSeconds(1 / MainPlugin.Configs.WaitUntilFrequency);
            }
        }
    }
}