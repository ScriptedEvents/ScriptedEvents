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

        public List<string> AutoRunScripts { get; set; }

        /// <inheritdoc/>
        public override string Name { get; } = "ScriptModule";

        public override bool ShouldGenerateFiles
            => !Directory.Exists(BasePath);

        public override void GenerateFiles()
        {
            base.GenerateFiles();

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
                Log.Error(ErrorGen.Get(ErrorCode.IOPermissionError) + $": {e}");
                return;
            }
            catch (Exception e)
            {
                Log.Error(ErrorGen.Get(ErrorCode.IOError) + $": {e}");
                return;
            }

            // Welcome message :)
            // 3s delay to show after other console spam
            Timing.CallDelayed(6f, () =>
            {
                Log.Warn($"Thank you for installing Scripted Events! View the README file located at {Path.Combine(MainPlugin.BaseFilePath, "README.txt")} for information on how to use and get the most out of this plugin.");
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

        public void OnWaitingForPlayers()
        {
            foreach (Script scr in ListScripts())
            {
                if (!scr.HasFlag("AUTORUN"))
                {
                    continue;
                }

                Log.Debug($"Script '{scr.ScriptName}' set to run automatically.");

                try
                {
                    if (scr.AdminEvent)
                    {
                        Log.Warn(ErrorGen.Get(ErrorCode.AutoRun_AdminEvent, scr.ScriptName));
                        continue;
                    }

                    RunScript(scr);
                }
                catch (DisabledScriptException)
                {
                    Log.Warn(ErrorGen.Get(ErrorCode.AutoRun_Disabled, scr.ScriptName));
                }
                catch (FileNotFoundException)
                {
                    Log.Warn(ErrorGen.Get(ErrorCode.AutoRun_NotFound, scr.ScriptName));
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
        /// Reads and returns the text of a script.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <returns>The contents of the script, if it is found.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script is not found.</exception>
        public string ReadScriptText(string scriptName) => InternalRead(scriptName, out _);

        /// <summary>
        /// Returns the file path of a script.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <returns>The directory of the script, if it is found.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script is not found.</exception>
        public string GetFilePath(string scriptName)
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
        public List<Script> ListScripts(ICommandSender sender = null)
        {
            List<Script> scripts = new();
            string[] files = Directory.GetFiles(BasePath, "*.txt", SearchOption.AllDirectories);

            foreach (string file in files)
            {
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
        public Script ReadScript(string scriptName, ICommandSender executor, bool suppressWarnings = false)
        {
            string allText = ReadScriptText(scriptName);
            bool inMultilineComment = false;
            Script script = new();

            List<IAction> actionList = ListPool<IAction>.Pool.Get();

            string[] array = allText.Split('\n');
            for (int currentline = 0; currentline < array.Length; currentline++)
            {
                array[currentline] = array[currentline].TrimStart();

                // NoAction
                string action = array[currentline];
                if (string.IsNullOrWhiteSpace(action))
                {
                    actionList.Add(new NullAction("BLANK LINE"));
                    continue;
                }
                else if (action.StartsWith("##"))
                {
                    inMultilineComment = !inMultilineComment;
                    actionList.Add(new NullAction("COMMENT"));
                    continue;
                }
                else if (action.StartsWith("#") || inMultilineComment)
                {
                    actionList.Add(new NullAction("COMMENT"));
                    continue;
                }
                else if (action.StartsWith("!--"))
                {
                    string flag = action.Replace("!--", string.Empty).Trim();

                    if (!script.HasFlag(flag))
                    {
                        string[] arguments = flag.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        Flag fl = new(arguments[0], arguments.Skip(1));
                        script.Flags.Add(fl);
                    }
                    else if (!suppressWarnings)
                    {
                        Log.Warn(ErrorGen.Get(ErrorCode.MultipleFlagDefs, flag, scriptName));
                    }

                    actionList.Add(new NullAction("FLAG DEFINE"));
                    continue;
                }

                // remove regex for variable spaces bc its politely saying dumb
                // maybe bring it back later cuz i changed my mind
                List<string> actionParts = ListPool<string>.Pool.Get();

                foreach (string str in action.Split(' '))
                {
                    if (string.IsNullOrWhiteSpace(str))
                        continue;

                    actionParts.Add(str);
                }

                string keyword = actionParts[0].RemoveWhitespace();

                // Std labels
                if (keyword.EndsWith(":"))
                {
                    string labelName = action.Remove(keyword.Length - 1, 1).RemoveWhitespace();

                    if (!script.Labels.ContainsKey(labelName))
                        script.Labels.Add(labelName, currentline);
                    else if (!suppressWarnings)
                        Log.Warn(ErrorGen.Get(ErrorCode.MultipleLabelDefs, labelName, scriptName));

                    actionList.Add(new NullAction($"{labelName} LABEL"));

                    continue;
                }

                // Function labels
                if (keyword == "->")
                {
                    if (actionParts.Count < 2)
                    {
                        Log.Error($"[SCRIPT: {script.ScriptName}] [LINE: {currentline}] A function label syntax has been used, but no name has been provided.");
                        continue;
                    }

                    string labelName = actionParts[1].RemoveWhitespace();

                    if (!script.FunctionLabels.ContainsKey(labelName))
                        script.FunctionLabels.Add(labelName, currentline);
                    else if (!suppressWarnings)
                        Log.Warn(ErrorGen.Get(ErrorCode.MultipleLabelDefs, labelName, scriptName));

                    actionList.Add(new StartFunctionAction());
                    continue;
                }

                if (keyword == "<-")
                {
                    actionList.Add(new EndFunctionAction());
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
                        Log.Warn($"[LINE: {currentline + 1}] " + ErrorGen.Get(ErrorCode.InvalidAction, keyword.RemoveWhitespace(), scriptName));

                    actionList.Add(new NullAction("ERROR"));
                    continue;
                }

                IAction newAction = Activator.CreateInstance(actionType) as IAction;
                newAction.RawArguments = actionParts.Skip(1).Select(str => str.RemoveWhitespace()).ToArray();

                // Obsolete check
                if (newAction.IsObsolete(out string obsoleteReason) && !suppressWarnings && !script.SuppressWarnings)
                {
                    Log.Warn($"[L: {script.CurrentLine + 1}] [{scriptName}] Action {newAction.Name} is obsolete; {obsoleteReason}");
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
        /// <param name="dispose">If <see langword="true"/>, the script will be disposed after finishing execution.</param>
        /// <exception cref="DisabledScriptException">If <see cref="Script.Disabled"/> is <see langword="true"/>.</exception>
        public void RunScript(Script scr, bool dispose = true)
        {
            if (scr.Disabled)
                throw new DisabledScriptException(scr.ScriptName);

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
        /// <returns>Whether or not the process errored.</returns>
        public static bool TryGetPlayers(string input, int? amount, out PlayerCollection collection, Script source)
        {
            void Log(string msg)
            {
                DebugLog($"[TryGetPlayers] {msg}", source);
            }

            Log($"Trying to get '{input}' player collection.");

            input = input.RemoveWhitespace();
            List<Player> list = ListPool<Player>.Pool.Get();
            if (input.ToUpper() is "*" or "ALL")
            {
                ListPool<Player>.Pool.Return(list);

                Log($"Fetch successful! Syntax referencing all players.");
                collection = new(Player.List.ToList());
                return true;
            }

            string patternForPlayerIdUsage = @"^\d+(\.\d+)*\.?$";
            if (Regex.IsMatch(input, patternForPlayerIdUsage))
            {
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

                collection = new(list);
                return true;
            }

            string[] variables = VariableSystemV2.IsolateVariables(input, source);
            foreach (string variable in variables)
            {
                Log($"Checking if '{variable}' is a variable containing players");
                try
                {
                    if (VariableSystemV2.TryGetPlayers(variable, source, out PlayerCollection playersFromVariable))
                    {
                        Log("Success! Variable contains players.");
                        list.AddRange(playersFromVariable);
                    }
                    else
                    {
                        Log(playersFromVariable.Message);
                    }
                }
                catch (Exception e)
                {
                    collection = new(null, false, $"Error when processing the {variable} variable: {e.Message}");
                    Log(collection.Message);
                    return false;
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
        public static bool TryGetDoors(string input, out Door[] doors, Script source)
        {
            List<Door> doorList = ListPool<Door>.Pool.Get();
            if (input is "*" or "ALL")
            {
                doorList = Door.List.ToList();
            }
            else if (SEParser.TryParse<ZoneType>(input, out ZoneType zt, source))
            {
                doorList = Door.List.Where(d => d.Zone.HasFlag(zt)).ToList();
            }
            else if (SEParser.TryParse<DoorType>(input, out DoorType dt, source))
            {
                doorList = Door.List.Where(d => d.Type == dt).ToList();
            }
            else if (SEParser.TryParse<RoomType>(input, out RoomType rt, source))
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
                roomList = Room.List.Where(d => d.Name.ToLower() == input.ToLower()).ToList();
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
                if (kvp.Key.ScriptName == name && StopScript(kvp.Key))
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
        internal string InternalRead(string scriptName, out string fileDirectory)
        {
            fileDirectory = null;

            string text = null;
            string mainFolderFile = Path.Combine(BasePath, scriptName + ".txt");
            if (File.Exists(mainFolderFile))
            {
                fileDirectory = mainFolderFile;
                text = File.ReadAllText(mainFolderFile);
            }
            else
            {
                foreach (string directory in Directory.GetDirectories(BasePath))
                {
                    string fileName = Path.Combine(directory, scriptName + ".txt");
                    if (File.Exists(fileName))
                    {
                        fileDirectory = fileName;
                        text = File.ReadAllText(fileName);
                    }
                }
            }

            if (text is not null && fileDirectory is not null)
                return text;

            throw new FileNotFoundException($"Script {scriptName} does not exist.");
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

                    Log.Debug($"Adding Action: {temp.Name} | From Assembly: {assembly.GetName().Name}");
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
            MainPlugin.Info($"Started running the '{scr.ScriptName}' script.");

            yield return Timing.WaitForOneFrame;

            scr.IsRunning = true;
            scr.RunDate = DateTime.UtcNow;

            Stopwatch runTime = Stopwatch.StartNew();
            int lines = 0;
            int successfulLines = 0;

            for (; scr.CurrentLine < scr.Actions.Length; scr.NextLine())
            {
                scr.DebugLog("-----------");
                scr.DebugLog($"Current Line: {scr.CurrentLine + 1}");
                if (!scr.Actions.TryGet(scr.CurrentLine, out IAction action) || action == null)
                    continue;

                if (scr.IfActionBlocksExecution && action is not ITerminatesIfAction)
                    continue;

                ActionResponse resp;
                float? delay = null;

                // Process Arguments
                ArgumentProcessResult res = ArgumentProcessor.Process(action.ExpectedArguments, action.RawArguments, action, scr);
                if (res.Errored)
                {
                    // Todo: Place error better later
                    // -> [WARN] Error trying to check 'value' argument: Invalid Object provided. See all options by running 'shelp Object' in the server console.
                    Log.Error($"[Script {scr.ScriptName}] [Line {scr.CurrentLine + 1}] Error! {res.Message}");
                    break;
                }

                if (!res.Success)
                    continue;

                action.Arguments = res.NewParameters.ToArray();

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
                catch (ScriptedEventsException seException)
                {
                    string message = $"[Script: {scr.ScriptName}] [L: {scr.CurrentLine + 1}] {seException.Message}";
                    LogSystem.ScriptError(message, scr, scr.Context, scr.Sender);

                    continue;
                }
                catch (Exception e)
                {
                    string message = $"[Script: {scr.ScriptName}] [L: {scr.CurrentLine + 1}] {ErrorGen.Get(ErrorCode.UnknownActionError, action.Name)}:\n{e}";
                    LogSystem.ScriptError(message, scr, scr.Context, scr.Sender);

                    continue;
                }

                if (!resp.Success)
                {
                    scr.DebugLog($"{action.Name} [Line: {scr.CurrentLine + 1}]: FAIL");
                    if (resp.ResponseFlags.HasFlag(ActionFlags.FatalError))
                    {
                        string message = $"[Script: {scr.ScriptName}] [L: {scr.CurrentLine + 1}] [{action.Name}] Fatal action error! {resp.Message}";
                        LogSystem.ScriptError(message, scr, scr.Context, scr.Sender, fatal: true);

                        break;
                    }
                    else if (!scr.SuppressWarnings)
                    {
                        string message = $"[Script: {scr.ScriptName}] [L: {scr.CurrentLine + 1}] [{action.Name}] Action error! {resp.Message}";
                        LogSystem.ScriptError(message, scr, scr.Context, scr.Sender);
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

                if (!scr.HasFlag("NOSAFETY"))
                {
                    yield return Timing.WaitForSeconds(0.01f);
                }
            }

            scr.DebugLog("-----------");
            scr.DebugLog($"Script {scr.ScriptName} concluded. Total time '{runTime.Elapsed:mm':'ss':'fff}', Executed '{successfulLines}/{lines}' ({Math.Round((float)successfulLines / lines * 100)}%) actions successfully");
            MainPlugin.Info($"Finished running script {scr.ScriptName}.");
            scr.DebugLog("-----------");
            scr.IsRunning = false;

            if (scr.HasFlag("LOOP"))
            {
                scr.DebugLog("Re-running looped script.");
                ReadAndRun(scr.ScriptName, scr.Sender); // so that it re-reads the content of the text file.
            }

            scr.DebugLog("Removing script from running scripts.");
            RunningScripts.Remove(scr);

            if (dispose)
                scr.Dispose();
        }

        private static void DebugLog(string message, Script source)
        {
            source.DebugLog($"[ScriptModule] {message}");
        }
    }
}
