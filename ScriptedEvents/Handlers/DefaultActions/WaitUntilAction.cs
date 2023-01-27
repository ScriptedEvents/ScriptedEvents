using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class WaitUntilAction : ITimingAction
    {
        public static List<string> Coroutines { get; } = new();
        public string Name => "WAITUNTIL";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            return new(true);
        }

        private IEnumerator<float> InternalWaitUntil(string input)
        {
            while (true)
            {
                var response = ConditionHelper.Evaluate(input);
                if (response.Success)
                {
                    if (response.Passed)
                        break;
                }
                else
                {
                    Log.Warn($"[WAITUNTIL] WaitUntil condition error: {response.Message}");
                    break;
                }
                yield return Timing.WaitForSeconds(1f);
            }
        }

        public float? GetDelay(out ActionResponse message)
        {
            if (Arguments.Length < 1)
            {
                message = new(false, "Missing argument: condition");
                return null;
            }

            string coroutineKey = $"WAITUNTIL_COROUTINE_{DateTime.UtcNow.Ticks}";
            Coroutines.Add(coroutineKey);
            message = new(true);
            return Timing.WaitUntilDone(InternalWaitUntil(string.Join("", Arguments)), coroutineKey);
        }
    }
}
