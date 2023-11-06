﻿namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using MEC;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;

    public class WaitUntilAction : ITimingAction, IHelpInfo
    {
        public static List<string> Coroutines { get; } = new();

        /// <inheritdoc/>
        public string Name => "WAITUNTIL";

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

            string coroutineKey = $"WAITUNTIL_COROUTINE_{DateTime.UtcNow.Ticks}";
            Coroutines.Add(coroutineKey);
            message = new(true);
            return Timing.WaitUntilDone(InternalWaitUntil(script, string.Join(string.Empty, Arguments)), coroutineKey);
        }

        private IEnumerator<float> InternalWaitUntil(Script script, string input)
        {
            while (true)
            {
                ConditionResponse response = ConditionHelper.Evaluate(input, script);
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
