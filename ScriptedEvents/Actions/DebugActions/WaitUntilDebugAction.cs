namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;

    using MEC;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class WaitUntilDebugAction : ITimingAction, IHiddenAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "WAITUNTILDEBUG";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Debug;

        /// <inheritdoc/>
        public string Description => "Reads the condition and yields execution of the script until the condition is TRUE.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("condition", typeof(string), "The condition to check. Math is supported.", true),
        };

        /// <inheritdoc/>
        public float? Execute(Script script, out ActionResponse message)
        {
            string coroutineKey = $"WAITUNTIL_DEBUG_COROUTINE_{DateTime.UtcNow.Ticks}";
            CoroutineHandle handle = Timing.RunCoroutine(InternalWaitUntil(script, Arguments.JoinMessage(0)), coroutineKey);
            CoroutineHelper.AddCoroutine("WAITUNTIL_DEBUG", handle, script);

            message = new(true);
            return Timing.WaitUntilDone(handle);
        }

        private IEnumerator<float> InternalWaitUntil(Script script, string input)
        {
            while (true)
            {
                ConditionResponse response = ConditionHelperV2.Evaluate(input, script);
                Logger.Info($"CONDITION: {SEParser.ReplaceContaminatedValueSyntax(input, script)} \\\\ SUCCESS: {response.Success} \\\\ PASSED: {response.Passed}", script);
                if (response.Success)
                {
                    if (response.Passed)
                        break;
                }
                else
                {
                    Logger.Warn($"WaitUntilDebug condition error: {response.Message}", script);
                    break;
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
