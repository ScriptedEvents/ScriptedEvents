namespace ScriptedEvents.API.Features
{
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    using LogInternal = Exiled.API.Features.Log;

    public static class Logger
    {
        public static void Log(string message, LogType logType, Script? script = null, int? printableLine = null, bool addErrorContext = true)
        {
            if (MainPlugin.Configs.DisableAllLogs)
                return;

            if (script is not null && addErrorContext)
                message = $"[Script: {script.ScriptName}] [Line: {printableLine ?? script.CurrentLine + 1}] {message}";

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
                    if (script is not null && script.IsDebug)
                        script.DebugLog(message);
                    else
                        LogInternal.Debug(message);
                    break;
                default:
                    break;
            }
        }

        public static void Info(string message, Script? source = null) => Log(message, LogType.Info, source);

        public static void Warn(string message, Script? source = null) => Log(message, LogType.Warning, source);

        public static void Warn(ErrorTrace trace, Script? source = null) => Log($"\n{trace.Format()}", LogType.Warning, source);

        public static void Error(string message, Script? source = null) => Log(message, LogType.Error, source);

        public static void Error(ErrorTrace trace, Script? source = null, int? printableLine = null) => Log($"\n{trace.Format()}", LogType.Error, source, printableLine);

        public static void Debug(string message, Script? source = null) => Log(message, LogType.Debug, source);

        public static void ScriptError(string message, Script source, bool fatal = false, int? printableLine = null)
        {
            var formattedMessage = $"[Script: {source.ScriptName}] [Line: {printableLine ?? source.CurrentLine + 1}] " + message;
            var broadcastMessage = $"<b><size=40>{(fatal ? "Fatal e" : "E")}rror when running the '{source.ScriptName}' script.\n<size=30>See the console for more details.";
            var sender = source.Sender;

            switch (source.Context)
            {
                case ExecuteContext.RemoteAdmin:
                    var ply = Player.Get(sender);
                    ply?.RemoteAdminMessage(formattedMessage, false, MainPlugin.Singleton.Name);

                    if (MainPlugin.Configs.BroadcastIssues)
                        ply?.Broadcast(5, broadcastMessage);

                    break;

                case ExecuteContext.PlayerConsole:
                    var ply2 = Player.Get(sender);
                    ply2?.SendConsoleMessage(formattedMessage, "red");

                    if (MainPlugin.Configs.BroadcastIssues)
                        ply2?.Broadcast(5, broadcastMessage);

                    break;

                case ExecuteContext.Automatic:
                case ExecuteContext.ServerConsole:
                case ExecuteContext.None:
                default:
                    Log(formattedMessage, LogType.Error, source, null, false);
                    break;
            }
        }

        public static void ScriptError(ErrorTrace trace, Script source, bool fatal = false, int? printableLine = null)
        {
            var error = $"[Script: {source.ScriptName}] [Line: {printableLine ?? source.CurrentLine + 1}]\n{trace.Format()}";
            var broadcastMessage = $"<b><size=40>{(fatal ? "Fatal e" : "E")}rror when running the '{source.ScriptName}' script.</size></b>\n<size=30>See the console for more details.</size>";

            switch (source.Context)
            {
                case ExecuteContext.RemoteAdmin:
                    var ply = Player.Get(source.Sender);
                    ply?.RemoteAdminMessage(error, false, MainPlugin.Singleton.Name);

                    if (MainPlugin.Configs.BroadcastIssues)
                        ply?.Broadcast(5, broadcastMessage);

                    break;

                case ExecuteContext.PlayerConsole:
                    var ply2 = Player.Get(source.Sender);
                    ply2?.SendConsoleMessage(error, "red");

                    if (MainPlugin.Configs.BroadcastIssues)
                        ply2?.Broadcast(5, broadcastMessage);

                    break;

                case ExecuteContext.Automatic:
                case ExecuteContext.ServerConsole:
                case ExecuteContext.None:
                default:
                    Log(error, LogType.Error, source, null, false);
                    break;
            }
        }
    }
}
