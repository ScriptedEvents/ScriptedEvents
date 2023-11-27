namespace ScriptedEvents.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Features.Exceptions;

    public class CustomCommand : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; set; }

        /// <inheritdoc/>
        public string[] Aliases { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of command this custom command is.
        /// </summary>
        public CommandType Type { get; set; }

        /// <summary>
        /// Gets or sets the cooldown mode of the command.
        /// </summary>
        public CommandCooldownMode CooldownMode { get; set; }

        /// <summary>
        /// Gets or sets the command cooldown length.
        /// </summary>
        public int Cooldown { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="string"/> array of scripts to run when this command is executed.
        /// </summary>
        public string[] Scripts { get; set; }

        /// <summary>
        /// Gets or sets the permission required to execute this command.
        /// </summary>
        public string Permission { get; set; }

        private DateTime globalCooldown;
        private Dictionary<string, DateTime> playerCooldown = new();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Permission != string.Empty && !sender.CheckPermission(Permission))
            {
                response = $"Missing permission: {Permission}";
                return false;
            }

            if (CooldownMode == CommandCooldownMode.Global)
            {
                if ((DateTime.UtcNow - globalCooldown).TotalSeconds < Cooldown)
                {
                    int cooldownLeft = (int)(Cooldown - (DateTime.UtcNow - globalCooldown).TotalSeconds);
                    response = $"This command is on cooldown and can be used in {cooldownLeft} second{(cooldownLeft != 1 ? "s" : string.Empty)}.";
                    return false;
                }

                globalCooldown = DateTime.UtcNow;
            }

            if (CooldownMode == CommandCooldownMode.Player && Player.TryGet(sender, out Player ply))
            {
                Log.Info(1);
                if (playerCooldown.ContainsKey(ply.UserId) && (DateTime.UtcNow - playerCooldown[ply.UserId]).TotalSeconds < Cooldown)
                {
                    Log.Info(2);
                    int cooldownLeft = (int)(Cooldown - (DateTime.UtcNow - playerCooldown[ply.UserId]).TotalSeconds);
                    response = $"This command is on cooldown and can be used in {cooldownLeft} second{(cooldownLeft != 1 ? "s" : string.Empty)}.";
                    return false;
                }

                playerCooldown[ply.UserId] = DateTime.UtcNow;
            }

            Dictionary<string, string> failed = new();
            int success = 0;

            foreach (string scr in Scripts)
            {
                try
                {
                    Script body = ScriptHelper.ReadScript(scr, sender);

                    if (sender is PlayerCommandSender playerSender && Player.TryGet(playerSender, out Player plr))
                    {
                        body.AddPlayerVariable("{SENDER}", "The player who executed the script.", new[] { plr });
                    }

                    for (int i = 0; i < 20; i++)
                    {
                        if (arguments.Count < i + 1)
                            break;

                        body.AddVariable($"{{ARG{i + 1}}}", $"Argument #{i + 1} of the command.", arguments.At(i).ToString());
                    }

                    body.AddVariable("{ARGS}", "All arguments of the command, separated by spaces.", string.Join(" ", arguments));

                    ScriptHelper.RunScript(body);
                    success++;
                }
                catch (DisabledScriptException)
                {
                    failed.Add(scr, "Script is disabled!");
                }
                catch (FileNotFoundException)
                {
                    failed.Add(scr, "Script not found!");
                }
            }

            if (failed.Count > 0)
            {
                response = $"Successfully ran {success} scripts. {failed.Count} failed scripts:";
                foreach (var kvp in failed)
                {
                    response += $"\n{kvp.Key} - {kvp.Value}";
                }

                return false;
            }

            response = $"Successfully ran {success} scripts.";
            return true;
        }
    }
}
