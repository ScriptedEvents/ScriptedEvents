namespace ScriptedEvents.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;

    using MEC;
    using RemoteAdmin;

    using ScriptedEvents.Actions;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.API.Interfaces;

    using ScriptedEvents.DemoScripts;

    using ScriptedEvents.Structures;

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
                Logger.Error(ErrorGen.Get(ErrorCode.IOPermissionError) + $": {e}");
                return;
            }
            catch (Exception e)
            {
                Logger.Error(ErrorGen.Get(ErrorCode.IOError) + $": {e}");
                return;
            }

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

        public void OnWaitingForPlayers()
        {
            foreach (Script scr in ListScripts())
            {
                if (!scr.HasFlag("AUTORUN"))
                {
                    continue;
                }

                Logger.Debug($"Script '{scr.ScriptName}' set to run automatically.");

                try
                {
                    if (scr.AdminEvent)
                    {
                        Logger.Warn(ErrorGen.Get(ErrorCode.AutoRun_AdminEvent, scr.ScriptName));
                        continue;
                    }

                    RunScript(scr);
                }
                catch (DisabledScriptException)
                {
                    Logger.Warn(ErrorGen.Get(ErrorCode.AutoRun_Disabled, scr.ScriptName));
                }
                catch (FileNotFoundException)
                {
                    Logger.Warn(ErrorGen.Get(ErrorCode.AutoRun_NotFound, scr.ScriptName));
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
            string allText = ReadScriptText(scriptName);
            bool inMultilineComment = false;
            Script script = new();

            void DebugLog(string message)
            {
                Logger.Debug($"[ScriptModule] [ReadScript] {message}", script);
            }

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
                        Logger.Warn(ErrorGen.Get(ErrorCode.MultipleLabelDefs, labelName, scriptName));

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
                        Logger.Warn(ErrorGen.Get(ErrorCode.MultipleLabelDefs, labelName, scriptName));

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
                    DebugLog($"[ExtractorSyntax] Variables section: {variablesSection}");
                    DebugLog($"[ExtractorSyntax] Action section: {actionSection}");

                    resultVariableNames = SEParser.IsolateValueSyntax(variablesSection, script, false);
                    if (resultVariableNames.Length != 0)
                    {
                        DebugLog($"[ExtractorSyntax] Variables found before the syntax: {string.Join(", ", resultVariableNames)}");

                        structureParts = actionSection.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(part => part.Trim()).ToList();
                        keyword = structureParts[0];
                    }
                }

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

                DebugLog($"Queuing action {keyword}, {string.Join(", ", script.OriginalActionArgs[newAction])}");

                // Obsolete check
                if (newAction.IsObsolete(out string obsoleteReason) && !suppressWarnings && !script.SuppressWarnings)
                {
                    Logger.Warn($"Action {newAction.Name} is obsolete; {obsoleteReason}", script);
                }

                actionList.Add(newAction);
                ListPool<string>.Pool.Return(structureParts);
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

            DebugLog($"Debug script read successfully. Name: {script.ScriptName} | Actions: {script.Actions.Count(act => act is not NullAction)} | Flags: {string.Join(" ", script.Flags)} | Labels: {string.Join(" ", script.Labels)} | Comments: {script.Actions.Count(action => action is NullAction @null && @null.Type is "COMMENT")}");

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
            string[] fileNames = Directory.GetFiles(BasePath, $"{scriptName}.txt", SearchOption.AllDirectories);

            if (File.Exists(mainFolderFile))
            {
                fileDirectory = mainFolderFile;
                text = File.ReadAllText(mainFolderFile);
            }
            else if (fileNames.Length == 1)
            {
                string fullFilePath = fileNames.FirstOrDefault();

                fileDirectory = fullFilePath;
                text = File.ReadAllText(fullFilePath);
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
            MainPlugin.Info($"Started running the '{scr.ScriptName}' script.");

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
                    string message = $"[Script: {scr.ScriptName}] [L: {scr.CurrentLine + 1}] {seException.Message}";
                    Logger.ScriptError(message, scr);

                    continue;
                }
                catch (Exception e)
                {
                    string message = $"[Script: {scr.ScriptName}] [L: {scr.CurrentLine + 1}] {ErrorGen.Get(ErrorCode.UnknownActionError, action.Name)}:\n{e}";
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
                        string message = $"[Script: {scr.ScriptName}] [L: {scr.CurrentLine + 1}] [{action.Name}] Response: {resp.Message}";
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
            Logger.Debug($"[ScriptModule] {message}", source);
        }
    }
}
