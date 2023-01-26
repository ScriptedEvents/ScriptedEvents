using CommandSystem;
using System;

namespace ScriptedEvents.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ExecuteEvent : ICommand
    {
        public string Command => "executeevent";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Start an event";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            ScriptHelper.RunScript(ScriptHelper.ReadScript(arguments.At(0)));
            response = "Done";
            return true;
        }
    }
}
