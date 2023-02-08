using Exiled.API.Features;
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

        public Argument[] ExpectedArguments => new[] { new Argument("action", "The name of the action. Case-sensitive.", true) };

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

            sb.AppendLine($"{action.Name}: {helpInfo.Description}\n\nArguments:");

            foreach (var arg in helpInfo.ExpectedArguments)
            {
                sb.AppendLine($"{arg.ArgumentName}{(arg.Required ? " [R]" : string.Empty)}");
                sb.AppendLine($"\t{arg.Description}");
            }

            Log.Info(StringBuilderPool.Pool.ToStringReturn(sb));
            return new(true);
        }
    }
}
