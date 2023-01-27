using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public class BroadcastAction : IAction
    {
        public string Name => "BROADCAST";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 2) return new(false, "Missing argument: duration, message");

            string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments));
            float duration;

            try
            {
                duration = (float)ConditionHelper.Math(formula);
            }
            catch (Exception ex)
            {
                return new(false, $"Invalid duration condition provided! Condition: {formula} Error type: '{ex.GetType().Name}' Message: '{ex.Message}'.");
            }

            string message = string.Join(" ", Arguments.Skip(1));
            Map.Broadcast((ushort)duration, message);
            return new(true);
        }
    }
}