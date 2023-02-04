using CommandSystem;
using Exiled.API.Features.Pools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Handlers.Commands.MainCommand
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ScriptedEvents : ParentCommand
    {
        public ScriptedEvents() => LoadGeneratedCommands();

        public override string Command => "scriptedevents";

        public override string[] Aliases => new[] { "se", "script" };

        public override string Description => "Parent command for the Scripted Events plugin.";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new ExecuteScript());
            RegisterCommand(new ListScripts());
            RegisterCommand(new StopAllScripts());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder sb = StringBuilderPool.Pool.Get();

            sb.AppendLine("Available commands:");
            sb.AppendLine("- SCRIPT EXECUTE <SCRIPTNAME> - Executes the script with the given name.");
            sb.AppendLine("- SCRIPT LIST - Lists all scripts and their respective permissions & last edited time.");
            sb.AppendLine("- SCRIPT STOPALL - Stops all currently executing scripts.");

            response = StringBuilderPool.Pool.ToStringReturn(sb);
            return false;
        }
    }
}
