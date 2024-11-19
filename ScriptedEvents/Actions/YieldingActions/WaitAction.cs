using System;
using MEC;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.YieldingActions
{
    public class WaitAction : ITimingAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Wait";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Yielding;

        /// <inheritdoc/>
        public string Description => "Yields the script execution for a specified amount of time.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("time", typeof(TimeSpan), "The time to wait.", true),
        };

        /// <inheritdoc/>
        public float? Execute(Script script, out ActionResponse message)
        {
            message = new(true);
            return Timing.WaitForSeconds((float)((TimeSpan)Arguments[0]!).TotalSeconds);
        }
    }
}
