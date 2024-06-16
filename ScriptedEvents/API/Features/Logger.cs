namespace ScriptedEvents.API.Features
{
    using CommandSystem;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;

    using LogInternal = Exiled.API.Features.Log;

    public static class Logger
    {
        public static void Log(string message, LogType logType, Script script = null)
        {
            if (MainPlugin.Configs.DisableAllLogs)
                return;

            if (script is not null)
                message = $"[Script: {script.ScriptName}] [L: {script.CurrentLine + 1}] {message}";

            switch (logType)
            {
                case LogType.Error:
                    LogInternal.Error(message);
                    break;
                case LogType.Warning:
                    LogInternal.Warn(message);
                    break;
                case LogType.Info:
                    LogInternal.Info(message);
                    break;
                case LogType.Debug:
                    if (script is not null)
                        script.DebugLog(message);
                    else
                        LogInternal.Debug(message);
                    break;
            }
        }

        public static void Info(string message, Script source = null) => Log(message, LogType.Info, source);

        public static void Warn(string message, Script source = null) => Log(message, LogType.Warning, source);

        public static void Error(string message, Script source = null) => Log(message, LogType.Error, source);

        public static void Debug(string message, Script source = null) => Log(message, LogType.Debug, source);

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
                    Warn(message);
                    break;
            }
        }
    }
}
