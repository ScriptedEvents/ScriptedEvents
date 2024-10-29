using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;

    using MEC;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class WaitAction : ITimingAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "WAIT";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Yielding;

        /// <inheritdoc/>
        public string Description => "Yields execution of the script.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("SEC", "'value' argument will expect amout of seconds to wait for."),
                new("MIL", "'value' argument will expect amout of miliseconds to wait for. "),
                new("UNTIL", "'value argument will expect a condition. Will yield until said condition evaluates to TRUE.")),
            new Argument("value", typeof(object), "The value. Math is supported.", true),
        };

        /// <inheritdoc/>
        public float? Execute(Script script, out ActionResponse message)
        {
            string formula = Arguments.JoinMessage(1);

            if (Arguments[0].ToUpper() == "UNTIL")
            {
                string coroutineKey = $"WAITUNTIL_COROUTINE_{DateTime.Now.Ticks}";
                CoroutineHandle handle = Timing.RunCoroutine(InternalWaitUntil(script, RawArguments.JoinMessage(1)), coroutineKey);
                CoroutineHelper.AddCoroutine("WAITUNTIL", handle, script);

                message = new(true);
                return Timing.WaitUntilDone(handle);
            }

            if (!ConditionHelper.TryMath(formula, out MathResult result))
            {
                message = new(false, "diot");
                return null;
            }

            if (result.Result < 0)
            {
                message = new(false, "diot");
                return null;
            }

            message = new(true);
            return Timing.WaitForSeconds(Arguments[0].ToUpper() == "MIL" ? result.Result / 1000 : result.Result);
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
