using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Handlers.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class SaveVariableAction : IScriptAction, IHelpInfo
    {
        public string Name => "SAVEVARIABLE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Saves a new variable. Saved variables can be used in ANY script, and are reset when the round ends.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(string), "The name of the new variable. Braces will be added automatically if not provided.", true),
            new Argument("value", typeof(object), "The value to store. Variables & Math are supported.", true)
        };

        public ActionResponse Execute(Script scr)
        {
            if (Arguments.Length < 2)
            {
                return new(false, "Missing arguments: variableName, value");
            }

            string varName = Arguments[0];
            string input = string.Join(" ", Arguments.Skip(1));

            input = ConditionVariables.ReplaceVariables(input);

            try
            {
                float value = (float)ConditionHelper.Math(input);
                ConditionVariables.DefineVariable(varName, value);
                return new(true);
            }
            catch { }
            

            ConditionVariables.DefineVariable(varName, input);

            return new(true);
        }
    }
}
