namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using MEC;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class WaitUntilDebugAction : ITimingAction, IHiddenAction, IHelpInfo
    {
        public static List<string> Coroutines { get; } = new();

        /// <inheritdoc/>
        public string Name => "WAITUNTILDEBUG";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Yielding;

        /// <inheritdoc/>
        public string Description => "Reads the condition and yields execution of the script until the condition is TRUE.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("condition", typeof(string), "The condition to check. Variables & Math are supported.", true),
        };

        /// <inheritdoc/>
        public float? Execute(Script script, out ActionResponse message)
        {
            if (Arguments.Length < 1)
            {
                message = new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);
                return null;
            }

            string coroutineKey = $"WAITUNTIL_DEBUG_COROUTINE_{DateTime.UtcNow.Ticks}";
            Coroutines.Add(coroutineKey);
            message = new(true);
            return Timing.WaitUntilDone(InternalWaitUntil(script, string.Join(string.Empty, Arguments)), coroutineKey);
        }

        private IEnumerator<float> InternalWaitUntil(Script script, string input)
        {
            while (true)
            {
                ConditionResponse response = ConditionHelper.Evaluate(input, script);
                Log.Info($"CONDITION: {VariableSystem.ReplaceVariables(input)} \\\\ SUCCESS: {response.Success} \\\\ PASSED: {response.Passed}");
                if (response.Success)
                {
                    if (response.Passed)
                        break;
                }
                else
                {
                    Log.Warn($"[Script: {script.ScriptName}] [WAITUNTILDEBUG] WaitUntilDebug condition error: {response.Message}");
                    break;
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
