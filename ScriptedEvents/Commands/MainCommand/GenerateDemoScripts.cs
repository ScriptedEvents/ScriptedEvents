namespace ScriptedEvents.Commands.MainCommand
{
    using System;

    using CommandSystem;
    using ScriptedEvents.API.Modules;

    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class GenerateDemoScripts : ICommand
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