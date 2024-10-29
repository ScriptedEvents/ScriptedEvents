using ScriptedEvents.API.Features;
using ScriptedEvents.Enums;

namespace ScriptedEvents.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    using RemoteAdmin;
    using ScriptedEvents.API.Features.Exceptions;

    public class CustomCommand : ICommand
    {
        private DateTime globalCooldown;
        private Dictionary<string, DateTime> playerCooldown = new();

        /// <inheritdoc/>
        public string Command { get; set; }

        /// <inheritdoc/>
        public bool SanitizeResponse => true;

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
        /// Gets or sets a value indicating whether or not to do default response.
        /// </summary>
        public bool DoResponse { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="string"/> array of scripts to run when this command is executed.
        /// </summary>
        public string[] Scripts { get; set; }

        /// <summary>
        /// Gets or sets the permission required to execute this command.
        /// </summary>
        public string Permission { get; set; }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Permission != string.Empty && !sender.CheckPermission(Permission))
            {
                response = MainPlugin.Translations.MissingPermission.Replace("{PERMISSION}", Permission);
                return false;
            }

            switch (CooldownMode)
            {
                case CommandCooldownMode.Global when (DateTime.Now - globalCooldown).TotalSeconds < Cooldown:
                {
                    int cooldownLeft = (int)(Cooldown - (DateTime.Now - globalCooldown).TotalSeconds);
                    response = cooldownLeft == 1
                        ? MainPlugin.Translations.CommandCooldown.Replace("{SECONDS}", cooldownLeft.ToString())
                        : MainPlugin.Translations.CommandCooldownSingular.Replace("{SECONDS}", cooldownLeft.ToString());
                    return false;
                }

                case CommandCooldownMode.Global:
                    globalCooldown = DateTime.Now;
                    break;

                case CommandCooldownMode.Player when Player.TryGet(sender, out Player ply):
                {
                    if (playerCooldown.ContainsKey(ply.UserId) && (DateTime.Now - playerCooldown[ply.UserId]).TotalSeconds < Cooldown)
                    {
                        int cooldownLeft = (int)(Cooldown - (DateTime.Now - playerCooldown[ply.UserId]).TotalSeconds);
                        response = cooldownLeft == 1
                            ? MainPlugin.Translations.CommandCooldown.Replace("{SECONDS}", cooldownLeft.ToString())
                            : MainPlugin.Translations.CommandCooldownSingular.Replace("{SECONDS}", cooldownLeft.ToString());
                        return false;
                    }

                    playerCooldown[ply.UserId] = DateTime.Now;
                    break;
                }
            }

            Dictionary<string, string> failed = new();
            int success = 0;

            foreach (string scrName in Scripts)
            {
                try
                {
                    if (!MainPlugin.ScriptModule.TryParseScript(scrName, sender, out var script, out var err) || script == null)
                    {
                        Logger.Error(err!);
                        continue;
                    }

                    // Override default script context for custom commands
                    switch (Type)
                    {
                        case CommandType.PlayerConsole:
                            script.Context = ExecuteContext.PlayerConsole;
                            break;
                        case CommandType.ServerConsole:
                            script.Context = ExecuteContext.ServerConsole;
                            break;
                        case CommandType.RemoteAdmin:
                            script.Context = ExecuteContext.RemoteAdmin;
                            break;
                    }

                    if (sender is PlayerCommandSender playerSender && Player.TryGet(playerSender, out Player plr))
                    {
                        script.AddPlayerVariable("@SENDER", new[] { plr }, true);
                    }

                    for (int i = 1; i < 20; i++)
                    {
                        if (arguments.Count < i)
                            break;

                        script.AddLiteralVariable($"$ARG{i}", arguments.At(i - 1), true);
                    }

                    script.AddLiteralVariable("$ARGS", string.Join(" ", arguments), true);

                    MainPlugin.ScriptModule.TryRunScript(script, out var err1);
                    if (err1 != null)
                    {
                        Logger.Error(err1);
                    }
                    else
                    {
                        success++;
                    }
                }
                catch (DisabledScriptException)
                {
                    failed.Add(scrName, MainPlugin.Translations.DisabledScript);
                }
                catch (FileNotFoundException)
                {
                    failed.Add(scrName, MainPlugin.Translations.MissingScript);
                }
            }

            if (failed.Count > 0)
            {
                string failList = string.Empty;
                foreach (var kvp in failed)
                {
                    failList += $"\n{kvp.Key} - {kvp.Value}";
                }

                response = MainPlugin.Translations.CommandSuccessWithFailure
                    .Replace("{SUCCESSAMOUNT}", success.ToString())
                    .Replace("{FAILAMOUNT}", failed.Count.ToString())
                    .Replace("{FAILED}", failList);
                return false;
            }

            if (DoResponse)
                response = MainPlugin.Translations.CommandSuccess.Replace("{SUCCESSAMOUNT}", success.ToString());
            else
                response = string.Empty;
            return true;
        }

        public void ResetCooldowns()
        {
            playerCooldown.Clear();
            globalCooldown = default;
        }
    }
}
