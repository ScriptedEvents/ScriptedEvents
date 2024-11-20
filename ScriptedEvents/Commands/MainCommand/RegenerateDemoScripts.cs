using System;
using CommandSystem;
using ScriptedEvents.API.Modules;

namespace ScriptedEvents.Commands.MainCommand
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class RegenerateDemoScripts : ICommand
    {
        /// <inheritdoc/>
        public string Command => "demoscripts";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "demo" };

        /// <inheritdoc/>
        public string Description => "Regenerates all demo scripts.";

        /// <inheritdoc/>
        public bool SanitizeResponse => true;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            ScriptModule.GenerateDemoScripts();
            response = "Demo scripts regenerated.";
            return true;
        }
    }
}