using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public class DebugMathAction : IAction
    {
        public string Name => "DEBUGMATH";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
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