namespace ScriptedEvents.API.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using CommandSystem;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Interactables.Interobjects;
    using MEC;
    using PlayerRoles;
    using RemoteAdmin;
    using ScriptedEvents.Actions;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features.Aliases;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;
    using Random = UnityEngine.Random;

    /// <summary>
    /// A helper class to read and execute scripts, and register actions, as well as providing useful API for individual actions.
    /// </summary>
    public static class ScriptHelper
    {
        /// <summary>
        /// The base path to the script folder.
        /// </summary>
        public static readonly string ScriptPath = Path.Combine(Paths.Configs, "ScriptedEvents");

        /// <summary>
        /// Gets a dictionary of action names and their respective types.
        /// </summary>
        public static Dictionary<string, Type> ActionTypes { get; } = new();

        /// <summary>
        /// Gets a dictionary of <see cref="Script"/> that are currently running, and the <see cref="CoroutineHandle"/> that is running them.
        /// </summary>
        public static Dictionary<Script, CoroutineHandle> RunningScripts { get; } = new();

        public static Dictionary<string, CustomAction> CustomActions { get; } = new();

        /// <summary>
        /// Reads and returns the text of a script.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <returns>The contents of the script, if it is found.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script is not found.</exception>
        public static string ReadScriptText(string scriptName) => InternalRead(scriptName, out _);

        /// <summary>
        /// Returns the file path of a script.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <returns>The directory of the script, if it is found.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script is not found.</exception>
        public static string GetFilePath(string scriptName)
        {
            InternalRead(scriptName, out string path);
            return path;
        }

        /// <summary>
        /// Reads a script line-by-line, converting every line into an appropriate action, flag, label, etc. Fills out all data and returns a <see cref="Script"/> object.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <param name="executor">The CommandSender that ran the script. Can be null.</param>
        /// <param name="suppressWarnings">Do not show warnings in the console.</param>
        /// <returns>The <see cref="Script"/> fully filled out, if the script was found.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script is not found.</exception>
        public static Script ReadScript(string scriptName, ICommandSender executor, bool suppressWarnings = false)
        {
            string allText = ReadScriptText(scriptName);
            Script script = new();

            List<IAction> actionList = ListPool<IAction>.Pool.Get();

            string[] array = allText.Split('\n');
            for (int currentline = 0; currentline < array.Length; currentline++)
            {
                // NoAction
                string action = array[currentline];
                if (string.IsNullOrWhiteSpace(action))
                {
                    actionList.Add(new NullAction("BLANK LINE"));
                    continue;
                }
                else if (action.StartsWith("#"))
                {
                    actionList.Add(new NullAction("COMMENT"));
                    continue;
                }
                else if (action.StartsWith("!--"))
                {
                    string flag = action.Substring(3).RemoveWhitespace();

                    if (!script.Flags.Contains(flag))
                        script.Flags.Add(flag);
                    else if (!suppressWarnings)
                        Log.Warn($"Multiple definitions for the '{flag}' flag detected in script {scriptName}.");

                    actionList.Add(new NullAction("FLAG DEFINE"));
                    continue;
                }

                string[] actionParts = action.Split(' ');
                string keyword = actionParts[0].RemoveWhitespace();

                // Labels
                if (keyword.EndsWith(":"))
                {
                    string labelName = action.Remove(keyword.Length - 1, 1).RemoveWhitespace();

                    if (!script.Labels.ContainsKey(labelName))
                        script.Labels.Add(labelName, currentline);
                    else if (!suppressWarnings)
                        Log.Warn($"Multiple definitions for the '{labelName}' label detected in script {scriptName}.");

                    actionList.Add(new NullAction($"{labelName} LABEL"));

                    continue;
                }

                Alias alias = MainPlugin.Singleton.Config.Aliases.Get(keyword);
                if (alias != null)
                {
                    actionParts = alias.Unalias(action).Split(' ');
                    keyword = actionParts[0].RemoveWhitespace();
                }

#if DEBUG
                Log.Debug($"Queuing action {keyword} {string.Join(", ", actionParts.Skip(1))}");
#endif
                ActionTypes.TryGetValue(keyword, out Type actionType);

                if (actionType is null && alias == null)
                {
                    // Check for custom actions
                    if (CustomActions.TryGetValue(keyword, out CustomAction customAction))
                    {
                        CustomAction customAction1 = new(customAction.Name, customAction.Action);
                        customAction1.Arguments = actionParts.Skip(1).Select(str => str.RemoveWhitespace()).ToArray();
                        actionList.Add(customAction1);
                        continue;
                    }

                    if (!suppressWarnings)
                        Log.Warn($"Invalid action '{keyword.RemoveWhitespace()}' detected in script '{scriptName}'.");
                    actionList.Add(new NullAction("ERROR"));
                    continue;
                }

                IAction newAction = Activator.CreateInstance(actionType) as IAction;
                newAction.Arguments = actionParts.Skip(1).Select(str => str.RemoveWhitespace()).ToArray();

                actionList.Add(newAction);
            }

            string scriptPath = GetFilePath(scriptName);

            // Fill out script data
            if (MainPlugin.Singleton.Config.RequiredPermissions.TryGetValue(scriptName, out string perm2))
            {
                script.ReadPermission += $".{perm2}";
                script.ExecutePermission += $".{perm2}";
            }

            script.ScriptName = scriptName;
            script.RawText = allText;
            script.FilePath = scriptPath;
            script.LastRead = File.GetLastAccessTimeUtc(scriptPath);
            script.LastEdited = File.GetLastWriteTimeUtc(scriptPath);
            script.Actions = ListPool<IAction>.Pool.ToArrayReturn(actionList);

            if (executor is null)
            {
                script.Context = ExecuteContext.Automatic;
            }
            else if (executor is ServerConsoleSender console)
            {
                script.Context = ExecuteContext.ServerConsole;
                script.Sender = console;
            }
            else if (executor is PlayerCommandSender player)
            {
                script.Context = ExecuteContext.RemoteAdmin;
                script.Sender = player;
            }

            script.DebugLog($"Debug script read successfully. Name: {script.ScriptName} | Actions: {string.Join(" ", script.Actions.Length)} | Flags: {string.Join(" ", script.Flags)} | Labels: {string.Join(" ", script.Labels)} | Comments: {script.Actions.Where(action => action is NullAction @null && @null.Type is "COMMENT").Count()}");

            return script;
        }

        /// <summary>
        /// Runs the script and disposes of it as soon as the script execution is complete.
        /// </summary>
        /// <param name="scr">The script to run.</param>
        /// <exception cref="DisabledScriptException">If <see cref="Script.Disabled"/> is <see langword="true"/>.</exception>
        public static void RunScript(Script scr)
        {
            if (scr.Disabled)
                throw new DisabledScriptException(scr.ScriptName);

            CoroutineHandle handle = Timing.RunCoroutine(RunScriptInternal(scr), $"SCRIPT_{scr.UniqueId}");
            RunningScripts.Add(scr, handle);
        }

        /// <summary>
        /// Reads and runs a script.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <param name="executor">The executor that is running the script. Can be null.</param>
        /// <exception cref="FileNotFoundException">The script was not found.</exception>
        /// <exception cref="DisabledScriptException">If <see cref="Script.Disabled"/> is <see langword="true"/>.</exception>
        public static void ReadAndRun(string scriptName, ICommandSender executor)
        {
            Script scr = ReadScript(scriptName, executor);
            if (scr is not null)
                RunScript(scr);
        }

        /// <summary>
        /// Converts an input into a list of players.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="amount">The maximum amount of players to give back, or <see langword="null"/> for unlimited.</param>
        /// <param name="plys">The list of players.</param>
        /// <param name="source">The script using the API. Used for per-script variables.</param>
        /// <returns>Whether or not any players were found.</returns>
        public static bool TryGetPlayers(string input, int? amount, out Player[] plys, Script source = null)
        {
            input = input.RemoveWhitespace();
            List<Player> list = ListPool<Player>.Pool.Get();
            if (input.ToUpper() is "*" or "ALL")
            {
                plys = Player.List.ToArray();
                ListPool<Player>.Pool.Return(list);
                return true;
            }
            else
            {
                string[] variables = ConditionHelper.IsolateVariables(input);
                foreach (string variable in variables)
                {
                    if (PlayerVariables.TryGet(variable, out IEnumerable<Player> playersFromVariable, source))
                    {
                        list.AddRange(playersFromVariable);
                    }
                }

                if (Player.TryGet(input, out Player ply))
                {
                    list.Add(ply);
                }
            }

            list.ShuffleList();
            list.RemoveAll(p => !p.IsConnected);

            if (MainPlugin.Configs.IgnoreOverwatch)
                list.RemoveAll(p => p.Role.Type is RoleTypeId.Overwatch);

            if (amount.HasValue && amount.Value > 0)
            {
                while (list.Count > amount.Value)
                {
                    list.PullRandomItem();
                }
            }

            plys = ListPool<Player>.Pool.ToArrayReturn(list);
            return plys.Length > 0;
        }

        public static bool TryGetDoors(string input, out Door[] doors)
        {
            List<Door> doorList = ListPool<Door>.Pool.Get();
            if (input is "*" or "ALL")
            {
                doorList = Door.List.ToList();
            }
            else if (Enum.TryParse<ZoneType>(input, true, out ZoneType zt))
            {
                doorList = Door.List.Where(d => d.Zone.HasFlag(zt)).ToList();
            }
            else if (Enum.TryParse<DoorType>(input, true, out DoorType dt))
            {
                doorList = Door.List.Where(d => d.Type == dt).ToList();
            }
            else if (Enum.TryParse<RoomType>(input, true, out RoomType rt))
            {
                doorList = Door.List.Where(d => d.Room?.Type == rt).ToList();
            }
            else
            {
                doorList = Door.List.Where(d => d.Name.ToLower() == input.ToLower()).ToList();
            }

            doorList = doorList.Where(d => d.IsElevator is false && d.Type is not DoorType.Scp914Door && d.Type is not DoorType.Scp079First && d.Type is not DoorType.Scp079Second && d.Base.GetComponentInParent<AirlockController>() == null).ToList();
            doors = ListPool<Door>.Pool.ToArrayReturn(doorList);
            return doors.Length > 0;
        }

        public static bool TryGetRooms(string input, out Room[] rooms)
        {
            List<Room> roomList = ListPool<Room>.Pool.Get();
            if (input is "*" or "ALL")
            {
                roomList = Room.List.ToList();
            }
            else if (Enum.TryParse<ZoneType>(input, true, out ZoneType zt))
            {
                roomList = Room.List.Where(room => room.Zone.HasFlag(zt)).ToList();
            }
            else if (Enum.TryParse<RoomType>(input, true, out RoomType rt))
            {
                roomList = Room.List.Where(d => d.Type == rt).ToList();
            }
            else
            {
                roomList = Room.List.Where(d => d.Name.ToLower() == input.ToLower()).ToList();
            }

            rooms = ListPool<Room>.Pool.ToArrayReturn(roomList);
            return rooms.Length > 0;
        }

        public static int StopAllScripts()
        {
            int amount = 0;
            foreach (KeyValuePair<Script, CoroutineHandle> kvp in RunningScripts)
            {
                amount++;
                Timing.KillCoroutines(kvp.Value);

                kvp.Key.IsRunning = false;
                kvp.Key.Dispose();
            }

            foreach (string key in WaitUntilAction.Coroutines)
            {
                Timing.KillCoroutines(key);
            }

            foreach (string key in WaitUntilDebugAction.Coroutines)
            {
                Timing.KillCoroutines(key);
            }

            WaitUntilAction.Coroutines.Clear();
            WaitUntilDebugAction.Coroutines.Clear();
            RunningScripts.Clear();
            return amount;
        }

        /// <summary>
        /// Reads a script.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <param name="fileDirectory">The directory of the script, if it is found.</param>
        /// <returns>The contents of the script, if it is found.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script is not found.</exception>
        internal static string InternalRead(string scriptName, out string fileDirectory)
        {
            fileDirectory = null;

            string text = null;
            string mainFolderFile = Path.Combine(ScriptPath, scriptName + ".txt");
            if (File.Exists(mainFolderFile))
            {
                fileDirectory = mainFolderFile;
                text = File.ReadAllText(mainFolderFile);
            }

            foreach (string directory in Directory.GetDirectories(ScriptPath))
            {
                string fileName = Path.Combine(directory, scriptName + ".txt");
                if (File.Exists(fileName))
                {
                    fileDirectory = fileName;
                    text = File.ReadAllText(fileName);
                }
            }

            if (text is not null && fileDirectory is not null)
            {
                if (text.Contains("!-- HELPRESPONSE"))
                    throw new FileNotFoundException($"Script {scriptName} does not exist.");

                return text;
            }

            throw new FileNotFoundException($"Script {scriptName} does not exist.");
        }

        /// <summary>
        /// Registers all the actions in the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly to register actions in.</param>
        internal static void RegisterActions(Assembly assembly)
        {
            int i = 0;
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IAction).IsAssignableFrom(type) && type.IsClass && type.GetConstructors().Length > 0)
                {
                    if (type == typeof(CustomAction))
                        continue;

                    IAction temp = (IAction)Activator.CreateInstance(type);

                    Log.Debug($"Adding Action: {temp.Name} | From Assembly: {assembly.GetName().Name}");
                    ActionTypes.Add(temp.Name, type);
                    i++;
                }
            }

            MainPlugin.Info($"Assembly '{assembly.GetName().Name}' has registered {i} actions.");
        }

        /// <summary>
        /// Internal coroutine to run the script.
        /// </summary>
        /// <param name="scr">The script to run.</param>
        /// <returns>Coroutine iterator.</returns>
        private static IEnumerator<float> RunScriptInternal(Script scr)
        {
            MainPlugin.Info($"Running script {scr.ScriptName}.");

            scr.IsRunning = true;
            scr.RunDate = DateTime.UtcNow;

            for (; scr.CurrentLine < scr.Actions.Length; scr.NextLine())
            {
                scr.DebugLog("-----------");
                scr.DebugLog($"Current Line: {scr.CurrentLine + 1}");
                if (scr.Actions.TryGet(scr.CurrentLine, out IAction action) && action != null)
                {
                    ActionResponse resp;
                    float? delay = null;

                    try
                    {
                        switch (action)
                        {
                            case ITimingAction timed:
                                scr.DebugLog($"Running {action.Name} action (timed) on line {scr.CurrentLine + 1}...");
                                delay = timed.Execute(scr, out resp);
                                break;
                            case IScriptAction scriptAction:
                                scr.DebugLog($"Running {action.Name} action on line {scr.CurrentLine + 1}...");
                                resp = scriptAction.Execute(scr);
                                break;
                            default:
                                scr.DebugLog($"Skipping line {scr.CurrentLine + 1} (no runnable action on line)");
                                continue;
                        }
                    }
                    catch (Exception e)
                    {
                        string message = $"[Script: {scr.ScriptName}] [L: {scr.CurrentLine + 1}] Ran into an error while running {action.Name} action:\n{e}";
                        switch (scr.Context)
                        {
                            case ExecuteContext.RemoteAdmin:
                                Player ply = Player.Get(scr.Sender);
                                ply.RemoteAdminMessage(message, false, "ScriptedEvents");

                                if (MainPlugin.Configs.BroadcastIssues)
                                    ply?.Broadcast(5, $"Error when running the <b>{scr.ScriptName}</b> script. See text RemoteAdmin for details.");

                                break;
                            default:
                                Log.Error(message);
                                break;
                        }

                        continue;
                    }

                    if (!resp.Success)
                    {
                        scr.DebugLog($"{action.Name} [Line: {scr.CurrentLine + 1}]: FAIL");
                        if (resp.ResponseFlags.HasFlag(ActionFlags.FatalError))
                        {
                            string message = $"[Script: {scr.ScriptName}] [L: {scr.CurrentLine + 1}] [{action.Name}] Fatal action error! {resp.Message}";
                            switch (scr.Context)
                            {
                                case ExecuteContext.RemoteAdmin:
                                    Player ply = Player.Get(scr.Sender);
                                    ply?.RemoteAdminMessage(message, false, "ScriptedEvents");

                                    if (MainPlugin.Configs.BroadcastIssues)
                                        ply?.Broadcast(5, $"Fatal action error when running the <b>{scr.ScriptName}</b> script. See text RemoteAdmin for details.");

                                    break;
                                default:
                                    Log.Error(message);
                                    break;
                            }

                            break;
                        }
                        else if (!scr.SuppressWarnings)
                        {
                            string message = $"[Script: {scr.ScriptName}] [L: {scr.CurrentLine + 1}] [{action.Name}] Action error! {resp.Message}";
                            switch (scr.Context)
                            {
                                case ExecuteContext.RemoteAdmin:
                                    Player ply = Player.Get(scr.Sender);
                                    ply?.RemoteAdminMessage(message, false, "ScriptedEvents");

                                    if (MainPlugin.Configs.BroadcastIssues)
                                        ply?.Broadcast(5, $"Action error when running the <b>{scr.ScriptName}</b> script. See text RemoteAdmin for details.");

                                    break;
                                default:
                                    Log.Warn(message);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        scr.DebugLog($"{action.Name} [Line: {scr.CurrentLine + 1}]: SUCCESS");
                        if (!string.IsNullOrEmpty(resp.Message))
                        {
                            string message = $"[Script: {scr.ScriptName}] [L: {scr.CurrentLine + 1}] [{action.Name}] Response: {resp.Message}";
                            switch (scr.Context)
                            {
                                case ExecuteContext.RemoteAdmin:
                                    Player.Get(scr.Sender)?.RemoteAdminMessage(message, true, "ScriptedEvents");
                                    break;
                                default:
                                    Log.Info(message);
                                    break;
                            }
                        }

                        if (delay.HasValue)
                        {
                            scr.DebugLog($"Action '{action.Name}' on line {scr.CurrentLine + 1} delaying script. Length of delay: {delay.Value}");
                            yield return delay.Value;
                        }
                    }

                    if (resp.ResponseFlags.HasFlag(ActionFlags.StopEventExecution))
                        break;
                }
            }

            scr.DebugLog("-----------");
            MainPlugin.Info($"Finished running script {scr.ScriptName}.");
            scr.IsRunning = false;

            if (MainPlugin.Singleton.Config.LoopScripts.Contains(scr.ScriptName))
            {
                scr.DebugLog("Re-running looped script.");
                ReadAndRun(scr.ScriptName, scr.Sender); // so that it re-reads the content of the text file.
            }

            scr.DebugLog("Removing script from running scripts.");
            RunningScripts.Remove(scr);

            scr.Dispose();
        }
    }
}
