namespace ScriptedEvents.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    using CommandSystem;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Pools;

    using MEC;
    using PlayerRoles;
    using RemoteAdmin;

    using ScriptedEvents.Actions;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Integrations;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    using AirlockController = Exiled.API.Features.Doors.AirlockController;

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
        public static Dictionary<ActionNameData, Type> ActionTypes { get; } = new();

        /// <summary>
        /// Gets a dictionary of <see cref="Script"/> that are currently running, and the <see cref="CoroutineHandle"/> that is running them.
        /// </summary>
        public static Dictionary<Script, CoroutineHandle> RunningScripts { get; } = new();

        public static Dictionary<string, CustomAction> CustomActions { get; } = new();

        public static RueIManager Ruei { get; } = new RueIManager();

        public static bool TryGetActionType(string name, out Type type)
        {
            foreach (var actionData in ActionTypes)
            {
                if (actionData.Key.Name == name || actionData.Key.Aliases.Contains(name))
                {
                    type = actionData.Value;
                    return true;
                }
            }

            type = null;
            return false;
        }

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
        /// Retrieves a list of all scripts in the server.
        /// </summary>
        /// <param name="sender">Optional sender.</param>
        /// <returns>A list of all scripts.</returns>
        /// <remarks>WARNING: Scripts created through this method are NOT DISPOSED!!! Call <see cref="Script.Dispose"/> when done with them.</remarks>
        public static List<Script> ListScripts(ICommandSender sender = null)
        {
            List<Script> scripts = new();
            string[] files = Directory.GetFiles(ScriptPath, "*.txt", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                if (File.ReadAllText(file).Contains("!-- HELPRESPONSE"))
                    continue;

                try
                {
                    Script scr = ReadScript(Path.GetFileNameWithoutExtension(file), sender, true);
                    scripts.Add(scr);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            return scripts;
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
                        Log.Warn(ErrorGen.Get(103, flag, scriptName));

                    actionList.Add(new NullAction("FLAG DEFINE"));
                    continue;
                }

                // Regular string.Split(' '), except ignore spaces inside of variable names
                // Allows spaces in variable names without giving a bizarre error.
                MatchCollection collection = Regex.Matches(action, "\\[[^]]*]|\\{[^}]*}|[^ ]+");
                List<string> actionParts = ListPool<string>.Pool.Get();

                foreach (Match m in collection)
                {
                    actionParts.Add(m.Value);
                }

                string keyword = actionParts[0].RemoveWhitespace();

                // Labels
                if (keyword.EndsWith(":"))
                {
                    string labelName = action.Remove(keyword.Length - 1, 1).RemoveWhitespace();

                    if (!script.Labels.ContainsKey(labelName))
                        script.Labels.Add(labelName, currentline);
                    else if (!suppressWarnings)
                        Log.Warn(ErrorGen.Get(104, labelName, scriptName));

                    actionList.Add(new NullAction($"{labelName} LABEL"));

                    continue;
                }

#if DEBUG
                Log.Debug($"Queuing action {keyword} {string.Join(", ", actionParts.Skip(1))}");
#endif

                if (!TryGetActionType(keyword, out Type actionType))
                {
                    // Check for custom actions
                    if (CustomActions.TryGetValue(keyword, out CustomAction customAction))
                    {
                        CustomAction customAction1 = new(customAction.Name, customAction.Action)
                        {
                            Arguments = actionParts.Skip(1).Select(str => str.RemoveWhitespace()).ToArray(),
                        };
                        actionList.Add(customAction1);
                        ListPool<string>.Pool.Return(actionParts);
                        continue;
                    }

                    if (!suppressWarnings)
                        Log.Warn($"[L: {script.CurrentLine + 1}]" + ErrorGen.Get(102, keyword.RemoveWhitespace(), scriptName));

                    actionList.Add(new NullAction("ERROR"));
                    continue;
                }

                IAction newAction = Activator.CreateInstance(actionType) as IAction;
                newAction.Arguments = actionParts.Skip(1).Select(str => str.RemoveWhitespace()).ToArray();

                // Obsolete check
                if (newAction.IsObsolete(out string obsoleteReason) && !suppressWarnings && !script.SuppressWarnings)
                {
                    Log.Warn($"[L: {script.CurrentLine + 1}] Notice: Action {newAction.Name} is marked as obsolete. Please follow reason directives when using. Reason: {obsoleteReason}");
                }

                actionList.Add(newAction);
                ListPool<string>.Pool.Return(actionParts);
            }

            string scriptPath = GetFilePath(scriptName);

            // Fill out script data
            if (MainPlugin.Singleton.Config.RequiredPermissions is not null && MainPlugin.Singleton.Config.RequiredPermissions.TryGetValue(scriptName, out string perm2) == true)
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
            }
            else if (executor is PlayerCommandSender player)
            {
                script.Context = ExecuteContext.RemoteAdmin;
            }

            script.Sender = executor;

            script.DebugLog($"Debug script read successfully. Name: {script.ScriptName} | Actions: {script.Actions.Count(act => act is not NullAction)} | Flags: {string.Join(" ", script.Flags)} | Labels: {string.Join(" ", script.Labels)} | Comments: {script.Actions.Count(action => action is NullAction @null && @null.Type is "COMMENT")}");

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
        /// <param name="collection">A <see cref="PlayerCollection"/> representing the players.</param>
        /// <param name="source">The script using the API. Used for per-script variables.</param>
        /// <returns>Whether or not the process errored.</returns>
        public static bool TryGetPlayers(string input, int? amount, out PlayerCollection collection, Script source = null)
        {
            source.DebugLog($"TRYGETPLAYERS {input}");
            input = input.RemoveWhitespace();
            List<Player> list = ListPool<Player>.Pool.Get();
            if (input.ToUpper() is "*" or "ALL")
            {
                ListPool<Player>.Pool.Return(list);

                collection = new(Player.List.ToList());
                return true;
            }
            else
            {
                string[] variables = VariableSystem.IsolateVariables(input, source);
                foreach (string variable in variables)
                {
                    try
                    {
                        if (VariableSystem.TryGetPlayers(variable, out PlayerCollection playersFromVariable, source))
                        {
                            list.AddRange(playersFromVariable);
                        }
                    }
                    catch (Exception e)
                    {
                        collection = new(null, false, $"Error when processing the {variable} variable: {e.Message}");
                        return false;
                    }
                }
            }

            // If list is still empty, match directly
            if (list.Count == 0)
            {
                Player match = Player.Get(input);
                if (match is not null)
                    list.Add(match);
            }

            // Shuffle, Remove unconnected/overwatch, limit
            list.ShuffleList();
            list.RemoveAll(p => !p.IsConnected);

            if (MainPlugin.Configs.IgnoreOverwatch)
                list.RemoveAll(p => p.Role.Type is RoleTypeId.Overwatch);

            if (amount.HasValue && amount.Value > 0 && list.Count > 0)
            {
                while (list.Count > amount.Value)
                {
                    list.PullRandomItem();
                }
            }

            // Return
            collection = new(list);
            return true;
        }

        /// <summary>
        /// Try-get a <see cref="Door"/> array given an input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="doors">The doors.</param>
        /// <param name="source">The script source.</param>
        /// <returns>Whether or not the try-get was successful.</returns>
        public static bool TryGetDoors(string input, out Door[] doors, Script source = null)
        {
            List<Door> doorList = ListPool<Door>.Pool.Get();
            if (input is "*" or "ALL")
            {
                doorList = Door.List.ToList();
            }
            else if (VariableSystem.TryParse<ZoneType>(input, out ZoneType zt, source))
            {
                doorList = Door.List.Where(d => d.Zone.HasFlag(zt)).ToList();
            }
            else if (VariableSystem.TryParse<DoorType>(input, out DoorType dt, source))
            {
                doorList = Door.List.Where(d => d.Type == dt).ToList();
            }
            else if (VariableSystem.TryParse<RoomType>(input, out RoomType rt, source))
            {
                doorList = Door.List.Where(d => d.Room?.Type == rt).ToList();
            }
            else
            {
                doorList = Door.List.Where(d => d.Name.ToLower() == input.ToLower()).ToList();
            }

            doorList = doorList.Where(d => d.IsElevator is false && d.Type is not DoorType.Scp914Door && d.Type is not DoorType.Scp079First && d.Type is not DoorType.Scp079Second && AirlockController.Get(d) is null).ToList();
            doors = ListPool<Door>.Pool.ToArrayReturn(doorList);
            return doors.Length > 0;
        }

        /// <summary>
        /// Try-get a <see cref="Lift"/> array given an input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="lifts">The lift objects.</param>
        /// <param name="source">The script source.</param>
        /// <returns>Whether or not the try-get was successful.</returns>
        public static bool TryGetLifts(string input, out Lift[] lifts, Script source = null)
        {
            List<Lift> liftList = ListPool<Lift>.Pool.Get();
            if (input is "*" or "ALL")
            {
                liftList = Lift.List.ToList();
            }
            else if (VariableSystem.TryParse<ElevatorType>(input, out ElevatorType et, source))
            {
                liftList = Lift.List.Where(l => l.Type == et).ToList();
            }
            else
            {
                liftList = Lift.List.Where(l => l.Name.ToLower() == input.ToLower()).ToList();
            }

            lifts = ListPool<Lift>.Pool.ToArrayReturn(liftList);
            return lifts.Length > 0;
        }

        /// <summary>
        /// Try-get a <see cref="Room"/> array given an input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="rooms">The rooms.</param>
        /// <param name="source">The script source.</param>
        /// <returns>Whether or not the try-get was successful.</returns>
        public static bool TryGetRooms(string input, out Room[] rooms, Script source = null)
        {
            List<Room> roomList = ListPool<Room>.Pool.Get();
            if (input is "*" or "ALL")
            {
                roomList = Room.List.ToList();
            }
            else if (VariableSystem.TryParse<ZoneType>(input, out ZoneType zt, source))
            {
                roomList = Room.List.Where(room => room.Zone.HasFlag(zt)).ToList();
            }
            else if (VariableSystem.TryParse<RoomType>(input, out RoomType rt, source))
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

        public static void ShowHint(string text, float duration, List<Player> players = null)
        {
            if (players == null)
                players = Player.List.ToList();

            Ruei.MakeNew();
            players.ForEach(p => Ruei.ShowHint(p, text, TimeSpan.FromSeconds(duration)));
        }

        /// <summary>
        /// Immediately stops execution of all scripts.
        /// </summary>
        /// <returns>The amount of scripts that were stopped.</returns>
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

            CoroutineHelper.KillAll();

            RunningScripts.Clear();
            return amount;
        }

        /// <summary>
        /// Stops execution of a script.
        /// </summary>
        /// <param name="scr">The script to stop.</param>
        /// <returns>Whether or not stopping was successful.</returns>
        public static bool StopScript(Script scr)
        {
            KeyValuePair<Script, CoroutineHandle>? found = RunningScripts.FirstOrDefault(k => k.Key == scr);
            if (found.HasValue && found.Value.Value.IsRunning)
            {
                Timing.KillCoroutines(found.Value.Value);

                found.Value.Key.Coroutines.ForEach(data =>
                {
                    if (!data.IsKilled)
                        data.Kill();
                });

                found.Value.Key.IsRunning = false;
                found.Value.Key.Dispose();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Stops execution of all scripts with the matching name.
        /// </summary>
        /// <param name="name">The name of the script.</param>
        /// <returns>The amount of scripts stopped.</returns>
        public static int StopScripts(string name)
        {
            int amount = 0;
            foreach (KeyValuePair<Script, CoroutineHandle> kvp in RunningScripts)
            {
                if (kvp.Key.ScriptName == name && StopScript(kvp.Key))
                    amount++;
            }

            return amount;
        }

        /// <summary>
        /// Determines if an action is obsolete.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="message">A message regarding the obsolete.</param>
        /// <returns>Whether or not the action is obsolete.</returns>
        public static bool IsObsolete(this IAction action, out string message)
        {
            Type t = action.GetType();
            var obsolete = t.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.Name == "ObsoleteAttribute");
            if (obsolete is not null)
            {
                message = obsolete.ConstructorArguments[0].Value.ToString();
                return true;
            }

            message = string.Empty;
            return false;
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
                    ActionTypes.Add(new() { Name = temp.Name, Aliases = temp.Aliases }, type);
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

            yield return Timing.WaitForOneFrame;

            scr.IsRunning = true;
            scr.RunDate = DateTime.UtcNow;

            int safetyActionCount = 0;
            Stopwatch safety = Stopwatch.StartNew();

            Stopwatch runTime = Stopwatch.StartNew();
            int lines = 0;
            int successfulLines = 0;

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
                        string message = $"[Script: {scr.ScriptName}] [L: {scr.CurrentLine + 1}] {ErrorGen.Get(141, action.Name)}:\n{e}";
                        switch (scr.Context)
                        {
                            case ExecuteContext.RemoteAdmin:
                                Player ply = Player.Get(scr.Sender);
                                ply.RemoteAdminMessage(message, false, MainPlugin.Singleton.Name);

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
                                    ply?.RemoteAdminMessage(message, false, MainPlugin.Singleton.Name);

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
                                    ply?.RemoteAdminMessage(message, false, MainPlugin.Singleton.Name);

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
                        successfulLines++;
                        if (!string.IsNullOrEmpty(resp.Message))
                        {
                            string message = $"[Script: {scr.ScriptName}] [L: {scr.CurrentLine + 1}] [{action.Name}] Response: {resp.Message}";
                            switch (scr.Context)
                            {
                                case ExecuteContext.RemoteAdmin:
                                    Player.Get(scr.Sender)?.RemoteAdminMessage(message, true, MainPlugin.Singleton.Name);
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

                    lines++;

                    if (resp.ResponseFlags.HasFlag(ActionFlags.StopEventExecution))
                        break;

                    // Safety
                    safetyActionCount++;
                    if (safety.Elapsed.TotalSeconds > 1)
                    {
                        if (safetyActionCount > MainPlugin.Configs.MaxActionsPerSecond && !scr.Flags.Contains("NOSAFETY"))
                        {
                            Log.Warn(ErrorGen.Get(113, scr.ScriptName, MainPlugin.Configs.MaxActionsPerSecond));
                            break;
                        }
                        else
                        {
                            safety.Restart();
                            safetyActionCount = 0;
                        }
                    }
                }
            }

            scr.DebugLog("-----------");
            scr.DebugLog($"Script {scr.ScriptName} concluded. Total time '{runTime.Elapsed:mm':'ss':'fff}', Executed '{successfulLines}/{lines}' ({Math.Round((float)successfulLines / lines * 100)}%) actions successfully");
            MainPlugin.Info($"Finished running script {scr.ScriptName}.");
            scr.DebugLog("-----------");
            scr.IsRunning = false;

            if (scr.Flags.Contains("LOOP"))
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
