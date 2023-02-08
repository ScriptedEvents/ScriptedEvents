﻿using Exiled.API.Features;
using Exiled.API.Features.Pools;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class HelpAction : IScriptAction, IHelpInfo
    {
        public string Name => "HELP";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Gets information about a command.";

        public Argument[] ExpectedArguments => new[] { new Argument("action", typeof(string), "The name of the action. Case-sensitive.", true) };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(false, "Missing argument: Action name");

            string actionString = Arguments[0];
            if (!ScriptHelper.ActionTypes.TryGetValue(actionString, out Type type))
                return new(false, $"Invalid action: {actionString}");

            IAction action = Activator.CreateInstance(type) as IAction;

            if (action is not IHelpInfo helpInfo)
                return new(false, "The command provided is not supported in the HELP action.");

            StringBuilder sb = StringBuilderPool.Pool.Get();

            if (helpInfo.ExpectedArguments.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"{action.Name}: {helpInfo.Description}\n\nArguments:");
            }

            foreach (var arg in helpInfo.ExpectedArguments)
            {
                string[] chars = arg.Required ? new[] { "<", ">" } : new[] { "[", "]" };
                sb.AppendLine();
                sb.AppendLine($"{chars[0]}{arg.ArgumentName}{chars[1]}");
                sb.AppendLine($"  Required: {(arg.Required ? "YES" : "NO")}");
                sb.AppendLine($"  Type: {arg.TypeString}");
                sb.AppendLine($"  {arg.Description}");
            }

            Log.Info(StringBuilderPool.Pool.ToStringReturn(sb));
            return new(true);
        }
    }
}