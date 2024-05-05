namespace ScriptedEvents.API.Features
{
    using CommandSystem;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;

    public static class LogSystem
    {
        public static void ScriptError(string message, Script source, ExecuteContext executeContext, ICommandSender sender = null, bool fatal = false)
        {
            switch (executeContext)
            {
                case ExecuteContext.RemoteAdmin:
                    Player ply = Player.Get(sender);
                    ply?.RemoteAdminMessage(message, false, MainPlugin.Singleton.Name);

                    if (MainPlugin.Configs.BroadcastIssues)
                        ply?.Broadcast(5, $"{(fatal ? "Fatal e" : "E")}rror when running the <b>{source.ScriptName}</b> script. See text RemoteAdmin for details.");

                    break;
                case ExecuteContext.PlayerConsole:
                    Player ply2 = Player.Get(sender);
                    ply2?.SendConsoleMessage(message, "red");

                    if (MainPlugin.Configs.BroadcastIssues)
                        ply2?.Broadcast(5, $"{(fatal ? "Fatal e" : "E")}rror when running the <b>{source.ScriptName}</b> script. See text console for details.");

                    break;
                default:
                    Log.Warn(message);
                    break;
            }
        }
    }
}
