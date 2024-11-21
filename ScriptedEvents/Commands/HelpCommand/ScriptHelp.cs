using ScriptedEvents.DocumentationGeneration;

namespace ScriptedEvents.Commands.HelpCommand
{
    using System;
    using System.Linq;

    using CommandSystem;
    using ScriptedEvents.Actions;
    using ScriptedEvents.Structures;

    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ScriptHelp : ICommand
    {
        /// <inheritdoc/>
        public string Command => "shelp";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => "Get documentation for most things in Scripted Events.";
        
        public bool SanitizeResponse => true;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = DocsGen.GenerateVariableList();
            return true;
        }
    }
}
