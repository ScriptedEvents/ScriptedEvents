﻿namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using MEC;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;

    public class WaitUntilAction : ITimingAction, IHelpInfo
    {
        public static List<string> Coroutines { get; } = new();

        public string Name => "WAITUNTIL";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Reads the condition and yields execution of the script until the condition is TRUE.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("condition", typeof(string), "The condition to check. Variables & Math are supported.", true),
        };

        public float? Execute(Script script, out ActionResponse message)
        {
            if (Arguments.Length < 1)
            {
                message = new(false, "Missing argument: condition");
                return null;
            }

            string coroutineKey = $"WAITUNTIL_COROUTINE_{DateTime.UtcNow.Ticks}";
            Coroutines.Add(coroutineKey);
            message = new(true);
            return Timing.WaitUntilDone(InternalWaitUntil(script, string.Join(string.Empty, Arguments)), coroutineKey);
        }

        private IEnumerator<float> InternalWaitUntil(Script script, string input)
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
                    Log.Warn($"[Script: {script.ScriptName}] [WAITUNTIL] WaitUntil condition error: {response.Message}");
                    break;
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
