using System;
using System.Collections.Generic;
using MEC;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.YieldingActions
{
    public class WaitUntilAction : ITimingAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "WaitUntil";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Yielding;

        /// <inheritdoc/>
        public string Description => "Yields the script until the provided conditon results in TRUE.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("condition", typeof(string), "The conditon.", true),
        };

        /// <inheritdoc/>
        public float? Execute(Script script, out ActionResponse message)
        {
            string coroutineKey = $"WAITUNTIL_COROUTINE_{DateTime.Now.Ticks}";
            CoroutineHandle handle = Timing.RunCoroutine(InternalWaitUntil(script, RawArguments.JoinMessage()), coroutineKey);
            CoroutineHelper.AddCoroutine("WAITUNTIL", handle, script);

            message = new(true);
            return Timing.WaitUntilDone(handle);
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
                    Logger.ScriptError($"WaitUntil condition error: {response.Message}", script);
                    break;
                }

                yield return Timing.WaitForSeconds(1 / MainPlugin.Configs.WaitUntilFrequency);
            }
        }
    }
}
