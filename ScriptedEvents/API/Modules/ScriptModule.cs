using System.Data;
using ScriptedEvents.Actions.DebugActions;
using ScriptedEvents.Actions.Logic;

namespace ScriptedEvents.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
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
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.API.Interfaces;

    using ScriptedEvents.DemoScripts;

    using ScriptedEvents.Structures;

    using AirlockController = Exiled.API.Features.Doors.AirlockController;

    /// <summary>
    /// A helper class to read and execute scripts, and register actions, as well as providing useful API for individual actions.
    /// </summary>
    public class ScriptModule : SEModule
    {
        /// <summary>
        /// The base path to the script folder.
        /// </summary>
        public static readonly string BasePath = Path.Combine(MainPlugin.BaseFilePath, "Scripts");

        /// <summary>
        /// Gets a dictionary of action names and their respective types.
        /// </summary>
        public Dictionary<ActionNameData, Type> ActionTypes { get; } = new();

        /// <summary>
        /// Gets a dictionary of <see cref="Script"/> that are currently running, and the <see cref="CoroutineHandle"/> that is running them.
        /// </summary>
        public Dictionary<Script, CoroutineHandle> RunningScripts { get; } = new();

        /// <summary>
        /// Gets all defined custom actions.
        /// </summary>
        public Dictionary<string, CustomAction> CustomActions { get; } = new();

        public List<string> AutoRunScripts { get; set; } = new();

        /// <inheritdoc/>
        public override string Name { get; } = "ScriptModule";

        public override bool ShouldGenerateFiles
            => !Directory.Exists(BasePath);

        public override void GenerateFiles()
        {
            base.GenerateFiles();

            GenerateDemoScripts();

            // Welcome message :)
            // 3s delay to show after other console spam
            Timing.CallDelayed(6f, () =>
            {
                Logger.Warn($"Thank you for installing Scripted Events! View the README file located at {Path.Combine(MainPlugin.BaseFilePath, "README.txt")} for information on how to use and get the most out of this plugin.");
            });
        }

        public override void Init()
        {
            base.Init();

            RegisterActions(MainPlugin.Singleton.Assembly);

            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
        }

        public override void Kill()
        {
            base.Kill();
            StopAllScripts();
            ActionTypes.Clear();

            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
        }

        public static void GenerateDemoScripts()
        {
            try
            {
                DirectoryInfo info = Directory.CreateDirectory(BasePath);
                DirectoryInfo demoScriptFolder = Directory.CreateDirectory(Path.Combine(info.FullName, "DemoScripts"));
                foreach (IDemoScript demo in MainPlugin.DemoScripts)
                {
                    File.WriteAllText(Path.Combine(demoScriptFolder.FullName, $"{demo.FileName}.txt"), demo.Contents);
                }

                File.WriteAllText(Path.Combine(MainPlugin.BaseFilePath, "README.txt"), new About().Contents);
            }
            catch (UnauthorizedAccessException e)
            {
                Logger.Error(ErrorGen.Get(ErrorCode.IOPermissionError) + $": {e}");
            }
            catch (Exception e)
            {
                Logger.Error(ErrorGen.Get(ErrorCode.IOError) + $": {e}");
            }
        }

        public void OnWaitingForPlayers()
        {
            foreach (Script scr in ListScripts())
            {
                if (!scr.HasFlag("AUTORUN"))
                {
                    continue;
                }

                Logger.Debug($"Script '{scr.Name}' set to run automatically.");
                AutoRunScripts.Add(scr.Name);
                try
                {
                    if (scr.AdminEvent)
                    {
                        Logger.Warn(ErrorGen.Get(ErrorCode.AutoRun_AdminEvent, scr.Name));
                        continue;
                    }

                    RunScript(scr);
                }
                catch (DisabledScriptException)
                {
                    Logger.Warn(ErrorGen.Get(ErrorCode.AutoRun_Disabled, scr.Name));
                }
                catch (FileNotFoundException)
                {
                    Logger.Warn(ErrorGen.Get(ErrorCode.AutoRun_NotFound, scr.Name));
                }
            }
        }

        /// <summary>
        /// Returns an action type, if its name or aliases match the input.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <param name="type">The action.</param>
        /// <returns>Whether or not the try-get was successful.</returns>
        public bool TryGetActionType(string name, out Type type)
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
        /// Retrieves a list of all scripts in the server.
        /// </summary>
        /// <param name="sender">Optional sender.</param>
        /// <returns>A list of all scripts.</returns>
        /// <remarks>WARNING: Scripts created through this method are NOT DISPOSED!!! Call <see cref="Script.Dispose"/> when done with them.</remarks>
        public List<Script> ListScripts(ICommandSender sender = null)
        {
            List<Script> scripts = new();
            string[] files = Directory.GetFiles(BasePath, "*.txt", SearchOption.AllDirectories);
            var duplicates = files
                .Select(Path.GetFileNameWithoutExtension)
                .GroupBy(s => s)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count > 0)
            {
                foreach (var duplicate in duplicates)
                {
                    Logger.Error($"Duplicate script name '{duplicate}' found! Please ensure all script names are unique.");
                }

                throw new ArgumentException($"There are {duplicates.Count} scripts of which the names are not unique. Please ensure all script names are unique to avoid conflicts.");
            }

            foreach (string file in files)
            {
                try
                {
                    Script scr = ReadScript(Path.GetFileNameWithoutExtension(file), sender, true);
                    scripts.Add(scr);
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
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
        public Script ReadScript(string scriptName, ICommandSender executor, bool suppressWarnings = false)
        {
            string allText = GetScriptValue(scriptName, out var scriptDirectory);
            bool inMultilineComment = false;
            Script script = new();

            List<IAction> actionList = ListPool<IAction>.Pool.Get();

            Action<IAction> addActionNoArgs = (action) =>
            {
                script.OriginalActionArgs[action] = Array.Empty<string>();
                actionList.Add(action);
            };

            string[] array = allText.Split('\n');
            IAction lastAction = null;
            for (int currentline = 0; currentline < array.Length; currentline++)
            {
                array[currentline] = array[currentline].TrimStart().Replace("\n", string.Empty);

                // no action
                string line = array[currentline];
                if (string.IsNullOrWhiteSpace(line))
                {
                    addActionNoArgs(new NullAction("BLANK LINE"));
                    continue;
                }
                else if (line.StartsWith("##"))
                {
                    inMultilineComment = !inMultilineComment;
                    addActionNoArgs(new NullAction("COMMENT"));
                    continue;
                }
                else if (line.StartsWith("#") || inMultilineComment)
                {
                    addActionNoArgs(new NullAction("COMMENT"));
                    continue;
                }

                List<string> structureParts = ListPool<string>.Pool.Get();

                foreach (string str in line.Split(' '))
                {
                    if (string.IsNullOrWhiteSpace(str))
                        continue;

                    structureParts.Add(str);
                }

                string keyword = structureParts[0].RemoveWhitespace();

                // labels
                if (keyword.EndsWith(":"))
                {
                    string labelName = line.Remove(keyword.Length - 1, 1).RemoveWhitespace();

                    if (!script.Labels.ContainsKey(labelName))
                        script.Labels.Add(labelName, currentline);
                    else if (!suppressWarnings)
                        Logger.ScriptError(ErrorGen.Get(ErrorCode.MultipleLabelDefs, labelName, scriptName), script, printableLine: currentline + 1);

                    addActionNoArgs(new NullAction($"{labelName} LABEL"));
                    continue;
                }

                // function labels
                if (keyword == "->")
                {
                    if (structureParts.Count < 2)
                    {
                        Logger.ScriptError($"A function label syntax has been used, but no name has been provided.", script, printableLine: currentline + 1);
                        continue;
                    }

                    string labelName = structureParts[1].RemoveWhitespace();

                    if (!script.FunctionLabels.ContainsKey(labelName))
                        script.FunctionLabels.Add(labelName, currentline);
                    else if (!suppressWarnings)
                        Logger.ScriptError(ErrorGen.Get(ErrorCode.MultipleLabelDefs, labelName, scriptName), script, printableLine: currentline + 1);

                    addActionNoArgs(new StartFunctionAction());
                    continue;
                }

                // smart args
                if (keyword == "//")
                {
                    if (actionList.Count == 0)
                    {
                        Logger.Log("'//' (smart argument) syntax can't be used if there isn't any action above it.", LogType.Warning, script, currentline + 1);
                        continue;
                    }

                    string value = string.Join(" ", structureParts.Skip(1)).Trim();

                    if (script.SmartArguments.ContainsKey(lastAction))
                    {
                        script.SmartArguments[lastAction] = script.SmartArguments[lastAction].Append(value).ToArray();
                    }
                    else
                    {
                        script.SmartArguments[lastAction] = new[] { value };
                    }

                    addActionNoArgs(new NullAction($"SMART ARG"));
                    continue;
                }

                // flags
                if (keyword == "!--")
                {
                    string flag = structureParts[1].Trim();

                    if (!script.HasFlag(flag))
                    {
                        Flag fl = new(flag, structureParts.Skip(2));
                        script.Flags.Add(fl);
                    }
                    else if (!suppressWarnings)
                    {
                        Logger.Warn(ErrorGen.Get(ErrorCode.MultipleFlagDefs, flag, scriptName));
                    }

                    addActionNoArgs(new NullAction("FLAG DEFINE"));
                    continue;
                }

                // extractor
                int indexOfExtractor = line.IndexOf("::");
                string[] resultVariableNames = Array.Empty<string>();

                if (indexOfExtractor != -1)
                {
                    string variablesSection = line.Substring(0, indexOfExtractor);
                    string actionSection = line.Substring(indexOfExtractor + 2).TrimStart();
                    DebugLog($"[ExtractorSyntax] Variables section: {variablesSection}", script);
                    DebugLog($"[ExtractorSyntax] Action section: {actionSection}", script);

                    resultVariableNames = VariableSystemV2.IsolateVariables(variablesSection, script);
                    if (resultVariableNames.Length == 0)
                    {
                        goto leave_extractor_parsing;
                    }

                    DebugLog($"[ExtractorSyntax] Variables found before the syntax: {string.Join(", ", resultVariableNames)}", script);

                    structureParts = actionSection.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(part => part.Trim()).ToList();
                    keyword = structureParts[0];
                }

                leave_extractor_parsing:

                keyword = keyword.ToUpper();

                if (!TryGetActionType(keyword, out Type actionType))
                {
                    // Check for custom actions
                    if (CustomActions.TryGetValue(keyword, out CustomAction customAction))
                    {
                        List<string> customActArgs = ListPool<string>.Pool.Get();
                        CustomAction customAction1 = new(customAction.Name, customAction.Action);

                        foreach (string part in structureParts.Skip(1))
                        {
                            if (ArgumentProcessor.TryProcessSmartArgument(part, customAction1, script, out string result, false))
                            {
                                customActArgs.Add(result);
                            }
                            else
                            {
                                customActArgs.Add(part);
                            }
                        }

                        customAction1.RawArguments = customActArgs.ToArray();
                        actionList.Add(customAction1);
                        script.ResultVariableNames[customAction1] = resultVariableNames;

                        ListPool<string>.Pool.Return(customActArgs);
                        ListPool<string>.Pool.Return(structureParts);
                        continue;
                    }

                    if (!suppressWarnings)
                        Logger.Warn(ErrorGen.Get(ErrorCode.InvalidAction, keyword.RemoveWhitespace(), scriptName), script);

                    addActionNoArgs(new NullAction("ERROR"));
                    continue;
                }

                IAction newAction = Activator.CreateInstance(actionType) as IAction;
                lastAction = newAction;
                script.OriginalActionArgs[newAction] = structureParts.Skip(1).Select(str => str.RemoveWhitespace()).ToArray();
                script.ResultVariableNames[newAction] = resultVariableNames;

                Logger.Debug($"Queuing action {keyword}, {string.Join(", ", script.OriginalActionArgs[newAction])}", script);

                // Obsolete check
                if (newAction.IsObsolete(out string obsoleteReason) && !suppressWarnings && !script.SuppressWarnings)
                {
                    Logger.Warn($"Action {newAction.Name} is obsolete; {obsoleteReason}", script);
                }

                actionList.Add(newAction);
                ListPool<string>.Pool.Return(structureParts);
            }

            // Fill out script data
            if (MainPlugin.Singleton.Config.RequiredPermissions is not null && MainPlugin.Singleton.Config.RequiredPermissions.TryGetValue(scriptName, out string perm2))
            {
                script.ReadPermission += $".{perm2}";
                script.ExecutePermission += $".{perm2}";
            }

            script.Name = scriptName;
            script.RawText = allText;
            script.FilePath = scriptDirectory;
            script.LastRead = File.GetLastAccessTimeUtc(scriptDirectory);
            script.LastEdited = File.GetLastWriteTimeUtc(scriptDirectory);
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

            script.DebugLog($"Debug script read successfully. Name: {script.Name} | Actions: {script.Actions.Count(act => act is not NullAction)} | Flags: {string.Join(" ", script.Flags)} | Labels: {string.Join(" ", script.Labels)} | Comments: {script.Actions.Count(action => action is NullAction @null && @null.Type is "COMMENT")}");

            return script;
        }

        /// <summary>
        /// Runs the script and disposes of it as soon as the script execution is complete.
        /// </summary>
        /// <param name="scr">The script to run.</param>
        /// <param name="dispose">If <see langword="true"/>, the script will be disposed after finishing execution.</param>
        /// <exception cref="DisabledScriptException">If <see cref="Script.Disabled"/> is <see langword="true"/>.</exception>
        public void RunScript(Script scr, bool dispose = true)
        {
            if (scr.Disabled)
                throw new DisabledScriptException(scr.Name);

            CoroutineHandle handle = Timing.RunCoroutine(RunScriptInternal(scr, dispose), $"SCRIPT_{scr.UniqueId}");
            RunningScripts.Add(scr, handle);
        }

        /// <summary>
        /// Reads and runs a script.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <param name="executor">The executor that is running the script. Can be null.</param>
        /// <param name="dispose">Whether to dispose of the script as soon as execution is finished.</param>
        /// <exception cref="FileNotFoundException">The script was not found.</exception>
        /// <exception cref="DisabledScriptException">If <see cref="Script.Disabled"/> is <see langword="true"/>.</exception>
        /// <returns>The script object.</returns>
        public Script ReadAndRun(string scriptName, ICommandSender executor, bool dispose = true)
        {
            Script scr = ReadScript(scriptName, executor);
            if (scr is not null)
                RunScript(scr, dispose);

            return scr;
        }

        /// <summary>
        /// Converts an input into a list of players.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="amount">The maximum amount of players to give back, or <see langword="null"/> for unlimited.</param>
        /// <param name="collection">A <see cref="PlayerCollection"/> representing the players.</param>
        /// <param name="source">The script using the API. Used for per-script variables.</param>
        /// <param name="brecketsRequired">Are brackets required.</param>
        /// <returns>Whether or not the process errored.</returns>
        public static bool TryGetPlayers(string input, int? amount, out PlayerCollection collection, Script source, bool brecketsRequired = true)
        {
            void Log(string msg)
            {
                Logger.Debug($"[TryGetPlayers] {msg}", source);
            }

            Log($"Trying to get '{input}' player collection.");

            input = input.RemoveWhitespace();
            List<Player> list = new();

            // TODO: When a variable with a player id is provided, it wont work since the raw variable name cant match to the regex
            // but if we convert a valid variable to int, a e.g. player variable with 1 player will then say that "yeah actually im the server"
            // which is just a tiny bit stupid
            string patternForPlayerIdUsage = @"^\d+(\.\d+)*\.?$";

            if (input.ToUpper() is "*" or "ALL")
            {
                Log("Getting all players");
                list = Player.List.ToList();
            }
            else if (Regex.IsMatch(input, patternForPlayerIdUsage))
            {
                Log("Doing regex");
                string[] splitInput = input.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string idInStr in splitInput)
                {
                    if (!int.TryParse(idInStr, out int playerId))
                        continue;

                    if (!Player.TryGet(playerId, out Player player))
                        continue;

                    Log($"Extracted a player object from {idInStr} string.");
                    list.Add(player);
                }
            }
            else if (VariableSystemV2.TryGetPlayers(input, source, out PlayerCollection playersFromVariable, brecketsRequired))
            {
                list = playersFromVariable.GetInnerList();
                Log($"Doing by variable: got {list.Count} players");
            }
            else
            {
                Log("Matching directly");
                Player match = Player.Get(input);
                if (match is not null)
                    list.Add(match);
            }

            // Shuffle, Remove unconnected/overwatch, limit
            list.ShuffleList();

            /* list.RemoveAll(p => !p.IsConnected); */

            if (MainPlugin.Configs.IgnoreOverwatch)
                list.RemoveAll(p => p.Role.Type is RoleTypeId.Overwatch);

            if (amount is > 0 && list.Count > 0)
            {
                Log("Removing players");
                while (list.Count > amount.Value)
                {
                    list.PullRandomItem();
                }
            }

            // Return
            Log($"Complete! Returning {list.Count} players");
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
        public static bool TryGetDoors(string input, out Door[] doors, Script source)
        {
            List<Door> doorList = ListPool<Door>.Pool.Get();
            if (input is "*" or "ALL")
            {
                doorList = Door.List.ToList();
            }
            else if (SEParser.TryParse(input, out ZoneType zt, source))
            {
                doorList = Door.List.Where(d => d.Zone.HasFlag(zt)).ToList();
            }
            else if (SEParser.TryParse(input, out DoorType dt, source))
            {
                doorList = Door.List.Where(d => d.Type == dt).ToList();
            }
            else if (SEParser.TryParse(input, out RoomType rt, source))
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
        public static bool TryGetLifts(string input, out Lift[] lifts, Script source)
        {
            List<Lift> liftList = ListPool<Lift>.Pool.Get();
            if (input is "*" or "ALL")
            {
                liftList = Lift.List.ToList();
            }
            else if (SEParser.TryParse<ElevatorType>(input, out ElevatorType et, source))
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
        public static bool TryGetRooms(string input, out Room[] rooms, Script source)
        {
            List<Room> roomList = ListPool<Room>.Pool.Get();
            if (input is "*" or "ALL")
            {
                roomList = Room.List.ToList();
            }
            else if (SEParser.TryParse<ZoneType>(input, out ZoneType zt, source))
            {
                roomList = Room.List.Where(room => room.Zone.HasFlag(zt)).ToList();
            }
            else if (SEParser.TryParse<RoomType>(input, out RoomType rt, source))
            {
                roomList = Room.List.Where(d => d.Type == rt).ToList();
            }
            else
            {
                roomList = Room.List.Where(d => string.Equals(d.Name, input, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            rooms = ListPool<Room>.Pool.ToArrayReturn(roomList);
            return rooms.Length > 0;
        }

        /// <summary>
        /// Shows a RueI hint.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="players">The players to show (or all).</param>
        public void ShowHint(string text, float duration, List<Player> players = null)
        {
            players ??= Player.List.ToList();
            players.ForEach(p => p.ShowHint(text, duration));
        }

        /// <summary>
        /// Immediately stops execution of all scripts.
        /// </summary>
        /// <returns>The amount of scripts that were stopped.</returns>
        public int StopAllScripts()
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
        public bool StopScript(Script scr)
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
        public int StopScripts(string name)
        {
            int amount = 0;
            foreach (KeyValuePair<Script, CoroutineHandle> kvp in RunningScripts)
            {
                if (kvp.Key.Name == name && StopScript(kvp.Key))
                    amount++;
            }

            return amount;
        }

        /// <summary>
        /// Reads a script.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <param name="fileDirectory">The directory of the script, if it is found.</param>
        /// <returns>The contents of the script, if it is found.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script is not found.</exception>
        internal string GetScriptValue(string scriptName, out string filePath)
        {
            filePath = string.Empty;

            if (TryFindInFolder(scriptName, BasePath, out var result, out var fileDirectory))
            {
                filePath = Path.Combine(fileDirectory, scriptName + ".txt");
                return result;
            }

            throw new FileNotFoundException($"Script {scriptName} does not exist.");

            bool TryFindInFolder(string scriptName, string folder, out string result, out string fileDirectory)
            {
                fileDirectory = folder;
                result = string.Empty;
                var pathToScript = Path.Combine(folder, scriptName + ".txt");

                if (File.Exists(pathToScript))
                {
                    result = File.ReadAllText(pathToScript);
                    return true;
                }

                foreach (var newFolder in Directory.GetDirectories(folder))
                {
                    if (TryFindInFolder(scriptName, Path.Combine(folder, newFolder), out result, out fileDirectory))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Registers all the actions in the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly to register actions in.</param>
        internal void RegisterActions(Assembly assembly)
        {
            int i = 0;
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IAction).IsAssignableFrom(type) && type.IsClass && type.GetConstructors().Length > 0)
                {
                    if (type == typeof(CustomAction))
                        continue;

                    IAction temp = (IAction)Activator.CreateInstance(type);

                    Logger.Debug($"Adding Action: {temp.Name} | From Assembly: {assembly.GetName().Name}");
                    ActionTypes.Add(new(temp.Name, temp.Aliases), type);
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
        private IEnumerator<float> RunScriptInternal(Script scr, bool dispose = true)
        {
            MainPlugin.Info($"Started running the '{scr.Name}' script.");

            yield return Timing.WaitForOneFrame;

            scr.IsRunning = true;
            scr.RunDate = DateTime.UtcNow;

            Stopwatch runTime = Stopwatch.StartNew();
            int lines = 0;
            int successfulLines = 0;

            for (; scr.CurrentLine < scr.Actions.Length; scr.NextLine())
            {
                Logger.Debug("-----------", scr);
                Logger.Debug($"Current Line: {scr.CurrentLine + 1}", scr);
                if (!scr.Actions.TryGet(scr.CurrentLine, out IAction action) || action == null)
                {
                    Logger.Debug("There is no runnable action on this line. Skipping...", scr);
                    continue;
                }

                Logger.Debug($"Fetched action '{action.Name}'", scr);
                if (action is NullAction nullAction)
                {
                    Logger.Debug($"Null action type: {nullAction.Type}", scr);
                }

                if (scr.IfActionBlocksExecution && action is not IIgnoresIfActionBlock)
                {
                    Logger.Debug("Action was skipped; the IF statement resulted in FALSE and action is not 'ITerminatesIfAction'", scr);
                    continue;
                }

                Logger.Debug("Action was not skipped by an IF statement.", scr);

                ActionResponse resp;
                float? delay = null;

                // Process Arguments
                if (scr.OriginalActionArgs.TryGetValue(action, out string[] originalArgs))
                {
                    ArgumentProcessResult res = ArgumentProcessor.Process(action.ExpectedArguments, originalArgs, action, scr);
                    if (res.Errored)
                    {
                        Logger.ScriptError((res.FailedArgument != string.Empty ? $"[Argument: {res.FailedArgument}] " : string.Empty) + res.Message, scr);
                        break;
                    }

                    if (!res.Success)
                    {
                        Logger.Debug("Action was skipped; the argument processor did not return 'success' as true.", scr);
                        continue;
                    }
                    else
                    {
                        Logger.Debug("Action was not skipped; the argument processor returned 'success' as true.", scr);
                    }

                    action.Arguments = res.NewParameters.ToArray();
                    action.RawArguments = res.StrippedRawParameters;
                }

                try
                {
                    switch (action)
                    {
                        case ITimingAction timed:
                            Logger.Debug($"Running {action.Name} action (timed)...", scr);
                            delay = timed.Execute(scr, out resp);
                            break;
                        case IScriptAction scriptAction:
                            Logger.Debug($"Running {action.Name} action...", scr);
                            resp = scriptAction.Execute(scr);
                            break;
                        default:
                            Logger.Debug($"Skipping line (no runnable action on line)", scr);
                            continue;
                    }
                }
                catch (ScriptedEventsException seException)
                {
                    string message = $"[Script: {scr.Name}] [L: {scr.CurrentLine + 1}] {seException.Message}";
                    Logger.ScriptError(message, scr);

                    continue;
                }
                catch (Exception e)
                {
                    string message = $"[Script: {scr.Name}] [L: {scr.CurrentLine + 1}] {ErrorGen.Get(ErrorCode.UnknownActionError, action.Name)}:\n{e}";
                    Logger.ScriptError(message, scr);

                    continue;
                }

                if (!resp.Success)
                {
                    Logger.Debug($"{action.Name} [Line: {scr.CurrentLine + 1}]: FAIL", scr);
                    if (resp.ResponseFlags.HasFlag(ActionFlags.FatalError))
                    {
                        string message = $"[{action.Name}] Fatal action error! {resp.Message}";
                        Logger.ScriptError(message, scr, fatal: true);

                        break;
                    }
                    else if (!scr.SuppressWarnings)
                    {
                        string message = $"[{action.Name}] Action error! {resp.Message}";
                        Logger.ScriptError(message, scr);
                    }
                }
                else
                {
                    Logger.Debug($"{action.Name} [Line: {scr.CurrentLine + 1}]: SUCCESS", scr);
                    successfulLines++;

                    if (resp.ResponseVariables != null && scr.ResultVariableNames.TryGetValue(action, out string[] variableNames))
                    {
                        foreach (var zipped in resp.ResponseVariables.Zip(variableNames, (variable, name) => (variable, name)))
                        {
                            switch (zipped.variable)
                            {
                                case Player[] plrVar:
                                    Logger.Debug($"Action {action.Name} is adding a player variable as '{zipped.name}'.", scr);
                                    scr.UniquePlayerVariables.Add(zipped.name, new(zipped.name, string.Empty, plrVar.ToList()));
                                    break;

                                case string strVar:
                                    Logger.Debug($"Action {action.Name} is adding a variable as '{zipped.name}'.", scr);
                                    scr.UniqueVariables.Add(zipped.name, new(zipped.name, string.Empty, strVar));
                                    break;

                                default:
                                    try
                                    {
                                        scr.UniqueVariables.Add(zipped.name, new(zipped.name, string.Empty, zipped.variable.ToString()));
                                    }
                                    catch (InvalidCastException)
                                    {
                                        Logger.ScriptError($"Action {action.Name} returned a value of an illegal type '{zipped.variable.GetType()}', which cannot be casted back to string. Please report this to the developer.", scr);
                                    }

                                    break;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(resp.Message))
                    {
                        string message = $"[Script: {scr.Name}] [L: {scr.CurrentLine + 1}] [{action.Name}] Response: {resp.Message}";
                        switch (scr.Context)
                        {
                            case ExecuteContext.RemoteAdmin:
                                Player.Get(scr.Sender)?.RemoteAdminMessage(message, true, MainPlugin.Singleton.Name);
                                break;
                            default:
                                Logger.Info(message);
                                break;
                        }
                    }

                    if (delay.HasValue)
                    {
                        Logger.Debug($"Action '{action.Name}' on line {scr.CurrentLine + 1} delaying script. Length of delay: {delay.Value}", scr);
                        yield return delay.Value;
                    }
                }

                lines++;

                if (resp.ResponseFlags.HasFlag(ActionFlags.StopEventExecution))
                    break;

                if (!scr.HasFlag("NOSAFETY"))
                {
                    yield return Timing.WaitForOneFrame;
                }
            }

            scr.DebugLog("-----------");
            scr.DebugLog($"Script {scr.Name} concluded. Total time '{runTime.Elapsed:mm':'ss':'fff}', Executed '{successfulLines}/{lines}' ({Math.Round((float)successfulLines / lines * 100)}%) actions successfully");
            MainPlugin.Info($"Finished running script {scr.Name}.");
            scr.DebugLog("-----------");
            scr.IsRunning = false;

            if (scr.HasFlag("LOOP"))
            {
                scr.DebugLog("Re-running looped script.");
                ReadAndRun(scr.Name, scr.Sender); // so that it re-reads the content of the text file.
            }

            scr.DebugLog("Removing script from running scripts.");
            RunningScripts.Remove(scr);

            if (dispose)
                scr.Dispose();
        }

        private static void DebugLog(string message, Script source)
        {
            Logger.Debug($"[ScriptModule] {message}", source);
        }
    }
}
