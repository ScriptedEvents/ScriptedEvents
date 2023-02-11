﻿namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using MEC;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class WaitUntilDebugAction : ITimingAction, IHiddenAction
    {
        public static List<string> Coroutines { get; } = new();
        public string Name => "WAITUNTILDEBUG";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public float? Execute(Script scr, out ActionResponse message)
        {
            if (Arguments.Length < 1)
            {
                message = new(false, "Missing argument: condition");
                return null;
            }

            string coroutineKey = $"WAITUNTIL_DEBUG_COROUTINE_{DateTime.UtcNow.Ticks}";
            Coroutines.Add(coroutineKey);
            message = new(true);
            return Timing.WaitUntilDone(InternalWaitUntil(string.Join(string.Empty, Arguments)), coroutineKey);
        }

        private IEnumerator<float> InternalWaitUntil(string input)
        {
            while (true)
            {
                ConditionResponse response = ConditionHelper.Evaluate(input);
                if (response.Success)
                {
                    if (response.Passed)
                        break;
                }
                else
                {
                    Log.Warn($"[WAITUNTILDEBUG] WaitUntilDebug condition error: {response.Message}");
                    break;
                }

                Log.Info($"CONDITION: {ConditionVariables.ReplaceVariables(input)} \\\\ PASSED: {response.Passed}");
                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
