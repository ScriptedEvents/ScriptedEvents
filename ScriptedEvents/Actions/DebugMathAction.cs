namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class DebugMathAction : IScriptAction, IHiddenAction
    {
        public string Name => "DEBUGMATH";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute(Script script)
        {
            string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments));
            if (!ConditionHelper.TryMath(formula, out MathResult result))
            {
                return new(MessageType.NotANumberOrCondition, this, "condition", formula, result);
            }

            return new(true, result.Result.ToString());
        }
    }
}