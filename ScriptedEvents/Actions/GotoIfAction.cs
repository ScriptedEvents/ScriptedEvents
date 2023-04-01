namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;

    public class GotoIfAction : IScriptAction, ILogicAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "GOTOIF";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Reads the condition and jumps to the first provided line if the condition is TRUE, or the second provided line if the condition is FALSE.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("trueLine", typeof(int), "The line to jump to if the condition is TRUE. Variables & Math are NOT supported.", true),
            new Argument("falseLine", typeof(int), "The line to jump to if the condition iS FALSE. Variables & Math are NOT supported.", true),
            new Argument("condition", typeof(string), "The condition to check. Variables & Math are supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!int.TryParse(Arguments[0], out int trueLine) && !script.Labels.TryGetValue(Arguments[0], out trueLine))
                return new(false, "trueLine is not a valid integer.");

            if (!int.TryParse(Arguments[1], out int falseLine) && !script.Labels.TryGetValue(Arguments[1], out falseLine))
                return new(false, "falseLine is not a valid integer.");

            ConditionResponse outcome = ConditionHelper.Evaluate(string.Join(string.Empty, Arguments.Skip(2)), script);
            if (!outcome.Success)
                return new(false, $"IF execution error: {outcome.Message}", ActionFlags.FatalError);

            if (outcome.Passed)
            {
                script.DebugLog($"GOTOIF result: true. Jumping to line {trueLine}.");
                script.Jump(trueLine);
            }
            else
            {
                script.DebugLog($"GOTOIF result: false. Jumping to line {falseLine}.");
                script.Jump(falseLine);
            }

            return new(true);
        }
    }
}