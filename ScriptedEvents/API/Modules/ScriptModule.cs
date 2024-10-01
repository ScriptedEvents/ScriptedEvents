namespace ScriptedEvents.API.Modules
{
    using System;
    using System.Collections.Generic;
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

    using Logger = Features.Logger;
    using LogType = Enums.LogType;

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
            catch (Exception e)
            {
                Logger.Error(ErrorGenV2.IOError() + $": {e}");
                return;
            }
        }

        public override void Init()
        {
            base.Init();

            RegisterActions(MainPlugin.Singleton.Assembly);
        }

        public override void Kill()
        {
            base.Kill();
            StopAllScripts();
            ActionTypes.Clear();
        }

        public void RegisterAutorunScripts(IEnumerable<Script> allScripts)
        {
            foreach (Script scr in allScripts)
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

            if (ShouldGenerateFiles)
            {
                Logger.Info($"Thank you for installing Scripted Events! View the README file located at {Path.Combine(MainPlugin.BaseFilePath, "README.txt")} for information on how to use and get the most out of this plugin.");
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

            void Log(string message)
            {
                Logger.Debug($"[ScriptModule] [ReadScript] {message}", script);
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

            List<IAction> actionList = ListPool<IAction>.Pool.Get();

            void AddActionNoArgs(IAction action)
            {
                script.OriginalActionArgs[action] = Array.Empty<string>();
                actionList.Add(action);
            }

            string[] array = allText.Split('\n');
            IAction lastAction = null;
            for (int currentline = 0; currentline < array.Length; currentline++)
            {
                array[currentline] = array[currentline].Trim().Replace("\n", string.Empty).Replace("\r", string.Empty);

                // no action
                string line = array[currentline];
                if (string.IsNullOrWhiteSpace(line))
                {
                    AddActionNoArgs(new NullAction("BLANK LINE"));
                    continue;
                }
                else if (line.StartsWith("##"))
                {
                    inMultilineComment = !inMultilineComment;
                    AddActionNoArgs(new NullAction("MULTI COMMENT"));
                    continue;
                }
                else if (line.StartsWith("#") || inMultilineComment)
                {
                    AddActionNoArgs(new NullAction("COMMENT"));
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

                    AddActionNoArgs(new StartFunctionAction());
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

                    Tuple<bool, string> Lambda()
                    {
                        return new(true, SEParser.ReplaceContaminatedValueSyntax(value, script));
                    }

                    if (script.SmartArguments.ContainsKey(lastAction))
                    {
                        script.SmartArguments[lastAction] = script.SmartArguments[lastAction].Append(Lambda).ToArray();
                    }
                    else
                    {
                        script.SmartArguments[lastAction] = new Func<Tuple<bool, string>>[] { Lambda };
                    }

                    AddActionNoArgs(new NullAction($"SMART ARG"));
                    continue;
                }

                if (keyword == "//::")
                {
                    if (actionList.Count == 0)
                    {
                        Logger.Log("'//::' (smart extractor) syntax can't be used if there isn't any action above it.", LogType.Warning, script, currentline + 1);
                        continue;
                    }

                    string actionName = structureParts[1];
                    string[] actionArgs = structureParts.Skip(2).ToArray();

                    // TODO: implement for external actions
                    if (!TryGetActionType(actionName, out Type actionType1))
                    {
                        Logger.Warn(ErrorGen.Get(ErrorCode.InvalidAction, actionName, scriptName), script);
                        continue;
                    }

                    IAction actionToExtract = Activator.CreateInstance(actionType1) as IAction;

                    if (actionToExtract is ITimingAction)
                    {
                        Logger.Log($"{actionToExtract.Name} is a timing action, which cannot be used with smart extractors.", LogType.Warning, script, currentline + 1);
                        continue;
                    }

                    if (actionToExtract is not IReturnValueAction)
                    {
                        Logger.Log($"{actionToExtract.Name} action does not return any values, therefore can't be used with smart accessors.", LogType.Warning, script, currentline + 1);
                        continue;
                    }

                    Tuple<bool, string> ActionWrapper()
                    {
                        if (!TryRunAction(script, actionToExtract, out ActionResponse resp, out float? _, actionArgs))
                        {
                            return new(false, resp.Message);
                        }

                        if (resp == null || resp.ResponseVariables.Length == 0)
                        {
                            return new(false, "Action did not return any values to use.");
                        }

                        if (resp.ResponseVariables.Length > 1)
                        {
                            Log("Action returned more than 1 value. Using the first one as default.");
                        }

                        object value = resp.ResponseVariables[0];

                        if (value is not string)
                        {
                            return new(false, "Action returned a value that is not a string.");
                        }

                        return new(true, value as string);
                    }

                    if (script.SmartArguments.ContainsKey(lastAction))
                    {
                        script.SmartArguments[lastAction] = script.SmartArguments[lastAction].Append(ActionWrapper).ToArray();
                    }
                    else
                    {
                        script.SmartArguments[lastAction] = new Func<Tuple<bool, string>>[] { ActionWrapper };
                    }

                    AddActionNoArgs(new NullAction($"SMART EXTR"));
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

                    AddActionNoArgs(new NullAction("FLAG DEFINE"));
                    continue;
                }

                // labels
                if (keyword.EndsWith(":"))
                {
                    string labelName = line.Remove(keyword.Length - 1, 1).RemoveWhitespace();

                    if (!script.Labels.ContainsKey(labelName))
                        script.Labels.Add(labelName, currentline);
                    else if (!suppressWarnings)
                        Logger.Warn(ErrorGen.Get(ErrorCode.MultipleLabelDefs, labelName, scriptName));

                    AddActionNoArgs(new NullAction($"{labelName} LABEL"));
                    continue;
                }

                // extractor
                int indexOfExtractor = line.IndexOf("::");
                string[] resultVariableNames = Array.Empty<string>();

                if (indexOfExtractor != -1)
                {
                    string variablesSection = line.Substring(0, indexOfExtractor);
                    string actionSection = line.Substring(indexOfExtractor + 2).TrimStart();
                    Log($"[ExtractorSyntax] Variables section: {variablesSection}");
                    Log($"[ExtractorSyntax] Action section: {actionSection}");

                    resultVariableNames = SEParser.IsolateValueSyntax(variablesSection, script, true, false, false).variables.Select(arg => arg.Value).ToArray();
                    if (resultVariableNames.Length != 0)
                    {
                        Log($"[ExtractorSyntax] Variables found before the syntax: {string.Join(", ", resultVariableNames)}");

                        structureParts = actionSection.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(part => part.Trim()).ToList();
                        keyword = structureParts[0];
                    }
                    else
                    {
                        Logger.Warn("The extraction operator `::` has been used, but no variable names were specified to contain extracted values.", script);
                    }
                }

                keyword = keyword.ToUpper();

                if (!TryGetActionType(keyword, out Type actionType))
                {
                    /*
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

                    */
                    AddActionNoArgs(new NullAction("ERROR"));
                    continue;
                }

                IAction newAction = Activator.CreateInstance(actionType) as IAction;
                lastAction = newAction;
                script.OriginalActionArgs[newAction] = structureParts.Skip(1).Select(str => str.RemoveWhitespace()).ToArray();
                script.ResultVariableNames[newAction] = resultVariableNames;

                Log($"Queuing action {keyword}, {string.Join(", ", script.OriginalActionArgs[newAction])}");

                // Obsolete check
                if (newAction.IsObsolete(out string obsoleteReason) && !suppressWarnings && !script.SuppressWarnings)
                {
                    Logger.Warn($"Action {newAction.Name} is obsolete; {obsoleteReason}", script);
                }

                actionList.Add(newAction);
                ListPool<string>.Pool.Return(structureParts);
            }

            script.Actions = ListPool<IAction>.Pool.ToArrayReturn(actionList);

            Log($"Debug script read successfully. Name: {script.ScriptName} | Actions: {script.Actions.Count(act => act is not NullAction)} | Flags: {string.Join(" ", script.Flags)} | Labels: {string.Join(" ", script.Labels)} | Comments: {script.Actions.Count(action => action is NullAction @null && @null.Type is "COMMENT")}");

            return script;
        }

        /// <summary>
        /// Runs the script and disposes of it as soon as the script execution is complete.
        /// </summary>
        /// <param name="scr">The script to run.</param>
        /// <param name="dispose">If <see langword="true"/>, the script will be disposed after finishing execution.</param>
        /// <exception cref="DisabledScriptException">If <see cref="Script.IsDisabled"/> is <see langword="true"/>.</exception>
        public void RunScript(Script scr, bool dispose = true)
        {
            if (scr.IsDisabled)
                throw new DisabledScriptException(scr.ScriptName);

            CoroutineHandle handle = Timing.RunCoroutine(SafeRunCoroutine(RunScriptInternal(scr, dispose)), $"SCRIPT_{scr.UniqueId}");
            RunningScripts.Add(scr, handle);
        }

        // chatgpt made this amazing thing
        public IEnumerator<float> SafeRunCoroutine(IEnumerator<float> coroutine)
        {
            while (true)
            {
                object current;
                try
                {
                    // Continue running the original coroutine
                    if (coroutine.MoveNext())
                    {
                        current = coroutine.Current;
                    }
                    else
                    {
                        yield break;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"A coroutine error has been caught!\n{e.Message}\n{e.StackTrace}");
                    yield break;
                }

                yield return (float)current;
            }
        }

        /// <summary>
        /// Reads and runs a script.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <param name="executor">The executor that is running the script. Can be null.</param>
        /// <param name="dispose">Whether to dispose of the script as soon as execution is finished.</param>
        /// <exception cref="FileNotFoundException">The script was not found.</exception>
        /// <exception cref="DisabledScriptException">If <see cref="Script.IsDisabled"/> is <see langword="true"/>.</exception>
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

        public static bool TryRunAction(Script scr, IAction action, out ActionResponse actResp, out float? delay, string[] originalActionArgs = null, bool processForDecorators = true)
        {
            actResp = null;
            delay = null;

            void Log(string message)
            {
                Logger.Debug("[ScriptModule] [TryRunAction] " + message, scr);
            }

            if (action is NullAction nullAction)
            {
                Log($"Null action type: {nullAction.Type}");
                return true;
            }

            if (scr.IfActionBlocksExecution && action is not IIgnoresIfActionBlock)
            {
                Log("Action was skipped; the IF block resulted in FALSE and action does not terminate IF blocks.");
                return true;
            }

            if (originalActionArgs == null)
            {
                // Process Arguments
                if (scr.OriginalActionArgs.TryGetValue(action, out string[] xxx))
                {
                    originalActionArgs = xxx;
                }
                else
                {
                    Log("Action does not have any arguments provided.");
                }
            }

            ArgumentProcessResult res = ArgumentProcessor.Process(action.ExpectedArguments, originalActionArgs, action, scr, processForDecorators);
            if (res.Errored)
            {
                string message = (res.FailedArgument != string.Empty ? $"[Argument: {res.FailedArgument}] " : string.Empty) + res.Message;
                actResp = new(false, message);
                return false;
            }

            if (!res.Success)
            {
                Log("Action will not be ran. " + res.Message != null ? res.Message : string.Empty);
                return true;
            }

            action.Arguments = res.NewParameters.ToArray();
            action.RawArguments = res.StrippedRawParameters;

            switch (action)
            {
                case ITimingAction timed:
                    Log($"Running {action.Name} action (timed)...");
                    delay = timed.Execute(scr, out actResp);
                    break;
                case IScriptAction scriptAction:
                    Log($"Running {action.Name} action...");
                    actResp = scriptAction.Execute(scr);
                    break;
                default:
                    Log($"Action is not runnable.");
                    return true;
            }

            if (delay.HasValue)
            {
                Log($"Action '{action.Name}' is delaying the script. Length of delay: {delay.Value}");
            }

            if (!actResp.Success)
            {
                return false;
            }

            Log($"{action.Name} has successfully executed.");
            return true;
        }

        /// <summary>
        /// Internal coroutine to run the script.
        /// </summary>
        /// <param name="scr">The script to run.</param>
        /// <returns>Coroutine iterator.</returns>
        public IEnumerator<float> RunScriptInternal(Script scr, bool dispose = true)
        {
            void Log(string message)
            {
                Logger.Debug("[ScriptModule] [RunScriptInternal] " + message, scr);
            }

            MainPlugin.Info($"Started running the '{scr.ScriptName}' script.");

            yield return Timing.WaitForOneFrame;

            scr.IsRunning = true;
            scr.RunDate = DateTime.Now;

            int lines = 0;
            int successfulLines = 0;

            for (; scr.CurrentLine < scr.Actions.Length; scr.NextLine())
            {
                if (!scr.HasFlag("NOSAFETY"))
                {
                    yield return Timing.WaitForOneFrame;
                }

                if (!scr.Actions.TryGet(scr.CurrentLine, out IAction action) || action == null)
                {
                    Log("There is no runnable action on this line. Skipping...");
                    continue;
                }

                lines++;
                Log("-> Running action " + action.Name);
                ActionResponse resp;

                if (!TryRunAction(scr, action, out resp, out float? delay))
                {
                    Log("Action failed.");
                    if (resp != null && resp.Message.Length > 0)
                    {
                        Logger.ScriptError(resp.Message, scr, true);
                    }

                    continue;
                }

                if (delay.HasValue)
                {
                    yield return delay.Value;
                }

                if (resp == null)
                {
                    continue;
                }

                Log("Action success.");

                if (resp.ResponseFlags.HasFlag(ActionFlags.StopEventExecution))
                    break;

                successfulLines++;

                if (!string.IsNullOrEmpty(resp.Message))
                {
                    string message = $"[Script: {scr.ScriptName}] [Line: {scr.CurrentLine + 1}] [Action: {action.Name}] {resp.Message}";
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

                if (resp.ResponseVariables == null)
                {
                    continue;
                }

                if (!scr.ResultVariableNames.TryGetValue(action, out string[] variableNames))
                {
                    continue;
                }

                foreach (var zipped in resp.ResponseVariables.Zip(variableNames, (variable, name) => (variable, name)))
                {
                    switch (zipped.variable)
                    {
                        case Player[] plrVar:
                            Log($"Action {action.Name} is adding a player variable as '{zipped.name}'.");
                            scr.UniquePlayerVariables.Add(zipped.name, new(zipped.name, string.Empty, plrVar.ToList()));
                            break;

                        case string strVar:
                            Log($"Action {action.Name} is adding a variable as '{zipped.name}'.");
                            scr.AddVariable(zipped.name, string.Empty, strVar);
                            break;

                        default:
                            Logger.ScriptError($"Action '{action.Name}' returned a value of an illegal type '{zipped.variable.GetType()}', which is not supported. Report this error to the developers.", scr);
                            scr.UniqueVariables.Add(zipped.name, new(zipped.name, string.Empty, zipped.variable.ToString()));
                            break;
                    }
                }
            }

            Log("-----------");
            Log($"Script concluded!");
            MainPlugin.Info($"Finished running script {scr.ScriptName}.");
            Log("-----------");
            scr.IsRunning = false;

            Log("Removing script from running scripts.");
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
