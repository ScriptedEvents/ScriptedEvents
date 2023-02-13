namespace ScriptedEvents.API.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using MEC;
    using ScriptedEvents.Actions;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features.Aliases;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
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
        /// <returns>The <see cref="Script"/> fully filled out, if the script was found.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script is not found.</exception>
        public static Script ReadScript(string scriptName)
        {
            Script script = new();
            string allText = ReadScriptText(scriptName);

            string[] array = allText.Split('\n');
            for (int currentline = 0; currentline < array.Length; currentline++)
            {
                // NoAction
                string action = array[currentline];
                if (string.IsNullOrWhiteSpace(action))
                {
                    script.Actions.Add(new NullAction("BLANK LINE"));
                    continue;
                }
                else if (action.StartsWith("#"))
                {
                    script.Actions.Add(new NullAction("COMMENT"));
                    continue;
                }
                else if (action.StartsWith("!--"))
                {
                    string flag = action.Substring(3).RemoveWhitespace();
                    script.Flags.Add(flag);

                    script.Actions.Add(new NullAction("FLAG DEFINE"));
                    continue;
                }

                string[] actionParts = action.Split(' ');
                string keyword = actionParts[0].RemoveWhitespace();

                // Labels
                if (keyword.EndsWith(":"))
                {
                    string labelName = action.Remove(keyword.Length - 1, 1).RemoveWhitespace();
                    script.Labels.Add(labelName, currentline);
                    script.Actions.Add(new NullAction($"{labelName} LABEL"));

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
                        script.Actions.Add(customAction1);
                        continue;
                    }

                    Log.Info($"Invalid action '{keyword.RemoveWhitespace()}' detected in script '{scriptName}'");
                    script.Actions.Add(new NullAction("ERROR"));
                    continue;
                }

                IAction newAction = Activator.CreateInstance(actionType) as IAction;
                newAction.Arguments = actionParts.Skip(1).Select(str => str.RemoveWhitespace()).ToArray();

                script.Actions.Add(newAction);
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
            return script;
        }

        /// <summary>
        /// Runs the script.
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
        /// <exception cref="FileNotFoundException">The script was not found.</exception>
        /// <exception cref="DisabledScriptException">If <see cref="Script.Disabled"/> is <see langword="true"/>.</exception>
        public static void ReadAndRun(string scriptName)
        {
            Script scr = ReadScript(scriptName);
            if (scr is not null)
                RunScript(scr);
        }

        /// <summary>
        /// Converts an input into a list of players.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="amount">The maximum amount of players to give back, or <see langword="null"/> for unlimited.</param>
        /// <param name="plys">The list of players.</param>
        /// <returns>Whether or not any players were found.</returns>
        public static bool TryGetPlayers(string input, int? amount, out List<Player> plys)
        {
            input = input.RemoveWhitespace();
            plys = new();
            if (input.ToUpper() is "*" or "ALL")
            {
                plys = Player.List.ToList();
                return true;
            }
            else
            {
                string[] variables = PlayerVariables.IsolateVariables(input);
                foreach (string variable in variables)
                {
                    if (PlayerVariables.TryGet(variable, out IEnumerable<Player> playersFromVariable))
                    {
                        plys.AddRange(playersFromVariable);
                    }
                }

                if (Player.TryGet(input, out Player ply))
                {
                    plys.Add(ply);
                }
            }

            plys.ShuffleList();
            plys.RemoveAll(p => !p.IsConnected);

            if (amount.HasValue && amount.Value > 0)
            {
                if (amount.Value < plys.Count)
                {
                    for (int i = 0; i < amount.Value; i++)
                    {
                        plys.PullRandomItem();
                    }
                }
            }

            return plys.Count > 0;
        }

        public static bool TryGetDoors(string input, out List<Door> doors)
        {
            doors = new();
            if (input == "*")
            {
                doors = Door.List.ToList();
            }
            else if (Enum.TryParse<ZoneType>(input, true, out ZoneType zt))
            {
                doors = Door.List.Where(d => d.Zone == zt).ToList();
            }
            else if (Enum.TryParse<DoorType>(input, true, out DoorType dt))
            {
                doors = Door.List.Where(d => d.Type == dt).ToList();
            }
            else if (Enum.TryParse<RoomType>(input, true, out RoomType rt))
            {
                doors = Door.List.Where(d => d.Room?.Type == rt).ToList();
            }
            else
            {
                doors = Door.List.Where(d => d.Name.ToLower() == input.ToLower()).ToList();
            }

            doors = doors.Where(d => d.IsElevator is false && d.Type is not DoorType.Scp079First && d.Type is not DoorType.Scp079Second).ToList();
            return doors.Count > 0;
        }

        public static int StopAllScripts()
        {
            int amount = 0;
            foreach (KeyValuePair<Script, CoroutineHandle> kvp in RunningScripts)
            {
                amount++;
                kvp.Key.IsRunning = false;
                Timing.KillCoroutines(kvp.Value);
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
            string mainFolderFile = Path.Combine(ScriptPath, scriptName + ".txt");
            if (File.Exists(mainFolderFile))
            {
                fileDirectory = mainFolderFile;
                return File.ReadAllText(mainFolderFile);
            }

            foreach (string directory in Directory.GetDirectories(ScriptPath))
            {
                string fileName = Path.Combine(directory, scriptName + ".txt");
                if (File.Exists(fileName))
                {
                    fileDirectory = fileName;
                    return File.ReadAllText(fileName);
                }
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

            for (; scr.CurrentLine < scr.Actions.Count; scr.NextLine())
            {
                if (scr.Actions.TryGet(scr.CurrentLine, out IAction action) && action != null)
                {
                    ActionResponse resp;
                    float? delay = null;

                    try
                    {
                        switch (action)
                        {
                            case ITimingAction timed:
                                Log.Debug($"[Script: {scr.ScriptName}] Running {action.Name} action...");
                                delay = timed.Execute(scr, out resp);
                                break;
                            case IScriptAction scriptAction:
                                Log.Debug($"[Script: {scr.ScriptName}] Running {action.Name} action...");
                                resp = scriptAction.Execute(scr);
                                break;
                            default:
                                continue;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error($"[Script: {scr.ScriptName}] Ran into an error while running {action.Name} action:\n{e}");
                        continue;
                    }

                    if (!resp.Success)
                    {
                        if (resp.ResponseFlags.HasFlag(ActionFlags.FatalError))
                        {
                            Log.Error($"[Script: {scr.ScriptName}] [{action.Name}] Fatal action error! {resp.Message}");
                            break;
                        }
                        else
                        {
                            Log.Warn($"[Script: {scr.ScriptName}] [{action.Name}] Action error! {resp.Message}");
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(resp.Message))
                            Log.Info($"[Script: {scr.ScriptName}] [{action.Name}] Response: {resp.Message}");
                        if (delay.HasValue)
                            yield return delay.Value;
                    }

                    if (resp.ResponseFlags.HasFlag(ActionFlags.StopEventExecution))
                        break;
                }
            }

            MainPlugin.Info($"Finished running script {scr.ScriptName}.");
            scr.IsRunning = false;

            if (MainPlugin.Singleton.Config.LoopScripts.Contains(scr.ScriptName))
            {
                ReadAndRun(scr.ScriptName); // so that it re-reads the content of the text file.
            }

            RunningScripts.Remove(scr);
        }
    }
}
