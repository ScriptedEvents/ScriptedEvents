namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
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
            new Argument("trueLine", typeof(string), "The line to jump to if the condition is TRUE. Variables & Math are NOT supported.", true),
            new Argument("falseLine", typeof(string), "The line to jump to if the condition is FALSE. Variables & Math are NOT supported.", true),
            new Argument("condition", typeof(string), "The condition to check. Variables & Math are supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            ConditionResponse outcome = ConditionHelper.Evaluate(string.Join(string.Empty, Arguments.Skip(2)), script);
            if (!outcome.Success)
                return new(false, $"IF execution error: {outcome.Message}", ActionFlags.FatalError);

            if (outcome.Passed)
            {
                script.DebugLog($"GOTOIF result: true. Jumping to line {Arguments[0]}.");
                if (!script.Jump(Arguments[0]))
                {
                    if (Arguments[0].ToUpper() == "STOP")
                        return new(true, flags: ActionFlags.StopEventExecution);

                    return new(false, $"Failed to jump to trueLine '{Arguments[0]}'. trueLine must be an integer, a label, or a keyword.");
                }
            }
            else
            {
                script.DebugLog($"GOTOIF result: false. Jumping to line {Arguments[1]}.");
                if (!script.Jump(Arguments[1]))
                {
                    if (Arguments[1].ToUpper() == "STOP")
                        return new(true, flags: ActionFlags.StopEventExecution);

                    return new(false, $"Failed to jump to falseLine '{Arguments[1]}'. falseLine must be an integer, a label, or a keyword.");
                }
            }

            return new(true);
        }
    }
}