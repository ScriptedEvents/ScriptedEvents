namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Helpers;

    public class DebugMathAction : IScriptAction, IHiddenAction
    {
        public string Name => "DEBUGMATH";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute(Script script)
        {
            try
            {
                float result = (float)ConditionHelper.Math(string.Join(" ", Arguments));
                return new(true, result.ToString());
            }
            catch (Exception ex)
            {
                return new(false, $"Invalid math expression provided. Error type: '{ex.GetType().Name}' Message: '{ex.Message}'.");
            }
        }
    }
}