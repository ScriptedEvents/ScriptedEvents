using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using ScriptedEvents.API.Features;

namespace ScriptedEvents.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class SeShowTag : ICommand
    {
        /// <inheritdoc/>
        public string Command => "seshowtag";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "sstag", "sst" };

        /// <inheritdoc/>
        public string Description => "Shows a custom SE rank if youre an SE contributor.";
        
        public bool SanitizeResponse => true;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is PlayerCommandSender player)
            {
                bool res = CreditHandler.AddCreditTagIfApplicable(Player.List.FirstOrDefault(p => p.Id == player.PlayerId));
                response = "Applying tag " + (res ? "successful!" : "failed, youre not a contributor or you already have a different rank.");
                return true;
            }
            
            response = "You must be a player to use this command.";
            return false;
        }
    }
}
